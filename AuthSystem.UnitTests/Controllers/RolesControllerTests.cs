using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.API.Controllers;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Models.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthSystem.UnitTests.Controllers
{
    public class RolesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IPermissionRepository> _mockPermissionRepository;
        private readonly Mock<IRolePermissionRepository> _mockRolePermissionRepository;
        private readonly Mock<ILogger<RolesController>> _mockLogger;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            // Configurar mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockPermissionRepository = new Mock<IPermissionRepository>();
            _mockRolePermissionRepository = new Mock<IRolePermissionRepository>();
            _mockLogger = new Mock<ILogger<RolesController>>();

            // Configurar UnitOfWork para devolver los repositorios
            _mockUnitOfWork.Setup(uow => uow.RoleRepository).Returns(_mockRoleRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.PermissionRepository).Returns(_mockPermissionRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.RolePermissionRepository).Returns(_mockRolePermissionRepository.Object);

            // Crear instancia del controlador
            _controller = new RolesController(
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
        public async Task GetAllRoles_ShouldReturnOkWithRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrator role",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    Description = "Standard user role",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            _mockRoleRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            // Act
            var result = await _controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RoleDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetRoleById_WithExistingId_ShouldReturnOkWithRole()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            // Act
            var result = await _controller.GetRoleById(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RoleDto>(okResult.Value);
            Assert.Equal(roleId, returnValue.Id);
            Assert.Equal("Admin", returnValue.Name);
        }

        [Fact]
        public async Task GetRoleById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.GetRoleById(roleId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetRoleWithPermissions_WithExistingId_ShouldReturnOkWithRoleAndPermissions()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var permissions = new List<Permission>
            {
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.view",
                    Description = "View users",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.create",
                    Description = "Create users",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByRoleAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            // Act
            var result = await _controller.GetRoleWithPermissions(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RoleDetailDto>(okResult.Value);
            Assert.Equal(roleId, returnValue.Id);
            Assert.Equal("Admin", returnValue.Name);
            Assert.Equal(2, returnValue.Permissions.Count());
        }

        [Fact]
        public async Task GetRoleWithPermissions_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.GetRoleWithPermissions(roleId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateRole_WithValidData_ShouldReturnCreatedRole()
        {
            // Arrange
            var request = new CreateRoleRequest
            {
                Name = "NewRole",
                Description = "New role description"
            };

            Role savedRole = null;
            _mockRoleRepository.Setup(repo => repo.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
                .Callback<Role, CancellationToken>((r, ct) => savedRole = r)
                .ReturnsAsync((Role r, CancellationToken ct) => 
                {
                    r.Id = Guid.NewGuid();
                    return r;
                });

            // Act
            var result = await _controller.CreateRole(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<RoleDto>(createdResult.Value);
            Assert.Equal("NewRole", returnValue.Name);
            Assert.Equal("New role description", returnValue.Description);
            
            Assert.NotNull(savedRole);
            Assert.Equal("NewRole", savedRole.Name);
            Assert.Equal("New role description", savedRole.Description);
        }

        [Fact]
        public async Task UpdateRole_WithValidData_ShouldReturnUpdatedRole()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "OldRole",
                Description = "Old role description",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new UpdateRoleRequest
            {
                Name = "UpdatedRole",
                Description = "Updated role description"
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            Role updatedRole = null;
            _mockRoleRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
                .Callback<Role, CancellationToken>((r, ct) => updatedRole = r)
                .ReturnsAsync((Role r, CancellationToken ct) => r);

            // Act
            var result = await _controller.UpdateRole(roleId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RoleDto>(okResult.Value);
            Assert.Equal("UpdatedRole", returnValue.Name);
            Assert.Equal("Updated role description", returnValue.Description);
            
            Assert.NotNull(updatedRole);
            Assert.Equal("UpdatedRole", updatedRole.Name);
            Assert.Equal("Updated role description", updatedRole.Description);
        }

        [Fact]
        public async Task UpdateRole_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var request = new UpdateRoleRequest
            {
                Name = "UpdatedRole",
                Description = "Updated role description"
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.UpdateRole(roleId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "RoleToDelete",
                Description = "Role to delete",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockRoleRepository.Setup(repo => repo.DeleteAsync(role, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var successResponse = Assert.IsType<Domain.Models.Auth.SuccessResponse>(okResult.Value);
            Assert.True(successResponse.Success);
        }

        [Fact]
        public async Task DeleteRole_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AssignPermission_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var permission = new Permission
            {
                Id = permissionId,
                Name = "users.view",
                Description = "View users",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new AssignPermissionRequest
            {
                PermissionId = permissionId
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permission);

            _mockRolePermissionRepository.Setup(repo => repo.ExistsAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            RolePermission savedRolePermission = null;
            _mockRolePermissionRepository.Setup(repo => repo.AddAsync(It.IsAny<RolePermission>(), It.IsAny<CancellationToken>()))
                .Callback<RolePermission, CancellationToken>((rp, ct) => savedRolePermission = rp)
                .ReturnsAsync((RolePermission rp, CancellationToken ct) => 
                {
                    rp.Id = Guid.NewGuid();
                    return rp;
                });

            // Act
            var result = await _controller.AssignPermission(roleId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var successResponse = Assert.IsType<Domain.Models.Auth.SuccessResponse>(okResult.Value);
            Assert.True(successResponse.Success);
            
            Assert.NotNull(savedRolePermission);
            Assert.Equal(roleId, savedRolePermission.RoleId);
            Assert.Equal(permissionId, savedRolePermission.PermissionId);
        }

        [Fact]
        public async Task AssignPermission_WithNonExistingRole_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var request = new AssignPermissionRequest
            {
                PermissionId = permissionId
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.AssignPermission(roleId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AssignPermission_WithNonExistingPermission_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new AssignPermissionRequest
            {
                PermissionId = permissionId
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Permission)null);

            // Act
            var result = await _controller.AssignPermission(roleId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AssignPermission_WithExistingAssignment_ShouldReturnBadRequest()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var permission = new Permission
            {
                Id = permissionId,
                Name = "users.view",
                Description = "View users",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var request = new AssignPermissionRequest
            {
                PermissionId = permissionId
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permission);

            _mockRolePermissionRepository.Setup(repo => repo.ExistsAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AssignPermission(roleId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<Domain.Models.Auth.ErrorResponse>(badRequestResult.Value);
            Assert.Contains("Permission is already assigned to this role", errorResponse.Message);
        }

        [Fact]
        public async Task RemovePermission_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var permission = new Permission
            {
                Id = permissionId,
                Name = "users.view",
                Description = "View users",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var rolePermission = new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permission);

            _mockRolePermissionRepository.Setup(repo => repo.GetByRoleAndPermissionAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolePermission);

            _mockRolePermissionRepository.Setup(repo => repo.DeleteAsync(rolePermission, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemovePermission(roleId, permissionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var successResponse = Assert.IsType<Domain.Models.Auth.SuccessResponse>(okResult.Value);
            Assert.True(successResponse.Success);
        }

        [Fact]
        public async Task RemovePermission_WithNonExistingRole_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _controller.RemovePermission(roleId, permissionId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task RemovePermission_WithNonExistingPermission_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Permission)null);

            // Act
            var result = await _controller.RemovePermission(roleId, permissionId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task RemovePermission_WithNonExistingAssignment_ShouldReturnNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var permission = new Permission
            {
                Id = permissionId,
                Name = "users.view",
                Description = "View users",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(role);

            _mockPermissionRepository.Setup(repo => repo.GetByIdAsync(permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permission);

            _mockRolePermissionRepository.Setup(repo => repo.GetByRoleAndPermissionAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RolePermission)null);

            // Act
            var result = await _controller.RemovePermission(roleId, permissionId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
