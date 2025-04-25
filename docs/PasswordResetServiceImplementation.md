# Implementación Técnica del Servicio de Restablecimiento de Contraseña

## Descripción General

Este documento detalla la implementación técnica del servicio de restablecimiento de contraseña (`PasswordResetService`) en el sistema AuthSystem. El servicio sigue los principios de Clean Architecture, separando claramente las responsabilidades y dependencias entre las diferentes capas de la aplicación.

## Estructura del Código

### Interfaz del Servicio (Capa de Dominio)

La interfaz `IPasswordResetService` define los contratos que debe implementar cualquier servicio de restablecimiento de contraseña:

```csharp
// AuthSystem.Domain/Interfaces/Services/IPasswordResetService.cs
public interface IPasswordResetService
{
    Task<string> GenerateResetTokenAsync(User user);
    Task<bool> ValidateResetTokenAsync(User user, string token);
    Task<bool> ResetPasswordAsync(User user, string token, string newPassword);
}
```

### Implementación del Servicio (Capa de Infraestructura)

La clase `PasswordResetService` implementa la interfaz `IPasswordResetService` y proporciona la lógica necesaria para gestionar el proceso de restablecimiento de contraseña:

```csharp
// AuthSystem.Infrastructure/Services/PasswordResetService.cs
public class PasswordResetService : IPasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<PasswordResetService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GenerateResetTokenAsync(User user)
    {
        try
        {
            // Generar un token aleatorio
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            // Establecer la fecha de expiración (1 hora)
            var expirationTime = DateTime.UtcNow.AddHours(1);

            // Guardar el token y la fecha de expiración en el usuario
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expirationTime;

            // Actualizar el usuario en la base de datos
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar token de restablecimiento de contraseña para el usuario: {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> ValidateResetTokenAsync(User user, string token)
    {
        try
        {
            // Verificar que el usuario tenga un token de restablecimiento
            if (string.IsNullOrEmpty(user.PasswordResetToken) || user.PasswordResetTokenExpiry == null)
            {
                _logger.LogWarning("El usuario {UserId} no tiene un token de restablecimiento", user.Id);
                return false;
            }

            // Verificar que el token no haya expirado
            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("El token de restablecimiento para el usuario {UserId} ha expirado", user.Id);
                return false;
            }

            // Verificar que el token coincida
            if (user.PasswordResetToken != token)
            {
                _logger.LogWarning("Token de restablecimiento inválido para el usuario {UserId}", user.Id);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token de restablecimiento de contraseña para el usuario: {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
    {
        try
        {
            // Validar el token
            if (!await ValidateResetTokenAsync(user, token))
            {
                return false;
            }

            // Actualizar la contraseña del usuario
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            
            // Limpiar el token de restablecimiento
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            
            // Actualizar la fecha del último cambio de contraseña
            user.LastPasswordChangeAt = DateTime.UtcNow;

            // Actualizar el usuario en la base de datos
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restablecer la contraseña para el usuario: {UserId}", user.Id);
            return false;
        }
    }
}
```

### Registro del Servicio (Configuración de Dependencias)

El servicio se registra en el contenedor de dependencias en la clase `ServiceCollectionExtensions`:

```csharp
// AuthSystem.API/Extensions/ServiceCollectionExtensions.cs
public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
{
    // Otros registros...

    // Registrar servicio de restablecimiento de contraseña
    services.AddScoped<IPasswordResetService, PasswordResetService>();

    // Otros registros...

    return services;
}
```

## Detalles de Implementación

### Generación de Tokens

La generación de tokens utiliza el generador de números aleatorios criptográficamente seguro de .NET (`RandomNumberGenerator`) para crear tokens seguros:

1. Se generan 32 bytes aleatorios
2. Se convierten a Base64 para obtener una representación en texto
3. Se reemplazan los caracteres especiales de Base64 (`+`, `/`, `=`) por caracteres URL-safe (`-`, `_`, ``)
4. Se establece una fecha de expiración (1 hora por defecto)
5. Se almacena el token y la fecha de expiración en el usuario

### Validación de Tokens

La validación de tokens incluye varias comprobaciones:

1. Verificar que el usuario tenga un token de restablecimiento
2. Verificar que el token no haya expirado
3. Verificar que el token coincida con el proporcionado

### Restablecimiento de Contraseña

El proceso de restablecimiento de contraseña incluye:

1. Validar el token
2. Actualizar la contraseña del usuario (hasheada con BCrypt)
3. Limpiar el token y la fecha de expiración
4. Actualizar la fecha del último cambio de contraseña
5. Guardar los cambios en la base de datos

## Integración con el Controlador

El servicio se utiliza en el controlador `AuthController` para implementar los endpoints de restablecimiento de contraseña:

### Solicitud de Restablecimiento

```csharp
// AuthSystem.API/Controllers/AuthController.cs
[HttpPost("request-password-reset")]
public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
{
    try
    {
        // Buscar el usuario por correo electrónico
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        
        if (user != null && user.UserStatus == UserStatus.Active)
        {
            // Generar token de restablecimiento
            var passwordResetService = HttpContext.RequestServices.GetService(typeof(IPasswordResetService)) as IPasswordResetService;
            var token = await passwordResetService.GenerateResetTokenAsync(user);

            // Construir la URL de restablecimiento
            var baseUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:3000";
            var resetUrl = $"{baseUrl}/reset-password?token={token}&email={Uri.EscapeDataString(user.Email)}";

            // Enviar correo electrónico con el enlace de restablecimiento
            await _userNotificationService.SendPasswordResetEmailAsync(user, token, resetUrl);
        }

        // Por seguridad, siempre devolver el mismo mensaje
        return Ok(new SuccessResponse
        {
            Message = "Si el correo electrónico está registrado, recibirás instrucciones para restablecer tu contraseña"
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al procesar solicitud de restablecimiento de contraseña");
        return StatusCode(500, new ErrorResponse
        {
            Message = "Error al procesar la solicitud de restablecimiento de contraseña"
        });
    }
}
```

### Confirmación de Restablecimiento

```csharp
// AuthSystem.API/Controllers/AuthController.cs
[HttpPost("confirm-password-reset")]
public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request)
{
    try
    {
        // Buscar el usuario por correo electrónico
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        
        if (user == null || user.UserStatus != UserStatus.Active)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Correo electrónico o token inválidos"
            });
        }

        // Validar y procesar el restablecimiento de contraseña
        var passwordResetService = HttpContext.RequestServices.GetService(typeof(IPasswordResetService)) as IPasswordResetService;
        var success = await passwordResetService.ResetPasswordAsync(user, request.Token, request.NewPassword);
        
        if (!success)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Token inválido o expirado"
            });
        }

        // Obtener información del cliente para la notificación
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        // Enviar notificación de cambio de contraseña
        await _userNotificationService.SendPasswordChangedEmailAsync(user, ipAddress, userAgent);

        return Ok(new SuccessResponse
        {
            Message = "Contraseña restablecida correctamente"
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al procesar confirmación de restablecimiento de contraseña");
        return StatusCode(500, new ErrorResponse
        {
            Message = "Error al procesar la confirmación de restablecimiento de contraseña"
        });
    }
}
```

## Entidad Usuario (Propiedades Relacionadas)

La entidad `User` incluye propiedades específicas para el restablecimiento de contraseña:

```csharp
// AuthSystem.Domain/Entities/User.cs
public class User : BaseEntity
{
    // Otras propiedades...

    /// <summary>
    /// Token para restablecer la contraseña
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Fecha de expiración del token para restablecer la contraseña
    /// </summary>
    public DateTime? PasswordResetTokenExpiry { get; set; }

    /// <summary>
    /// Fecha del último cambio de contraseña
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; set; }

    // Otras propiedades...
}
```

## Consideraciones de Seguridad

### Protección contra Ataques de Fuerza Bruta
- Los tokens son suficientemente largos (32 bytes) para resistir ataques de fuerza bruta
- La expiración de tokens limita la ventana de ataque

### Protección contra Enumeración de Usuarios
- El sistema siempre devuelve el mismo mensaje genérico, independientemente de si el correo existe o no

### Almacenamiento Seguro de Contraseñas
- Las contraseñas se almacenan hasheadas usando BCrypt
- El hash de BCrypt incluye un salt aleatorio para cada contraseña

### Notificaciones de Seguridad
- Se envían notificaciones al usuario para alertar sobre cambios en su cuenta
- Las notificaciones incluyen información detallada para ayudar a identificar actividad no autorizada

## Pruebas y Mantenimiento

### Pruebas Unitarias Recomendadas
- Pruebas de generación de tokens
- Pruebas de validación de tokens
- Pruebas de restablecimiento de contraseña
- Pruebas de expiración de tokens

### Mantenimiento
- Revisar periódicamente la duración de la expiración de tokens
- Considerar la implementación de límites de intentos para prevenir ataques
- Monitorear los logs para detectar patrones de uso anómalos
