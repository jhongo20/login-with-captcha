# Documentación de Bloqueo de Cuentas

## Descripción General

El sistema de bloqueo de cuentas es una característica de seguridad crítica en AuthSystem que protege las cuentas de usuario contra ataques de fuerza bruta. El sistema bloquea temporalmente las cuentas después de múltiples intentos fallidos de inicio de sesión y notifica a los usuarios sobre el bloqueo.

## Configuración

La funcionalidad de bloqueo de cuentas se configura en el archivo `appsettings.json`:

```json
"Security": {
  "MaxFailedLoginAttempts": 5,
  "LockoutDurationMinutes": 15,
  ...
}
```

- **MaxFailedLoginAttempts**: Define el número de intentos fallidos de inicio de sesión antes de que la cuenta se bloquee (valor predeterminado: 5).
- **LockoutDurationMinutes**: Define la duración del bloqueo en minutos (valor predeterminado: 15).

## Implementación

### Servicio de Bloqueo de Cuentas

El bloqueo de cuentas se implementa a través del servicio `AccountLockoutService`, que proporciona los siguientes métodos:

1. **RecordFailedLoginAttemptAsync**: Registra un intento fallido de inicio de sesión y bloquea la cuenta si se alcanza el límite.
2. **RecordSuccessfulLoginAsync**: Registra un inicio de sesión exitoso y reinicia el contador de intentos fallidos.
3. **IsLockedOutAsync**: Verifica si una cuenta está bloqueada.
4. **GetRemainingLockoutTimeAsync**: Obtiene el tiempo restante de bloqueo en segundos.
5. **UnlockAccountAsync**: Desbloquea una cuenta manualmente.

### Flujo de Bloqueo Automático

1. El usuario intenta iniciar sesión con credenciales incorrectas.
2. El controlador de autenticación llama a `RecordFailedLoginAttemptAsync`.
3. El servicio incrementa el contador de intentos fallidos para el usuario.
4. Si el contador alcanza el límite (5 por defecto), la cuenta se bloquea:
   - Se establece una fecha de fin de bloqueo (DateTime.UtcNow + LockoutDurationMinutes).
   - Se actualiza el campo `LockoutEnd` del usuario en la base de datos.
   - El método devuelve `true` para indicar que la cuenta ha sido bloqueada.
5. El controlador de autenticación envía una notificación por correo electrónico al usuario.
6. El usuario recibe un mensaje de error indicando que su cuenta está bloqueada y cuándo podrá intentar nuevamente.

### Bloqueo Manual

Los administradores pueden bloquear manualmente las cuentas de usuario a través de:

1. **API Endpoint**: `PATCH /api/Users/{id}/status` con `status = "Locked"`.
2. **Interfaz de Administración**: Cambiando el estado del usuario a "Bloqueado".

En ambos casos, se establece una fecha de fin de bloqueo (generalmente 24 horas) y se envía una notificación al usuario.

## Notificaciones por Correo Electrónico

Cuando una cuenta es bloqueada, se envía automáticamente un correo electrónico al usuario utilizando la plantilla `AccountLocked`. Esta notificación incluye:

- Fecha y hora del bloqueo.
- Duración del bloqueo.
- Fecha y hora de desbloqueo automático.
- Dirección IP desde donde se realizaron los intentos.
- Dispositivo y navegador utilizados.
- Instrucciones sobre qué hacer si el usuario no realizó los intentos.
- Opciones para contactar a soporte.

### Implementación de la Notificación

La notificación se implementa en el método `SendAccountLockedEmailAsync` del servicio `UserNotificationService`:

```csharp
public async Task<bool> SendAccountLockedEmailAsync(User user, DateTime lockoutEnd, string ipAddress, string userAgent)
{
    try
    {
        // Calcular la duración del bloqueo
        TimeSpan duration = lockoutEnd - DateTime.Now;
        string formattedDuration = FormatTimeSpan(duration);

        // Extraer información del dispositivo y navegador del User-Agent
        string device = DetermineDevice(userAgent);
        string browser = DetermineBrowser(userAgent);
        string location = await GetLocationFromIpAsync(ipAddress);

        var templateData = new Dictionary<string, string>
        {
            { "FullName", user.FullName },
            { "Email", user.Email },
            { "LockoutDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
            { "LockoutDuration", formattedDuration },
            { "UnlockDate", lockoutEnd.ToString("dd/MM/yyyy HH:mm:ss") },
            { "IPAddress", ipAddress ?? "Desconocida" },
            { "Location", location },
            { "Device", device },
            { "Browser", browser },
            { "ContactSupportUrl", "/contact-support" },
            { "SupportEmail", "soporte@authsystem.com" },
            { "CurrentYear", DateTime.Now.Year.ToString() }
        };

        return await _emailService.SendEmailAsync(
            "AccountLocked",
            user.Email,
            templateData);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al enviar notificación de bloqueo de cuenta al usuario: {Email}", user.Email);
        return false;
    }
}
```

## Integración con Controladores

### AuthController

El controlador de autenticación integra el bloqueo de cuentas en el proceso de inicio de sesión:

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // Validar credenciales
    var user = await _userRepository.GetByUsernameAsync(request.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        if (user != null)
        {
            // Registrar intento fallido y verificar si la cuenta debe ser bloqueada
            bool isLocked = await _accountLockoutService.RecordFailedLoginAttemptAsync(user.Id);
            
            if (isLocked)
            {
                // Obtener tiempo restante de bloqueo
                int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
                
                // Obtener la fecha de desbloqueo
                DateTime lockoutEnd = DateTime.Now.AddSeconds(remainingTime);
                
                // Enviar notificación de cuenta bloqueada
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
                string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                _ = _userNotificationService.SendAccountLockedEmailAsync(user, lockoutEnd, ipAddress, userAgent);
                
                return StatusCode(403, new ErrorResponse
                {
                    Message = $"La cuenta ha sido bloqueada debido a múltiples intentos fallidos. Intente nuevamente en {remainingTime / 60} minutos.",
                    LockoutRemainingSeconds = remainingTime
                });
            }
        }
        
        return Unauthorized(new ErrorResponse { Message = "Credenciales inválidas" });
    }
    
    // Verificar si la cuenta ya está bloqueada
    if (await _accountLockoutService.IsLockedOutAsync(user.Id))
    {
        int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
        return StatusCode(403, new ErrorResponse
        {
            Message = $"La cuenta está bloqueada. Intente nuevamente en {remainingTime / 60} minutos.",
            LockoutRemainingSeconds = remainingTime
        });
    }
    
    // Registrar inicio de sesión exitoso
    await _accountLockoutService.RecordSuccessfulLoginAsync(user.Id);
    
    // Continuar con el proceso de inicio de sesión...
}
```

### UsersController

El controlador de usuarios integra el bloqueo de cuentas en la actualización de estado de usuario:

```csharp
[HttpPatch("{id}/status")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
    {
        return NotFound(new ErrorResponse { Message = "Usuario no encontrado" });
    }
    
    user.UserStatus = request.Status;
    
    if (request.Status == UserStatus.Locked)
    {
        // Establecer un tiempo de bloqueo (por ejemplo, 24 horas)
        DateTime lockoutEnd = DateTime.Now.AddHours(24);
        user.LockoutEnd = lockoutEnd;
        
        // Enviar notificación de cuenta bloqueada
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        await _userNotificationService.SendAccountLockedEmailAsync(user, lockoutEnd, ipAddress, userAgent);
    }
    
    await _userRepository.UpdateAsync(user);
    return Ok(new SuccessResponse { Message = "Estado del usuario actualizado exitosamente" });
}
```

## Plantilla de Correo Electrónico

La plantilla de correo electrónico `AccountLocked` se almacena en la base de datos y se inserta mediante el script SQL `InsertEmailTemplates_AccountLocked.sql`. La plantilla incluye variables con el formato `{{NombreVariable}}` que son reemplazadas por valores reales al enviar el correo.

## Consideraciones de Seguridad

1. **Prevención de Ataques de Fuerza Bruta**: El bloqueo de cuentas es una medida efectiva contra ataques de fuerza bruta, pero debe equilibrarse con la experiencia del usuario.
2. **Notificaciones Oportunas**: Las notificaciones por correo electrónico permiten a los usuarios detectar actividad no autorizada en sus cuentas.
3. **Desbloqueo Manual**: Los administradores pueden desbloquear manualmente las cuentas en caso de bloqueos legítimos.
4. **Registro de Actividad**: Todas las actividades de bloqueo y desbloqueo se registran para auditoría.

## Pruebas

Para probar la funcionalidad de bloqueo de cuentas:

1. Intente iniciar sesión con credenciales incorrectas 5 veces consecutivas.
2. Verifique que la cuenta se bloquee después del quinto intento.
3. Verifique que se envíe una notificación por correo electrónico al usuario.
4. Intente iniciar sesión con credenciales correctas y verifique que se muestre un mensaje de error indicando que la cuenta está bloqueada.
5. Espere a que expire el tiempo de bloqueo (15 minutos por defecto) e intente iniciar sesión nuevamente.

## Mejoras Futuras

1. **Bloqueo Progresivo**: Aumentar la duración del bloqueo con cada bloqueo sucesivo.
2. **Bloqueo Geográfico**: Bloquear inicios de sesión desde ubicaciones inusuales.
3. **Notificaciones Adicionales**: Enviar notificaciones cuando se detecten patrones sospechosos de intentos de inicio de sesión.
4. **Integración con Sistemas de Detección de Intrusiones**: Bloquear direcciones IP que muestren comportamiento malicioso.
