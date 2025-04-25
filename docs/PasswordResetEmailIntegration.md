# Integración del Sistema de Restablecimiento de Contraseña con Notificaciones por Correo Electrónico

## Descripción General

Este documento describe cómo el sistema de restablecimiento de contraseña se integra con el sistema de notificaciones por correo electrónico en AuthSystem. Esta integración es crucial para proporcionar una experiencia de usuario segura y transparente durante el proceso de recuperación de acceso.

## Componentes Involucrados

### Sistema de Notificaciones por Correo Electrónico

Como se describe en la documentación principal del sistema de notificaciones, los componentes principales son:

1. **EmailService**: Servicio base para enviar correos usando plantillas almacenadas en la base de datos
2. **EmailTemplateRepository**: Accede a las plantillas almacenadas en la base de datos
3. **UserNotificationService**: Implementa la lógica específica para cada tipo de notificación

### Sistema de Restablecimiento de Contraseña

1. **PasswordResetService**: Gestiona la generación y validación de tokens de restablecimiento
2. **AuthController**: Expone los endpoints para solicitar y confirmar el restablecimiento

## Plantillas de Correo Electrónico Utilizadas

El sistema de restablecimiento de contraseña utiliza dos plantillas específicas:

### 1. PasswordResetRequest

Esta plantilla se utiliza cuando un usuario solicita restablecer su contraseña.

**Variables utilizadas**:
- `{{FullName}}`: Nombre completo del usuario
- `{{Email}}`: Correo electrónico del usuario
- `{{ResetToken}}`: Token de restablecimiento generado
- `{{ResetUrl}}`: URL completa para restablecer la contraseña
- `{{ExpirationTime}}`: Tiempo de expiración del token (por defecto: "1 hora")

**Ejemplo de uso en código**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "Email", user.Email },
    { "ResetToken", resetToken },
    { "ResetUrl", resetUrl },
    { "ExpirationTime", "1 hora" }
};

await _emailService.SendEmailAsync(
    "PasswordReset",
    user.Email,
    templateData);
```

### 2. PasswordChanged

Esta plantilla se envía después de que un usuario ha cambiado su contraseña exitosamente, ya sea a través del proceso de restablecimiento o por cambio voluntario.

**Variables utilizadas**:
- `{{FullName}}`: Nombre completo del usuario
- `{{Email}}`: Correo electrónico del usuario
- `{{ChangeDate}}`: Fecha y hora del cambio de contraseña
- `{{IPAddress}}`: Dirección IP desde donde se realizó el cambio
- `{{Device}}`: Tipo de dispositivo utilizado (Computadora, Dispositivo móvil, Tablet)
- `{{Browser}}`: Navegador utilizado (Chrome, Firefox, Edge, etc.)
- `{{SecuritySettingsUrl}}`: URL a la página de configuración de seguridad
- `{{CurrentYear}}`: Año actual para el pie de página

**Ejemplo de uso en código**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "Email", user.Email },
    { "ChangeDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "IPAddress", ipAddress ?? "Desconocida" },
    { "Location", location },
    { "Device", device },
    { "Browser", browser },
    { "SecuritySettingsUrl", "/account/security" },
    { "SupportEmail", "soporte@authsystem.com" },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync(
    "PasswordChanged",
    user.Email,
    templateData);
```

## Flujo de Integración

### 1. Solicitud de Restablecimiento

1. El usuario solicita restablecer su contraseña a través del endpoint `POST /api/Auth/request-password-reset`
2. El controlador `AuthController` valida la solicitud y busca al usuario por su correo electrónico
3. Si el usuario existe y está activo, se genera un token de restablecimiento usando `PasswordResetService`
4. El controlador construye la URL de restablecimiento utilizando la configuración `AppSettings:FrontendBaseUrl`
5. Se llama al método `SendPasswordResetEmailAsync` del servicio `UserNotificationService`
6. Este método prepara los datos de la plantilla y utiliza `EmailService` para enviar el correo

### 2. Confirmación de Restablecimiento

1. El usuario accede al enlace y proporciona su nueva contraseña
2. El usuario envía la solicitud al endpoint `POST /api/Auth/confirm-password-reset`
3. El controlador `AuthController` valida la solicitud y busca al usuario por su correo electrónico
4. Se valida el token utilizando `PasswordResetService` y se actualiza la contraseña si es válido
5. El controlador obtiene información del cliente (IP, User-Agent) del contexto HTTP
6. Se llama al método `SendPasswordChangedEmailAsync` del servicio `UserNotificationService`
7. Este método analiza el User-Agent para determinar el dispositivo y navegador
8. Se preparan los datos de la plantilla y se utiliza `EmailService` para enviar el correo

## Consideraciones Técnicas

### Formato de Variables

Todas las plantillas utilizan el formato de dobles llaves `{{NombreVariable}}` para las variables que serán reemplazadas. Esto es consistente con el resto del sistema de notificaciones.

### Almacenamiento de Plantillas

Las plantillas se almacenan en la tabla `EmailTemplates` de la base de datos `AuthSystemNewDb`. Cada plantilla tiene:
- Un identificador único
- Un nombre (ej. "PasswordReset", "PasswordChanged")
- Contenido HTML y texto plano
- Asunto del correo

### Procesamiento Asíncrono

Todas las operaciones de envío de correo se realizan de forma asíncrona para no bloquear el flujo principal de la aplicación. Esto es especialmente importante para operaciones como el restablecimiento de contraseña, donde la respuesta al usuario debe ser rápida.

## Configuración

La integración utiliza las siguientes configuraciones:

1. **Configuración de correo electrónico** (en `appsettings.json`):
```json
"Email": {
  "SenderName": "AuthSystem",
  "SenderEmail": "correo@ejemplo.com",
  "SmtpServer": "smtp.ejemplo.com",
  "SmtpPort": 587,
  "Username": "usuario",
  "Password": "contraseña",
  "UseSsl": true
}
```

2. **Configuración de URL del frontend** (en `appsettings.json`):
```json
"AppSettings": {
  "FrontendBaseUrl": "http://localhost:3001"
}
```

## Seguridad

1. **Protección contra enumeración de usuarios**: El sistema siempre devuelve el mismo mensaje genérico, independientemente de si el correo existe o no
2. **Tokens de un solo uso**: Los tokens solo pueden usarse una vez y se invalidan después de su uso
3. **Expiración de tokens**: Los tokens tienen una vida útil limitada (1 hora por defecto)
4. **Notificaciones de seguridad**: Se notifica al usuario sobre cambios en su cuenta para detectar actividad no autorizada
5. **Información detallada**: Las notificaciones incluyen información sobre el dispositivo, navegador e IP para ayudar a identificar accesos no autorizados

## Pruebas

Para probar la integración:

1. Configurar correctamente los ajustes de correo en `appsettings.json`
2. Asegurarse de que las plantillas "PasswordReset" y "PasswordChanged" existen en la base de datos
3. Solicitar un restablecimiento de contraseña para un usuario existente
4. Verificar que se recibe el correo con el enlace de restablecimiento
5. Utilizar el enlace para cambiar la contraseña
6. Verificar que se recibe la notificación de cambio de contraseña con los detalles correctos
