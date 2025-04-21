using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using AuthSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthSystem.UnitTests.Repositories
{
    public class RepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public RepositoryTests()
        {
            // Configurar la base de datos en memoria para las pruebas
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyActiveEntities()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            // Agregar datos de prueba
            var activeRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "ActiveRole",
                Description = "Active role for testing",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var inactiveRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "InactiveRole",
                Description = "Inactive role for testing",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = false
            };

            context.Roles.Add(activeRole);
            context.Roles.Add(inactiveRole);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            var roles = result.ToList();
            Assert.Single(roles);
            Assert.Equal("ActiveRole", roles[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "TestRole",
                Description = "Role for testing",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result.Id);
            Assert.Equal("TestRole", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithInactiveEntity_ShouldReturnNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "InactiveRole",
                Description = "Inactive role for testing",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = false
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(roleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithMatchingPredicate_ShouldReturnMatchingEntities()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            // Agregar datos de prueba
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Administrator role",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var userRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Description = "User role",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var inactiveRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Inactive",
                Description = "Inactive role",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = false
            };

            context.Roles.AddRange(adminRole, userRole, inactiveRole);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.FindAsync(r => r.Name.Contains("Admin"));

            // Assert
            var roles = result.ToList();
            Assert.Single(roles);
            Assert.Equal("Admin", roles[0].Name);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityWithGeneratedId()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var role = new Role
            {
                Name = "NewRole",
                Description = "New role for testing",
                CreatedBy = "Test"
            };

            // Act
            var result = await repository.AddAsync(role);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("NewRole", result.Name);
            Assert.True(result.IsActive);
            Assert.NotEqual(default, result.CreatedAt);

            // Verificar que se guardó en la base de datos
            var savedRole = await context.Roles.FindAsync(result.Id);
            Assert.NotNull(savedRole);
            Assert.Equal("NewRole", savedRole.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntityProperties()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "OldName",
                Description = "Old description",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                CreatedBy = "Test",
                IsActive = true
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            // Modificar la entidad
            role.Name = "UpdatedName";
            role.Description = "Updated description";

            // Act
            var result = await repository.UpdateAsync(role);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal("UpdatedName", result.Name);
            Assert.Equal("Updated description", result.Description);
            Assert.NotNull(result.LastModifiedAt);

            // Verificar que se actualizó en la base de datos
            var updatedRole = await context.Roles.FindAsync(roleId);
            Assert.NotNull(updatedRole);
            Assert.Equal("UpdatedName", updatedRole.Name);
            Assert.Equal("Updated description", updatedRole.Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSetIsActiveToFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "RoleToDelete",
                Description = "Role to be deleted",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(role);
            await context.SaveChangesAsync();

            // Assert
            Assert.True(result);

            // Verificar que se marcó como inactivo en la base de datos
            var deletedRole = await context.Roles.FindAsync(roleId);
            Assert.NotNull(deletedRole);
            Assert.False(deletedRole.IsActive);
            Assert.NotNull(deletedRole.LastModifiedAt);
        }

        [Fact]
        public async Task DeleteByIdAsync_WithExistingId_ShouldSetIsActiveToFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "RoleToDelete",
                Description = "Role to be deleted",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteByIdAsync(roleId);
            await context.SaveChangesAsync();

            // Assert
            Assert.True(result);

            // Verificar que se marcó como inactivo en la base de datos
            var deletedRole = await context.Roles.FindAsync(roleId);
            Assert.NotNull(deletedRole);
            Assert.False(deletedRole.IsActive);
        }

        [Fact]
        public async Task DeleteByIdAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new Repository<Role>(context);

            // Act
            var result = await repository.DeleteByIdAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}
