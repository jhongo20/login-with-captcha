# Documentación del Sistema de Notificaciones por Correo Electrónico

## Descripción General

El sistema de notificaciones por correo electrónico es un componente integral de la aplicación AuthSystem que permite enviar comunicaciones automáticas a los usuarios cuando ocurren eventos importantes relacionados con su cuenta. Estas notificaciones mejoran la seguridad y la experiencia del usuario al mantenerlos informados sobre la actividad de su cuenta.

## Arquitectura

El sistema de notificaciones por correo electrónico se compone de los siguientes componentes:

1. **EmailService**: Servicio principal encargado de enviar correos electrónicos utilizando plantillas.
2. **EmailTemplateRepository**: Repositorio para acceder a las plantillas de correo electrónico almacenadas en la base de datos.
3. **UserNotificationService**: Servicio que utiliza el EmailService para enviar notificaciones específicas a los usuarios.
4. **Controladores**: Puntos de integración donde se invocan las notificaciones (AuthController, UsersController, etc.).

## Plantillas de Correo Electrónico

Las plantillas de correo electrónico se almacenan en la tabla `EmailTemplates` de la base de datos `AuthSystemNewDb`. Cada plantilla contiene:

- **Id**: Identificador único de la plantilla (GUID).
- **Name**: Nombre de la plantilla (utilizado para referenciarla en el código).
- **Subject**: Asunto del correo electrónico.
- **HtmlContent**: Contenido HTML del correo electrónico.
- **TextContent**: Versión de texto plano del correo electrónico.
- **Description**: Descripción de la plantilla.
- **IsActive**: Indica si la plantilla está activa.
- **CreatedAt**: Fecha de creación.
- **CreatedBy**: Usuario que creó la plantilla.
- **LastModifiedAt**: Fecha de última modificación.
- **LastModifiedBy**: Usuario que realizó la última modificación.

### Plantillas Implementadas

1. **LoginNotification**: Notifica a los usuarios cuando se detecta un inicio de sesión en su cuenta.
2. **PasswordResetRequest**: Envía un enlace para restablecer la contraseña.
3. **AccountLocked**: Alerta a los usuarios cuando su cuenta ha sido bloqueada.
4. **PasswordChanged**: Notifica a los usuarios cuando su contraseña ha sido cambiada.
5. **UnusualActivity**: Alerta a los usuarios sobre actividad sospechosa en su cuenta.
6. **AccountExpiring**: Notifica a los usuarios cuando su cuenta está a punto de expirar.
7. **TwoFactorAuthCode**: Envía un código de verificación para la autenticación de dos factores.

## Variables en las Plantillas

Las plantillas de correo electrónico utilizan variables con el formato `{{NombreVariable}}` que son reemplazadas por valores reales al enviar el correo. Las variables disponibles dependen de cada plantilla.

### Variables Comunes

- `{{FullName}}`: Nombre completo del usuario.
- `{{Username}}`: Nombre de usuario.
- `{{Email}}`: Dirección de correo electrónico del usuario.
- `{{CurrentDate}}`: Fecha actual.
- `{{CurrentYear}}`: Año actual.

### Variables Específicas por Plantilla

#### LoginNotification
- `{{LoginDate}}`: Fecha y hora del inicio de sesión.
- `{{IPAddress}}`: Dirección IP desde donde se realizó el inicio de sesión.
- `{{Location}}`: Ubicación geográfica aproximada.
- `{{Device}}`: Tipo de dispositivo utilizado.
- `{{Browser}}`: Navegador utilizado.
- `{{SecuritySettingsUrl}}`: URL para acceder a la configuración de seguridad.

#### TwoFactorAuthCode
- `{{VerificationCode}}`: Código de verificación para la autenticación de dos factores.
- `{{ExpirationTime}}`: Tiempo de expiración del código en minutos.

## Implementación de Notificaciones

### Notificación de Inicio de Sesión

La notificación de inicio de sesión se envía cuando un usuario inicia sesión exitosamente en el sistema. Esta notificación incluye detalles sobre el inicio de sesión, como la dirección IP, el dispositivo y el navegador utilizados.

#### Flujo de Implementación

1. El usuario inicia sesión a través de `/api/Auth/login` o `/api/Auth/login-with-google-recaptcha`.
2. Después de validar las credenciales y generar el token JWT, el sistema captura la información del cliente:
   - Dirección IP (`HttpContext.Connection.RemoteIpAddress`)
   - User-Agent (`HttpContext.Request.Headers["User-Agent"]`)
3. El sistema analiza el User-Agent para determinar el tipo de dispositivo y navegador.
4. Se invoca el método `SendLoginNotificationAsync` del `UserNotificationService`.
5. El servicio prepara los datos para la plantilla y llama al `EmailService` para enviar el correo.

#### Código Relevante

```csharp
// En AuthController.cs
try
{
    // Obtener la dirección IP y el User-Agent del cliente
    string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
    string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
    
    // Enviar la notificación de forma asíncrona (no esperamos a que termine)
    _ = _userNotificationService.SendLoginNotificationAsync(user, ipAddress, userAgent);
}
catch (Exception ex)
{
    // Registrar el error pero continuar con el proceso de login
    _logger.LogError(ex, "Error al enviar notificación de inicio de sesión para el usuario: {Username}", user.Username);
}
```

### Notificación de Bloqueo de Cuenta

La notificación de bloqueo de cuenta se envía cuando la cuenta de un usuario es bloqueada, ya sea automáticamente después de múltiples intentos fallidos de inicio de sesión o manualmente por un administrador. Esta notificación proporciona información detallada sobre el bloqueo y ofrece opciones para contactar a soporte.

#### Escenarios de Activación

1. **Bloqueo Automático**: Después de 5 intentos fallidos de inicio de sesión (configurable en `appsettings.json`).
2. **Bloqueo Manual**: Cuando un administrador cambia el estado de un usuario a "Locked" a través de:
   - `PUT /api/Users/{id}` (actualización general del usuario)
   - `PATCH /api/Users/{id}/status` (actualización específica del estado)

#### Variables Específicas

- `{{FullName}}`: Nombre completo del usuario.
- `{{Email}}`: Correo electrónico del usuario.
- `{{LockoutDate}}`: Fecha y hora del bloqueo.
- `{{LockoutDuration}}`: Duración del bloqueo (por ejemplo, "15 minutos", "1 hora", "24 horas").
- `{{UnlockDate}}`: Fecha y hora de desbloqueo automático.
- `{{IPAddress}}`: Dirección IP desde donde se realizaron los intentos.
- `{{Location}}`: Ubicación geográfica aproximada.
- `{{Device}}`: Tipo de dispositivo utilizado.
- `{{Browser}}`: Navegador utilizado.
- `{{ContactSupportUrl}}`: URL para contactar a soporte.
- `{{SupportEmail}}`: Correo electrónico de soporte.
- `{{CurrentYear}}`: Año actual para el pie de página.

#### Flujo de Implementación

1. **Bloqueo Automático**:
   - El usuario realiza múltiples intentos fallidos de inicio de sesión.
   - Al alcanzar el límite de intentos (5 por defecto), `AccountLockoutService.RecordFailedLoginAttemptAsync` retorna `true`.
   - El controlador de autenticación captura la información del cliente y llama a `SendAccountLockedEmailAsync`.

2. **Bloqueo Manual**:
   - Un administrador cambia el estado del usuario a "Locked".
   - El controlador de usuarios captura la información del cliente y llama a `SendAccountLockedEmailAsync`.

#### Código Relevante

```csharp
// En AuthController.cs (bloqueo automático)
if (isLocked)
{
    int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
    
    // Obtener la fecha de desbloqueo
    DateTime lockoutEnd = DateTime.Now.AddSeconds(remainingTime);
    
    // Obtener la dirección IP y el User-Agent del cliente
    string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
    string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
    
    // Enviar notificación de cuenta bloqueada
    _ = _userNotificationService.SendAccountLockedEmailAsync(user, lockoutEnd, ipAddress, userAgent);
    
    return StatusCode(403, new ErrorResponse
    {
        Message = $"La cuenta ha sido bloqueada debido a múltiples intentos fallidos. Intente nuevamente en {remainingTime / 60} minutos.",
        LockoutRemainingSeconds = remainingTime
    });
}
```

```csharp
// En UsersController.cs (bloqueo manual)
if (user.UserStatus == UserStatus.Locked)
{
    try
    {
        // Establecer un tiempo de bloqueo (por ejemplo, 24 horas)
        DateTime lockoutEnd = DateTime.Now.AddHours(24);
        
        // Actualizar el LockoutEnd del usuario
        user.LockoutEnd = lockoutEnd;
        
        // Obtener la dirección IP y el User-Agent del cliente
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        // Enviar notificación de cuenta bloqueada
        await _userNotificationService.SendAccountLockedEmailAsync(user, lockoutEnd, ipAddress, userAgent);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error al enviar notificación de bloqueo al usuario {user.Email}");
    }
}
```

### Notificación de Cambio de Contraseña

La notificación de cambio de contraseña se envía cuando un usuario cambia su contraseña exitosamente. Esta notificación proporciona información detallada sobre el cambio y ofrece opciones para contactar a soporte.

#### Escenarios de Activación

1. **Cambio de Contraseña**: Después de que un usuario cambie su contraseña a través de:
   - `PUT /api/Users/{id}/password` (actualización de la contraseña del usuario)

#### Variables Específicas

- `{{FullName}}`: Nombre completo del usuario.
- `{{Email}}`: Correo electrónico del usuario.
- `{{PasswordChangedDate}}`: Fecha y hora del cambio de contraseña.
- `{{IPAddress}}`: Dirección IP desde donde se realizó el cambio.
- `{{Location}}`: Ubicación geográfica aproximada.
- `{{Device}}`: Tipo de dispositivo utilizado.
- `{{Browser}}`: Navegador utilizado.
- `{{ContactSupportUrl}}`: URL para contactar a soporte.
- `{{SupportEmail}}`: Correo electrónico de soporte.
- `{{CurrentYear}}`: Año actual para el pie de página.

#### Flujo de Implementación

1. **Cambio de Contraseña**:
   - El usuario cambia su contraseña a través de `/api/Users/{id}/password`.
   - El controlador de usuarios captura la información del cliente y llama a `SendPasswordChangedEmailAsync`.

#### Código Relevante

```csharp
// En UsersController.cs (cambio de contraseña)
if (user.PasswordChanged)
{
    try
    {
        // Obtener la fecha de cambio de contraseña
        DateTime passwordChangedDate = DateTime.Now;
        
        // Obtener la dirección IP y el User-Agent del cliente
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        // Enviar notificación de cambio de contraseña
        await _userNotificationService.SendPasswordChangedEmailAsync(user, passwordChangedDate, ipAddress, userAgent);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error al enviar notificación de cambio de contraseña al usuario {user.Email}");
    }
}
```

## Consideraciones Importantes

1. **Formato de Variables**: Las variables en las plantillas deben usar el formato `{{NombreVariable}}` (dobles llaves).
2. **Manejo de Errores**: Los errores en el envío de notificaciones se registran pero no interrumpen el flujo principal de la aplicación.
3. **Asincronía**: Las notificaciones se envían de forma asíncrona para no bloquear la respuesta al usuario.
4. **Personalización**: Las plantillas pueden personalizarse modificando su contenido en la base de datos.

## Mantenimiento y Extensión

### Añadir una Nueva Plantilla

1. Crear un script SQL para insertar la nueva plantilla en la tabla `EmailTemplates`.
2. Asegurarse de que las variables en la plantilla usen el formato `{{NombreVariable}}`.
3. Implementar un método en el `UserNotificationService` para enviar la notificación.
4. Integrar la llamada al método en el punto adecuado de la aplicación.

### Modificar una Plantilla Existente

1. Actualizar el contenido de la plantilla en la base de datos.
2. Si se añaden nuevas variables, asegurarse de que el método correspondiente en el `UserNotificationService` incluya esas variables en el diccionario de datos.

## Solución de Problemas

### Variables no se Reemplazan

Si las variables no se reemplazan por sus valores reales en los correos enviados:

1. Verificar que las variables en la plantilla usen el formato `{{NombreVariable}}` (dobles llaves).
2. Comprobar que los nombres de las variables en la plantilla coincidan exactamente con las claves en el diccionario de datos.
3. Revisar los logs para detectar posibles errores en el proceso de envío.

### Correos no se Envían

Si los correos no se envían:

1. Verificar la configuración SMTP en el archivo de configuración.
2. Comprobar que la plantilla exista en la base de datos y esté activa.
3. Revisar los logs para detectar errores en la conexión con el servidor SMTP.

## Conclusión

El sistema de notificaciones por correo electrónico proporciona una forma efectiva de mantener a los usuarios informados sobre eventos importantes relacionados con su cuenta. La implementación modular y basada en plantillas facilita la personalización y extensión del sistema según las necesidades del negocio.
