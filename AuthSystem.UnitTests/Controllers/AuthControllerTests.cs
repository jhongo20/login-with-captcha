using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.API.Controllers;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthSystem.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ICaptchaService> _mockCaptchaService;
        private readonly Mock<IAccountLockoutService> _mockAccountLockoutService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Configurar mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockJwtService = new Mock<IJwtService>();
            _mockCaptchaService = new Mock<ICaptchaService>();
            _mockAccountLockoutService = new Mock<IAccountLockoutService>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            // Crear instancia del controlador
            _controller = new AuthController(
                _mockUnitOfWork.Object,
                _mockJwtService.Object,
                _mockCaptchaService.Object,
                _mockAccountLockoutService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123*"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123*"),
                UserType = Domain.Common.Enums.UserType.Internal,
                LockoutEnabled = true,
                IsActive = true
            };

            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "User" }
            };

            var permissions = new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Name = "users.view" }
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAccountLockoutService.Setup(service => service.IsLockedOutAsync(userId))
                .ReturnsAsync(false);

            _mockUnitOfWork.Setup(uow => uow.Roles.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            _mockUnitOfWork.Setup(uow => uow.Permissions.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            _mockJwtService.Setup(service => service.GenerateTokenAsync(
                    userId,
                    user.Username,
                    user.Email,
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(),
                    null,
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("jwt_token");

            _mockJwtService.Setup(service => service.GenerateRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("refresh_token");

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            
            Assert.Equal(userId, response.UserId);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("test@example.com", response.Email);
            Assert.Equal("Test User", response.FullName);
            Assert.Equal("jwt_token", response.Token);
            Assert.Equal("refresh_token", response.RefreshToken);
            Assert.Contains("User", response.Roles);
            Assert.Contains("users.view", response.Permissions);

            // Verificar que se registró el inicio de sesión exitoso
            _mockAccountLockoutService.Verify(
                service => service.RecordSuccessfulLoginAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123*"),
                UserType = Domain.Common.Enums.UserType.Internal,
                LockoutEnabled = true,
                IsActive = true
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAccountLockoutService.Setup(service => service.IsLockedOutAsync(userId))
                .ReturnsAsync(false);

            _mockAccountLockoutService.Setup(service => service.RecordFailedLoginAttemptAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);
            
            Assert.Equal("Nombre de usuario o contraseña incorrectos", response.Message);

            // Verificar que se registró el intento fallido
            _mockAccountLockoutService.Verify(
                service => service.RecordFailedLoginAttemptAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithLockedAccount_ShouldReturnForbidden()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123*"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123*"),
                UserType = Domain.Common.Enums.UserType.Internal,
                LockoutEnabled = true,
                IsActive = true
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAccountLockoutService.Setup(service => service.IsLockedOutAsync(userId))
                .ReturnsAsync(true);

            _mockAccountLockoutService.Setup(service => service.GetRemainingLockoutTimeAsync(userId))
                .ReturnsAsync(300); // 5 minutos en segundos

            // Act
            var result = await _controller.Login(request);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
            
            var response = Assert.IsType<ErrorResponse>(forbiddenResult.Value);
            Assert.Contains("La cuenta está bloqueada", response.Message);
            Assert.Equal(300, response.LockoutRemainingSeconds);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "nonexistentuser",
                Password = "Password123*"
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);
            
            Assert.Equal("Nombre de usuario o contraseña incorrectos", response.Message);
        }

        [Fact]
        public async Task LoginWithCaptcha_WithValidCaptcha_ShouldReturnOkWithToken()
        {
            // Arrange
            var request = new LoginWithRecaptchaRequest
            {
                Username = "testuser",
                Password = "Password123*",
                CaptchaId = "captcha_id",
                CaptchaResponse = "captcha_response"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123*"),
                UserType = Domain.Common.Enums.UserType.Internal,
                LockoutEnabled = true,
                IsActive = true
            };

            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "User" }
            };

            var permissions = new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Name = "users.view" }
            };

            _mockCaptchaService.Setup(service => service.ValidateCaptchaAsync(request.CaptchaId, request.CaptchaResponse))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(uow => uow.Users.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAccountLockoutService.Setup(service => service.IsLockedOutAsync(userId))
                .ReturnsAsync(false);

            _mockUnitOfWork.Setup(uow => uow.Roles.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            _mockUnitOfWork.Setup(uow => uow.Permissions.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            _mockJwtService.Setup(service => service.GenerateTokenAsync(
                    userId,
                    user.Username,
                    user.Email,
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(),
                    null,
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("jwt_token");

            _mockJwtService.Setup(service => service.GenerateRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("refresh_token");

            // Act
            var result = await _controller.LoginWithCaptcha(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            
            Assert.Equal(userId, response.UserId);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("jwt_token", response.Token);
            Assert.Equal("refresh_token", response.RefreshToken);
        }

        [Fact]
        public async Task LoginWithCaptcha_WithInvalidCaptcha_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new LoginWithRecaptchaRequest
            {
                Username = "testuser",
                Password = "Password123*",
                CaptchaId = "captcha_id",
                CaptchaResponse = "invalid_captcha_response"
            };

            _mockCaptchaService.Setup(service => service.ValidateCaptchaAsync(request.CaptchaId, request.CaptchaResponse))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.LoginWithCaptcha(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            
            Assert.Equal("CAPTCHA inválido", response.Message);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ShouldReturnNewToken()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "valid_refresh_token"
            };

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                UserType = Domain.Common.Enums.UserType.Internal,
                LockoutEnabled = true,
                IsActive = true
            };

            var userSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = "valid_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "User" }
            };

            var permissions = new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Name = "users.view" }
            };

            _mockJwtService.Setup(service => service.ValidateRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, userId));

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockAccountLockoutService.Setup(service => service.IsLockedOutAsync(userId))
                .ReturnsAsync(false);

            _mockUnitOfWork.Setup(uow => uow.Roles.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            _mockUnitOfWork.Setup(uow => uow.Permissions.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            _mockJwtService.Setup(service => service.GenerateTokenAsync(
                    userId,
                    user.Username,
                    user.Email,
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(),
                    null,
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("new_jwt_token");

            _mockJwtService.Setup(service => service.GenerateRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("new_refresh_token");

            _mockUnitOfWork.Setup(uow => uow.UserSessions.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            // Act
            var result = await _controller.RefreshToken(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            
            Assert.Equal(userId, response.UserId);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("new_jwt_token", response.Token);
            Assert.Equal("new_refresh_token", response.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "invalid_refresh_token"
            };

            _mockJwtService.Setup(service => service.ValidateRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, Guid.Empty));

            // Act
            var result = await _controller.RefreshToken(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);
            
            Assert.Equal("Token de actualización inválido o expirado", response.Message);
        }

        [Fact]
        public async Task Logout_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "valid_refresh_token"
            };

            var userId = Guid.NewGuid();
            var userSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = "valid_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            // Configurar claims para el usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _mockUnitOfWork.Setup(uow => uow.UserSessions.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            // Act
            var result = await _controller.Logout(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<SuccessResponse>(okResult.Value);
            
            Assert.Equal("Sesión cerrada correctamente", response.Message);

            // Verificar que se eliminó la sesión
            _mockUnitOfWork.Verify(
                uow => uow.UserSessions.DeleteAsync(userSession, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCaptcha_ShouldReturnCaptchaResponse()
        {
            // Arrange
            var captchaResponse = new CaptchaResponse
            {
                CaptchaId = "captcha_id",
                CaptchaImage = "base64_image",
                RequireCaptcha = true
            };

            _mockCaptchaService.Setup(service => service.GenerateCaptchaAsync())
                .ReturnsAsync(captchaResponse);

            // Act
            var result = await _controller.GetCaptcha();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CaptchaResponse>(okResult.Value);
            
            Assert.Equal("captcha_id", response.CaptchaId);
            Assert.Equal("base64_image", response.CaptchaImage);
            Assert.True(response.RequireCaptcha);
        }

        [Fact]
        public async Task GetCaptcha_WithUsername_ShouldCheckRequirement()
        {
            // Arrange
            var username = "testuser";
            var captchaResponse = new CaptchaResponse
            {
                CaptchaId = "captcha_id",
                CaptchaImage = "base64_image",
                RequireCaptcha = true
            };

            _mockCaptchaService.Setup(service => service.CheckCaptchaRequirementAsync(username))
                .ReturnsAsync(captchaResponse);

            // Act
            var result = await _controller.GetCaptcha(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CaptchaResponse>(okResult.Value);
            
            Assert.Equal("captcha_id", response.CaptchaId);
            Assert.Equal("base64_image", response.CaptchaImage);
            Assert.True(response.RequireCaptcha);
        }
    }
}
