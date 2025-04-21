using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de bloqueo de cuentas
    /// </summary>
    public class AccountLockoutService : IAccountLockoutService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountLockoutService> _logger;
        
        // Almacenamiento en memoria para los intentos fallidos y tiempos de bloqueo
        private readonly ConcurrentDictionary<Guid, int> _failedLoginAttempts = new ConcurrentDictionary<Guid, int>();
        private readonly ConcurrentDictionary<Guid, DateTime> _lockoutEndTime = new ConcurrentDictionary<Guid, DateTime>();

        // Configuración predeterminada
        private readonly int _maxFailedAttempts;
        private readonly int _lockoutDurationMinutes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository">Repositorio de usuarios</param>
        /// <param name="configuration">Configuración</param>
        /// <param name="logger">Logger</param>
        public AccountLockoutService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AccountLockoutService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Cargar configuración
            int.TryParse(_configuration["Security:MaxFailedLoginAttempts"], out _maxFailedAttempts);
            if (_maxFailedAttempts == 0) _maxFailedAttempts = 5; // Valor predeterminado
            
            int.TryParse(_configuration["Security:LockoutDurationMinutes"], out _lockoutDurationMinutes);
            if (_lockoutDurationMinutes == 0) _lockoutDurationMinutes = 15; // Valor predeterminado
        }

        /// <summary>
        /// Registra un intento fallido de inicio de sesión
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si la cuenta ha sido bloqueada</returns>
        public async Task<bool> RecordFailedLoginAttemptAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return false;
                }

                // Verificar si el usuario existe y tiene habilitado el bloqueo
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.LockoutEnabled)
                {
                    return false;
                }

                // Incrementar el contador de intentos fallidos
                int attempts = _failedLoginAttempts.AddOrUpdate(
                    userId,
                    1,
                    (_, currentAttempts) => currentAttempts + 1);

                // Actualizar el usuario en la base de datos
                user.AccessFailedCount = attempts;
                await _userRepository.UpdateAsync(user);

                // Verificar si se debe bloquear la cuenta
                if (attempts >= _maxFailedAttempts)
                {
                    // Establecer el tiempo de fin de bloqueo
                    DateTime lockoutEnd = DateTime.UtcNow.AddMinutes(_lockoutDurationMinutes);
                    _lockoutEndTime[userId] = lockoutEnd;

                    // Actualizar el usuario en la base de datos
                    user.LockoutEnd = lockoutEnd;
                    await _userRepository.UpdateAsync(user);

                    _logger.LogWarning("La cuenta del usuario {UserId} ha sido bloqueada hasta {LockoutEnd}", userId, lockoutEnd);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar intento fallido de inicio de sesión para el usuario {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Registra un inicio de sesión exitoso
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Task</returns>
        public async Task RecordSuccessfulLoginAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return;
                }

                // Restablecer el contador de intentos fallidos
                _failedLoginAttempts.TryRemove(userId, out _);
                _lockoutEndTime.TryRemove(userId, out _);

                // Actualizar el usuario en la base de datos
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    await _userRepository.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar inicio de sesión exitoso para el usuario {UserId}", userId);
            }
        }

        /// <summary>
        /// Verifica si una cuenta está bloqueada
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si la cuenta está bloqueada</returns>
        public async Task<bool> IsLockedOutAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return false;
                }

                // Verificar si el usuario existe
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.LockoutEnabled)
                {
                    return false;
                }

                // Verificar el bloqueo en la base de datos
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value.DateTime > DateTime.UtcNow)
                {
                    return true;
                }

                // Verificar el bloqueo en memoria
                if (_lockoutEndTime.TryGetValue(userId, out DateTime lockoutEnd))
                {
                    return lockoutEnd > DateTime.UtcNow;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si la cuenta del usuario {UserId} está bloqueada", userId);
                return false;
            }
        }

        /// <summary>
        /// Obtiene el tiempo restante de bloqueo
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Tiempo restante de bloqueo en segundos, o 0 si no está bloqueado</returns>
        public async Task<int> GetRemainingLockoutTimeAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return 0;
                }

                // Verificar si el usuario existe
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.LockoutEnabled)
                {
                    return 0;
                }

                DateTime? lockoutEnd = null;

                // Verificar el bloqueo en la base de datos
                if (user.LockoutEnd.HasValue)
                {
                    lockoutEnd = user.LockoutEnd.Value.DateTime;
                }
                // Verificar el bloqueo en memoria
                else if (_lockoutEndTime.TryGetValue(userId, out DateTime memoryLockoutEnd))
                {
                    lockoutEnd = memoryLockoutEnd;
                }

                if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.UtcNow)
                {
                    TimeSpan remainingTime = lockoutEnd.Value - DateTime.UtcNow;
                    return (int)remainingTime.TotalSeconds;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tiempo restante de bloqueo para el usuario {UserId}", userId);
                return 0;
            }
        }

        /// <summary>
        /// Desbloquea una cuenta
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Task</returns>
        public async Task UnlockAccountAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return;
                }

                // Restablecer el contador de intentos fallidos
                _failedLoginAttempts.TryRemove(userId, out _);
                _lockoutEndTime.TryRemove(userId, out _);

                // Actualizar el usuario en la base de datos
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    await _userRepository.UpdateAsync(user);

                    _logger.LogInformation("La cuenta del usuario {UserId} ha sido desbloqueada", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desbloquear la cuenta del usuario {UserId}", userId);
            }
        }
    }
}
