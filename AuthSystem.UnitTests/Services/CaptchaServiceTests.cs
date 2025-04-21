using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace AuthSystem.UnitTests.Services
{
    public class CaptchaServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<CaptchaService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public CaptchaServiceTests()
        {
            // Configurar mocks
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<CaptchaService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Configurar IConfiguration
            var secretKeySection = new Mock<IConfigurationSection>();
            secretKeySection.Setup(x => x.Value).Returns("test_recaptcha_secret_key");
            _mockConfiguration.Setup(x => x.GetSection("ReCaptcha:SecretKey")).Returns(secretKeySection.Object);
            
            var verifyUrlSection = new Mock<IConfigurationSection>();
            verifyUrlSection.Setup(x => x.Value).Returns("https://www.google.com/recaptcha/api/siteverify");
            _mockConfiguration.Setup(x => x.GetSection("ReCaptcha:VerifyUrl")).Returns(verifyUrlSection.Object);

            // Configurar HttpClient
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        }

        [Fact]
        public async Task ValidateReCaptchaAsync_WithValidResponse_ShouldReturnTrue()
        {
            // Arrange
            var captchaResponse = new CaptchaResponse
            {
                Success = true,
                Score = 0.9m,
                Action = "login",
                ChallengeTs = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Hostname = "localhost"
            };

            var jsonResponse = JsonConvert.SerializeObject(captchaResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = await captchaService.ValidateReCaptchaAsync("valid_token");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateReCaptchaAsync_WithInvalidResponse_ShouldReturnFalse()
        {
            // Arrange
            var captchaResponse = new CaptchaResponse
            {
                Success = false,
                ErrorCodes = new[] { "invalid-input-response" }
            };

            var jsonResponse = JsonConvert.SerializeObject(captchaResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = await captchaService.ValidateReCaptchaAsync("invalid_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateReCaptchaAsync_WithLowScore_ShouldReturnFalse()
        {
            // Arrange
            var captchaResponse = new CaptchaResponse
            {
                Success = true,
                Score = 0.1m, // Score bajo, probablemente un bot
                Action = "login",
                ChallengeTs = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Hostname = "localhost"
            };

            var jsonResponse = JsonConvert.SerializeObject(captchaResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = await captchaService.ValidateReCaptchaAsync("low_score_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateReCaptchaAsync_WithHttpError_ShouldReturnFalse()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = await captchaService.ValidateReCaptchaAsync("any_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateReCaptchaAsync_WithException_ShouldReturnFalse()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = await captchaService.ValidateReCaptchaAsync("any_token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCaptcha_WithCorrectAnswer_ShouldReturnTrue()
        {
            // Arrange
            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);
            var captchaId = captchaService.GenerateCaptcha();
            var captchaInfo = captchaService.GetCaptchaInfo(captchaId);

            // Act
            var result = captchaService.ValidateCaptcha(captchaId, captchaInfo.Answer);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCaptcha_WithIncorrectAnswer_ShouldReturnFalse()
        {
            // Arrange
            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);
            var captchaId = captchaService.GenerateCaptcha();

            // Act
            var result = captchaService.ValidateCaptcha(captchaId, "wrong_answer");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCaptcha_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var result = captchaService.ValidateCaptcha(Guid.NewGuid().ToString(), "any_answer");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GenerateCaptcha_ShouldCreateNewCaptcha()
        {
            // Arrange
            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var captchaId = captchaService.GenerateCaptcha();
            var captchaInfo = captchaService.GetCaptchaInfo(captchaId);

            // Assert
            Assert.NotNull(captchaId);
            Assert.NotNull(captchaInfo);
            Assert.NotEmpty(captchaInfo.Question);
            Assert.NotEmpty(captchaInfo.Answer);
            Assert.NotEmpty(captchaInfo.Options);
        }

        [Fact]
        public void GetCaptchaInfo_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var captchaService = new CaptchaService(_mockConfiguration.Object, _httpClient, _mockLogger.Object);

            // Act
            var captchaInfo = captchaService.GetCaptchaInfo(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(captchaInfo);
        }
    }
}
