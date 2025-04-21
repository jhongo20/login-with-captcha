using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUserSessionRepository> _mockUserSessionRepository;
        private readonly Mock<ILogger<JwtService>> _mockLogger;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            // Configurar mocks
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserSessionRepository = new Mock<IUserSessionRepository>();
            _mockLogger = new Mock<ILogger<JwtService>>();

            // Configurar IConfiguration
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(x => x.Value).Returns("S3cr3t_K3y!2023_AuthSystem_JWT_Token_Key_Must_Be_Long_Enough");
            _mockConfiguration.Setup(x => x.GetSection("Jwt:Key")).Returns(configurationSection.Object);
            
            var issuerSection = new Mock<IConfigurationSection>();
            issuerSection.Setup(x => x.Value).Returns("AuthSystem");
            _mockConfiguration.Setup(x => x.GetSection("Jwt:Issuer")).Returns(issuerSection.Object);
            
            var audienceSection = new Mock<IConfigurationSection>();
            audienceSection.Setup(x => x.Value).Returns("AuthSystemClient");
            _mockConfiguration.Setup(x => x.GetSection("Jwt:Audience")).Returns(audienceSection.Object);

            var refreshTokenExpiryDaysSection = new Mock<IConfigurationSection>();
            refreshTokenExpiryDaysSection.Setup(x => x.Value).Returns("7");
            _mockConfiguration.Setup(x => x.GetSection("Jwt:RefreshTokenExpiryDays")).Returns(refreshTokenExpiryDaysSection.Object);

            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("S3cr3t_K3y!2023_AuthSystem_JWT_Token_Key_Must_Be_Long_Enough");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("AuthSystem");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("AuthSystemClient");
            _mockConfiguration.Setup(x => x["Jwt:RefreshTokenExpiryDays"]).Returns("7");

            // Crear instancia del servicio
            _jwtService = new JwtService(
                _mockConfiguration.Object,
                _mockUserSessionRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldReturnValidToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };
            var permissions = new List<string> { "users.view" };

            // Act
            var token = await _jwtService.GenerateTokenAsync(
                userId,
                username,
                email,
                roles,
                permissions);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Verificar que el token es un JWT válido
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            
            Assert.NotNull(jsonToken);
            Assert.Equal(userId.ToString(), jsonToken.Subject);
            Assert.Equal(username, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value);
            Assert.Equal(email, jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value);
            Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(jsonToken.Claims, c => c.Type == "permission" && c.Value == "users.view");
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_ShouldCreateUserSession()
        {
            // Arrange
            var userId = Guid.NewGuid();
            UserSession savedSession = null;

            _mockUserSessionRepository
                .Setup(repo => repo.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Callback<UserSession, CancellationToken>((session, token) => savedSession = session)
                .ReturnsAsync((UserSession session, CancellationToken token) => session);

            // Act
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(userId);

            // Assert
            Assert.NotNull(refreshToken);
            Assert.NotEmpty(refreshToken);
            
            _mockUserSessionRepository.Verify(
                repo => repo.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()),
                Times.Once);
            
            Assert.NotNull(savedSession);
            Assert.Equal(userId, savedSession.UserId);
            Assert.Equal(refreshToken, savedSession.RefreshToken);
            Assert.True(savedSession.IsActive);
            Assert.True(savedSession.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_WithValidToken_ShouldReturnTrueAndUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = "valid_refresh_token";
            var userSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            _mockUserSessionRepository
                .Setup(repo => repo.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            // Act
            var result = await _jwtService.ValidateRefreshTokenAsync(refreshToken);

            // Assert
            Assert.True(result.isValid);
            Assert.Equal(userId, result.userId);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var refreshToken = "invalid_refresh_token";

            _mockUserSessionRepository
                .Setup(repo => repo.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserSession)null);

            // Act
            var result = await _jwtService.ValidateRefreshTokenAsync(refreshToken);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(Guid.Empty, result.userId);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_WithExpiredToken_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = "expired_refresh_token";
            var userSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expirado
                IsActive = true
            };

            _mockUserSessionRepository
                .Setup(repo => repo.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            // Act
            var result = await _jwtService.ValidateRefreshTokenAsync(refreshToken);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(Guid.Empty, result.userId);
        }

        [Fact]
        public async Task ValidateTokenAsync_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };
            var permissions = new List<string> { "users.view" };

            var token = await _jwtService.GenerateTokenAsync(
                userId,
                username,
                email,
                roles,
                permissions);

            // Act
            var isValid = await _jwtService.ValidateTokenAsync(token, false);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateTokenAsync_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var invalidToken = "invalid.token.string";

            // Act
            var isValid = await _jwtService.ValidateTokenAsync(invalidToken, false);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task GetPrincipalFromTokenAsync_WithValidToken_ShouldReturnClaimsPrincipal()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "testuser";
            var email = "test@example.com";
            var roles = new List<string> { "User" };
            var permissions = new List<string> { "users.view" };

            var token = await _jwtService.GenerateTokenAsync(
                userId,
                username,
                email,
                roles,
                permissions);

            // Act
            var principal = await _jwtService.GetPrincipalFromTokenAsync(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal(username, principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value);
            Assert.Equal(email, principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(principal.Claims, c => c.Type == "permission" && c.Value == "users.view");
        }

        [Fact]
        public async Task GetPrincipalFromTokenAsync_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.string";

            // Act
            var principal = await _jwtService.GetPrincipalFromTokenAsync(invalidToken);

            // Assert
            Assert.Null(principal);
        }
    }
}
