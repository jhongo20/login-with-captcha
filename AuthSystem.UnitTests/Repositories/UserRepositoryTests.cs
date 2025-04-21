using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Common.Enums;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using AuthSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthSystem.UnitTests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public UserRepositoryTests()
        {
            // Configurar la base de datos en memoria para las pruebas
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"UserTestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task GetByUsernameAsync_WithExistingUsername_ShouldReturnUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByUsernameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithNonExistingUsername_ShouldReturnNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByUsernameAsync("nonexistentuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithInactiveUser_ShouldReturnNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "inactiveuser",
                Email = "inactive@example.com",
                FullName = "Inactive User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = false
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByUsernameAsync("inactiveuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_WithExistingUsername_ShouldReturnTrue()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.UsernameExistsAsync("existinguser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_WithNonExistingUsername_ShouldReturnFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.UsernameExistsAsync("nonexistentuser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_WithExcludedUserId_ShouldReturnFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.UsernameExistsAsync("existinguser", userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.EmailExistsAsync("existing@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.EmailExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithExcludedUserId_ShouldReturnFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.EmailExistsAsync("existing@example.com", userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByRoleAsync_WithExistingRole_ShouldReturnUsers()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var roleId = Guid.NewGuid();
            var role = new Role
            {
                Id = roleId,
                Name = "TestRole",
                Description = "Test role",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@example.com",
                FullName = "User One",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Username = "user2",
                Email = "user2@example.com",
                FullName = "User Two",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var userRole1 = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var userRole2 = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            context.Roles.Add(role);
            context.Users.AddRange(user1, user2);
            context.UserRoles.AddRange(userRole1, userRole2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByRoleAsync(roleId);

            // Assert
            var users = result.ToList();
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Username == "user1");
            Assert.Contains(users, u => u.Username == "user2");
        }

        [Fact]
        public async Task GetByRoleAsync_WithNonExistingRole_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByRoleAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldIncludeUserRoles()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var role = new Role
            {
                Id = roleId,
                Name = "TestRole",
                Description = "Test role",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = "hashedpassword",
                UserType = UserType.Internal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true
            };

            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test",
                IsActive = true,
                Role = role
            };

            context.Roles.Add(role);
            context.Users.Add(user);
            context.UserRoles.Add(userRole);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.NotNull(result.UserRoles);
            Assert.Single(result.UserRoles);
            Assert.Equal(roleId, result.UserRoles.First().RoleId);
            Assert.NotNull(result.UserRoles.First().Role);
            Assert.Equal("TestRole", result.UserRoles.First().Role.Name);
        }
    }
}
