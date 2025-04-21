using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de tokens JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ILogger<JwtService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuración</param>
        /// <param name="userSessionRepository">Repositorio de sesiones de usuario</param>
        /// <param name="logger">Logger</param>
        public JwtService(
            IConfiguration configuration,
            IUserSessionRepository userSessionRepository,
            ILogger<JwtService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        /// <param name="user">Usuario para el que se genera el token</param>
        /// <returns>Token JWT</returns>
        public string GenerateToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roles = user.UserRoles?.Where(ur => ur.IsActive)
                .Select(ur => ur.Role?.Name)
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList() ?? new List<string>();

            var permissions = user.UserRoles?.Where(ur => ur.IsActive && ur.Role?.RolePermissions != null)
                .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive)
                    .Select(rp => rp.Permission?.Name))
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList() ?? new List<string>();

            return GenerateTokenAsync(
                user.Id,
                user.Username,
                user.Email,
                roles,
                permissions,
                null,
                60,
                CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Genera un token JWT
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="email">Correo electrónico</param>
        /// <param name="roles">Roles del usuario</param>
        /// <param name="permissions">Permisos del usuario</param>
        /// <param name="additionalClaims">Reclamaciones adicionales (opcional)</param>
        /// <param name="expirationMinutes">Minutos de expiración (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Token JWT generado</returns>
        public Task<string> GenerateTokenAsync(
            Guid userId,
            string username,
            string email,
            IEnumerable<string> roles,
            IEnumerable<string> permissions,
            IDictionary<string, string> additionalClaims = null,
            int expirationMinutes = 60,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, username),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Agregar roles
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Agregar permisos
                if (permissions != null)
                {
                    foreach (var permission in permissions)
                    {
                        claims.Add(new Claim("permission", permission));
                    }
                }

                // Agregar reclamaciones adicionales
                if (additionalClaims != null)
                {
                    foreach (var claim in additionalClaims)
                    {
                        claims.Add(new Claim(claim.Key, claim.Value));
                    }
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token JWT");
                throw;
            }
        }

        /// <summary>
        /// Genera un token de actualización
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Token de actualización generado</returns>
        public async Task<string> GenerateRefreshTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Generar un token de actualización aleatorio
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);

                // Configurar la fecha de expiración
                var refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
                var now = DateTime.UtcNow;
                var expiresAt = now.AddDays(refreshTokenExpiryDays);

                try
                {
                    // Primero verificamos si ya existe una sesión activa para este usuario
                    var existingSessions = await _userSessionRepository.GetByUserAsync(userId, true, cancellationToken);
                    
                    // Si ya existe una sesión activa, la invalidamos
                    foreach (var session in existingSessions)
                    {
                        session.IsActive = false;
                        session.LastModifiedAt = now;
                        session.LastModifiedBy = "System";
                    }

                    // Creamos una nueva sesión con todas las propiedades requeridas
                    var userSession = new UserSession
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        RefreshToken = refreshToken,
                        ExpiresAt = expiresAt,
                        CreatedAt = now,
                        CreatedBy = "System",
                        LastModifiedAt = now,
                        LastModifiedBy = "System",
                        LastActivity = now,
                        IsActive = true,
                        IpAddress = "", // Valor por defecto
                        DeviceInfo = "" // Valor por defecto
                    };

                    // Agregar la nueva sesión
                    await _userSessionRepository.AddAsync(userSession, cancellationToken);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Error al guardar la sesión de usuario en la base de datos");
                    // Si hay un error al guardar en la base de datos, devolvemos el token de todos modos
                    // para que el usuario pueda iniciar sesión, pero registramos el error
                }

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token de actualización");
                throw;
            }
        }

        /// <summary>
        /// Valida un token de actualización
        /// </summary>
        /// <param name="refreshToken">Token de actualización</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tupla con (esValido, idUsuario)</returns>
        public async Task<(bool isValid, Guid userId)> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return (false, Guid.Empty);
                }

                var userSession = await _userSessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
                if (userSession == null || !userSession.IsActive || userSession.ExpiresAt <= DateTime.UtcNow)
                {
                    return (false, Guid.Empty);
                }

                return (true, userSession.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el token de actualización");
                return (false, Guid.Empty);
            }
        }

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="validateLifetime">Indica si se debe validar la vida útil</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el token es válido</returns>
        public async Task<bool> ValidateTokenAsync(
            string token,
            bool validateLifetime = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var principal = await GetPrincipalFromTokenAsync(token, cancellationToken);
                return principal != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene las reclamaciones de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Reclamaciones del token</returns>
        public Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(
            string token,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateLifetime = false, // No validamos la vida útil aquí para poder renovar tokens expirados
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Task.FromResult<ClaimsPrincipal>(null);
                }

                return Task.FromResult(principal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las reclamaciones del token JWT");
                return Task.FromResult<ClaimsPrincipal>(null);
            }
        }
    }
}
