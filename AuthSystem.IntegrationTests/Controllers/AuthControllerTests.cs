using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AuthSystem.API;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.IntegrationTests.Helpers;
using Newtonsoft.Json;
using Xunit;

namespace AuthSystem.IntegrationTests.Controllers
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "Admin123*"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.AccessToken);
            Assert.NotNull(authResponse.RefreshToken);
            Assert.Equal("admin", authResponse.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "WrongPassword"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(errorResponse);
            Assert.Equal("Invalid username or password", errorResponse.Message);
        }

        [Fact]
        public async Task Login_WithMissingFields_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "",
                Password = ""
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ShouldReturnNewTokens()
        {
            // Arrange - First login to get tokens
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "Admin123*"
            };

            var loginContent = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponseContent);

            // Now try to refresh the token
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = authResponse.RefreshToken
            };

            var refreshContent = new StringContent(
                JsonConvert.SerializeObject(refreshRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/refresh-token", refreshContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var newAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(newAuthResponse);
            Assert.NotNull(newAuthResponse.AccessToken);
            Assert.NotNull(newAuthResponse.RefreshToken);
            Assert.NotEqual(authResponse.AccessToken, newAuthResponse.AccessToken);
            Assert.NotEqual(authResponse.RefreshToken, newAuthResponse.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = "invalid-refresh-token"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(refreshRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/refresh-token", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Logout_WithValidToken_ShouldReturnOk()
        {
            // Arrange - First login to get tokens
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "Admin123*"
            };

            var loginContent = new StringContent(
                JsonConvert.SerializeObject(loginRequest),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponseContent);

            // Set the token for the client
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

            // Now try to logout
            var logoutRequest = new LogoutRequest
            {
                RefreshToken = authResponse.RefreshToken
            };

            var logoutContent = new StringContent(
                JsonConvert.SerializeObject(logoutRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/logout", logoutContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var successResponse = JsonConvert.DeserializeObject<SuccessResponse>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(successResponse);
            Assert.True(successResponse.Success);
            Assert.Equal("Logout successful", successResponse.Message);

            // Try to use the refresh token again, should fail
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = authResponse.RefreshToken
            };

            var refreshContent = new StringContent(
                JsonConvert.SerializeObject(refreshRequest),
                Encoding.UTF8,
                "application/json");

            var refreshResponse = await _client.PostAsync("/api/auth/refresh-token", refreshContent);
            Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
        }
    }
}
