using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.API.Controllers;
using AuthSystem.Domain.Common.Enums;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Models.Users;
using AuthSystem.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthSystem.UnitTests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            // Configurar mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockUserRoleRepository = new Mock<IUserRoleRepository>();
            _mockLogger = new Mock<ILogger<UsersController>>();

            // Configurar UnitOfWork para devolver los repositorios
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.RoleRepository).Returns(_mockRoleRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.UserRoleRepository).Returns(_mockUserRoleRepository.Object);

            // Crear instancia del controlador
            _controller = new UsersController(
                _mockUnitOfWork.Object,
                _mockLogger.Object);

            // Configurar HttpContext con un usuario autenticado
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkWithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    Email = "user1@example.com",
                    FullName = "User One",
                    UserType = UserType.Internal,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user2",
                    Email = "user2@example.com",
                    FullName = "User Two",
                    UserType = UserType.Internal,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetUserById_WithExistingId_ShouldReturnOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
            Assert.Equal("testuser", returnValue.Username);
        }

        [Fact]
        public async Task GetUserById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldReturnCreatedUser()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "newuser",
                Email = "new@example.com",
                FullName = "New User",
                Password = "Password123*",
                UserType = UserType.Internal,
                RoleIds = new List<Guid> { Guid.NewGuid() }
            };

            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            User savedUser = null;
            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => savedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => 
                {
                    u.Id = Guid.NewGuid();
                    return u;
                });

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "User" });

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal("newuser", returnValue.Username);
            Assert.Equal("new@example.com", returnValue.Email);
            
            Assert.NotNull(savedUser);
            Assert.Equal("newuser", savedUser.Username);
            Assert.NotNull(savedUser.PasswordHash);
            Assert.NotEqual("Password123*", savedUser.PasswordHash); // Asegurarse de que la contraseña está hasheada
        }

        [Fact]
        public async Task CreateUser_WithExistingUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "existinguser",
                Email = "new@example.com",
                FullName = "New User",
                Password = "Password123*",
                UserType = UserType.Internal
            };

            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<Domain.Models.Auth.ErrorResponse>(badRequestResult.Value);
            Assert.Contains("Username already exists", errorResponse.Message);
        }

        [Fact]
        public async Task CreateUser_WithExistingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Username = "newuser",
                Email = "existing@example.com",
                FullName = "New User",
                Password = "Password123*",
                UserType = UserType.Internal
            };

            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<Domain.Models.Auth.ErrorResponse>(badRequestResult.Value);
            Assert.Contains("Email already exists", errorResponse.Message);
        }

        [Fact]
        public async Task UpdateUser_WithValidData_ShouldReturnUpdatedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new UpdateUserRequest
            {
                Email = "updated@example.com",
                FullName = "Updated User",
                UserType = UserType.Internal,
                RoleIds = new List<Guid> { Guid.NewGuid() }
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(request.Email, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            User updatedUser = null;
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "User" });

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("existinguser", returnValue.Username);
            Assert.Equal("updated@example.com", returnValue.Email);
            Assert.Equal("Updated User", returnValue.FullName);
            
            Assert.NotNull(updatedUser);
            Assert.Equal("updated@example.com", updatedUser.Email);
            Assert.Equal("Updated User", updatedUser.FullName);
        }

        [Fact]
        public async Task UpdateUser_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest
            {
                Email = "updated@example.com",
                FullName = "Updated User",
                UserType = UserType.Internal
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_WithExistingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new UpdateUserRequest
            {
                Email = "taken@example.com",
                FullName = "Updated User",
                UserType = UserType.Internal
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(request.Email, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<Domain.Models.Auth.ErrorResponse>(badRequestResult.Value);
            Assert.Contains("Email already exists", errorResponse.Message);
        }

        [Fact]
        public async Task DeleteUser_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "userToDelete",
                Email = "delete@example.com",
                FullName = "User To Delete",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.DeleteAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var successResponse = Assert.IsType<Domain.Models.Auth.SuccessResponse>(okResult.Value);
            Assert.True(successResponse.Success);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
