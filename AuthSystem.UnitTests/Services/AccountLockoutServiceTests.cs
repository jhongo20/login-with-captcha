using System;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthSystem.UnitTests.Services
{
    public class AccountLockoutServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AccountLockoutService>> _mockLogger;
        private readonly AccountLockoutService _accountLockoutService;

        public AccountLockoutServiceTests()
        {
            // Configurar mocks
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AccountLockoutService>>();

            // Configurar IConfiguration
            _mockConfiguration.Setup(x => x.GetValue<int>("Security:MaxFailedLoginAttempts", 5)).Returns(3);
            _mockConfiguration.Setup(x => x.GetValue<int>("Security:LockoutDurationMinutes", 15)).Returns(5);

            // Crear instancia del servicio
            _accountLockoutService = new AccountLockoutService(
                _mockUserRepository.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task RecordFailedLoginAttemptAsync_WithValidUser_ShouldIncrementFailedAttempts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            User updatedUser = null;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            // Act
            var result = await _accountLockoutService.RecordFailedLoginAttemptAsync(userId);

            // Assert
            Assert.False(result); // No debería estar bloqueado después de un solo intento
            Assert.NotNull(updatedUser);
            Assert.Equal(1, updatedUser.AccessFailedCount);
            Assert.Null(updatedUser.LockoutEnd); // No debería estar bloqueado aún
        }

        [Fact]
        public async Task RecordFailedLoginAttemptAsync_ExceedingMaxAttempts_ShouldLockAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                AccessFailedCount = 2 // Ya tiene 2 intentos fallidos, el siguiente debería bloquearlo
            };

            User updatedUser = null;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            // Act
            var result = await _accountLockoutService.RecordFailedLoginAttemptAsync(userId);

            // Assert
            Assert.True(result); // Debería estar bloqueado después de exceder el máximo de intentos
            Assert.NotNull(updatedUser);
            Assert.Equal(3, updatedUser.AccessFailedCount);
            Assert.NotNull(updatedUser.LockoutEnd);
            Assert.True(updatedUser.LockoutEnd > DateTime.UtcNow); // Debería tener una fecha de fin de bloqueo en el futuro
        }

        [Fact]
        public async Task RecordFailedLoginAttemptAsync_WithLockoutDisabled_ShouldNotLockAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = false, // Bloqueo deshabilitado
                AccessFailedCount = 5 // Ya tiene más intentos fallidos que el máximo
            };

            User updatedUser = null;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            // Act
            var result = await _accountLockoutService.RecordFailedLoginAttemptAsync(userId);

            // Assert
            Assert.False(result); // No debería estar bloqueado aunque exceda el máximo de intentos
            Assert.NotNull(updatedUser);
            Assert.Equal(6, updatedUser.AccessFailedCount);
            Assert.Null(updatedUser.LockoutEnd); // No debería tener una fecha de fin de bloqueo
        }

        [Fact]
        public async Task RecordSuccessfulLoginAsync_ShouldResetFailedAttempts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                AccessFailedCount = 2,
                LockoutEnd = DateTime.UtcNow.AddMinutes(5)
            };

            User updatedUser = null;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            // Act
            await _accountLockoutService.RecordSuccessfulLoginAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(0, updatedUser.AccessFailedCount);
            Assert.Null(updatedUser.LockoutEnd);
        }

        [Fact]
        public async Task IsLockedOutAsync_WithLockedAccount_ShouldReturnTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                LockoutEnd = DateTime.UtcNow.AddMinutes(5) // Bloqueado por 5 minutos más
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _accountLockoutService.IsLockedOutAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsLockedOutAsync_WithUnlockedAccount_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                LockoutEnd = null // No está bloqueado
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _accountLockoutService.IsLockedOutAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsLockedOutAsync_WithExpiredLockout_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                LockoutEnd = DateTime.UtcNow.AddMinutes(-5) // Bloqueado, pero ya expiró
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _accountLockoutService.IsLockedOutAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetRemainingLockoutTimeAsync_WithLockedAccount_ShouldReturnRemainingTime()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var lockoutEnd = DateTime.UtcNow.AddMinutes(5); // Bloqueado por 5 minutos más
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                LockoutEnd = lockoutEnd
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(userId);

            // Assert
            Assert.True(remainingTime > 0);
            Assert.True(remainingTime <= 5 * 60); // Debería ser menor o igual a 5 minutos en segundos
        }

        [Fact]
        public async Task GetRemainingLockoutTimeAsync_WithUnlockedAccount_ShouldReturnZero()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                LockoutEnd = null // No está bloqueado
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(userId);

            // Assert
            Assert.Equal(0, remainingTime);
        }

        [Fact]
        public async Task UnlockAccountAsync_ShouldResetLockoutAndFailedAttempts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                LockoutEnabled = true,
                AccessFailedCount = 3,
                LockoutEnd = DateTime.UtcNow.AddMinutes(5)
            };

            User updatedUser = null;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => updatedUser = u)
                .ReturnsAsync((User u, CancellationToken ct) => u);

            // Act
            await _accountLockoutService.UnlockAccountAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(0, updatedUser.AccessFailedCount);
            Assert.Null(updatedUser.LockoutEnd);
        }
    }
}
