# Plantillas de Correo Electrónico - Sistema de Autenticación

## Introducción

Este documento describe las plantillas de correo electrónico implementadas en el sistema de autenticación para notificar a los usuarios sobre diferentes eventos relacionados con sus cuentas. Estas plantillas están diseñadas para mejorar la experiencia del usuario, proporcionar información clara y relevante, y aumentar la seguridad del sistema.

## Estructura de las Plantillas

Todas las plantillas siguen una estructura consistente:

1. **Encabezado**: Con un color distintivo según el tipo de notificación
2. **Contenido**: Información clara y estructurada sobre el evento
3. **Información detallada**: Datos específicos sobre el evento
4. **Instrucciones**: Pasos a seguir según el tipo de notificación
5. **Pie de página**: Información de contacto y derechos de autor

## Plantillas Disponibles

### 1. LoginNotification

**Nombre**: `LoginNotification`  
**Asunto**: Inicio de sesión detectado en su cuenta  
**Descripción**: Notifica al usuario cuando se detecta un inicio de sesión en su cuenta.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{LoginDate}`: Fecha y hora del inicio de sesión
- `{IPAddress}`: Dirección IP desde donde se realizó el inicio de sesión
- `{Location}`: Ubicación geográfica aproximada
- `{Device}`: Dispositivo utilizado
- `{Browser}`: Navegador utilizado
- `{SecuritySettingsUrl}`: URL para revisar la configuración de seguridad
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "LoginDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "IPAddress", ipAddress },
    { "Location", location },
    { "Device", device },
    { "Browser", browser },
    { "SecuritySettingsUrl", securitySettingsUrl },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("LoginNotification", user.Email, templateData);
```

### 2. PasswordResetRequest

**Nombre**: `PasswordResetRequest`  
**Asunto**: Solicitud de restablecimiento de contraseña  
**Descripción**: Envía un enlace para restablecer la contraseña cuando el usuario lo solicita.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{ResetUrl}`: URL para restablecer la contraseña
- `{ExpirationTime}`: Tiempo de expiración del enlace
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "ResetUrl", resetUrl },
    { "ExpirationTime", "1 hora" },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("PasswordResetRequest", user.Email, templateData);
```

### 3. AccountLocked

**Nombre**: `AccountLocked`  
**Asunto**: Su cuenta ha sido bloqueada temporalmente  
**Descripción**: Notifica al usuario cuando su cuenta ha sido bloqueada debido a múltiples intentos fallidos de inicio de sesión.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{LockoutDate}`: Fecha y hora del bloqueo
- `{LockoutDuration}`: Duración del bloqueo
- `{UnlockDate}`: Fecha y hora estimada de desbloqueo
- `{ContactSupportUrl}`: URL para contactar a soporte
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "LockoutDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "LockoutDuration", "30 minutos" },
    { "UnlockDate", DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy HH:mm:ss") },
    { "ContactSupportUrl", contactSupportUrl },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("AccountLocked", user.Email, templateData);
```

### 4. PasswordChanged

**Nombre**: `PasswordChanged`  
**Asunto**: Su contraseña ha sido cambiada  
**Descripción**: Notifica al usuario cuando su contraseña ha sido cambiada.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{ChangeDate}`: Fecha y hora del cambio
- `{IPAddress}`: Dirección IP desde donde se realizó el cambio
- `{Device}`: Dispositivo utilizado
- `{ContactSupportUrl}`: URL para contactar a soporte
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "ChangeDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "IPAddress", ipAddress },
    { "Device", device },
    { "ContactSupportUrl", contactSupportUrl },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("PasswordChanged", user.Email, templateData);
```

### 5. UnusualActivity

**Nombre**: `UnusualActivity`  
**Asunto**: Alerta de seguridad: Actividad inusual detectada  
**Descripción**: Alerta al usuario sobre actividad sospechosa en su cuenta.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{ActivityType}`: Tipo de actividad detectada
- `{ActivityDate}`: Fecha y hora de la actividad
- `{IPAddress}`: Dirección IP desde donde se realizó la actividad
- `{Location}`: Ubicación geográfica aproximada
- `{Device}`: Dispositivo utilizado
- `{SecuritySettingsUrl}`: URL para revisar la configuración de seguridad
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "ActivityType", activityType },
    { "ActivityDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "IPAddress", ipAddress },
    { "Location", location },
    { "Device", device },
    { "SecuritySettingsUrl", securitySettingsUrl },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("UnusualActivity", user.Email, templateData);
```

### 6. AccountExpiring

**Nombre**: `AccountExpiring`  
**Asunto**: Su cuenta expirará pronto  
**Descripción**: Notifica al usuario cuando su cuenta está por expirar.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{DaysRemaining}`: Días restantes hasta la expiración
- `{ExpirationDate}`: Fecha de expiración
- `{LoginUrl}`: URL para iniciar sesión
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "DaysRemaining", daysRemaining.ToString() },
    { "ExpirationDate", expirationDate.ToString("dd/MM/yyyy") },
    { "LoginUrl", loginUrl },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("AccountExpiring", user.Email, templateData);
```

### 7. TwoFactorAuthCode

**Nombre**: `TwoFactorAuthCode`  
**Asunto**: Código de verificación para autenticación de dos factores  
**Descripción**: Envía un código de verificación para completar el proceso de autenticación de dos factores.

**Variables de plantilla**:
- `{FullName}`: Nombre completo del usuario
- `{VerificationCode}`: Código de verificación
- `{ExpirationTime}`: Tiempo de expiración del código
- `{LoginAttemptDate}`: Fecha y hora del intento de inicio de sesión
- `{IPAddress}`: Dirección IP desde donde se realizó el intento
- `{Location}`: Ubicación geográfica aproximada
- `{Device}`: Dispositivo utilizado
- `{CurrentYear}`: Año actual

**Ejemplo de uso**:
```csharp
var templateData = new Dictionary<string, string>
{
    { "FullName", user.FullName },
    { "VerificationCode", verificationCode },
    { "ExpirationTime", "5" },
    { "LoginAttemptDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
    { "IPAddress", ipAddress },
    { "Location", location },
    { "Device", device },
    { "CurrentYear", DateTime.Now.Year.ToString() }
};

await _emailService.SendEmailAsync("TwoFactorAuthCode", user.Email, templateData);
```

## Implementación en la Base de Datos

Las plantillas se almacenan en la tabla `EmailTemplates` con la siguiente estructura:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | uniqueidentifier | Identificador único de la plantilla |
| Name | nvarchar | Nombre de la plantilla |
| Subject | nvarchar | Asunto del correo electrónico |
| HtmlContent | nvarchar | Contenido HTML de la plantilla |
| TextContent | nvarchar | Contenido de texto plano de la plantilla |
| Description | nvarchar | Descripción de la plantilla |
| IsActive | bit | Indica si la plantilla está activa |
| CreatedAt | datetime2 | Fecha de creación |
| CreatedBy | nvarchar | Usuario que creó la plantilla |
| LastModifiedAt | datetime2 | Fecha de última modificación |
| LastModifiedBy | nvarchar | Usuario que modificó la plantilla por última vez |

## Uso en el Código

Para utilizar estas plantillas en el código, se debe utilizar el servicio `EmailService` que ya está implementado en el sistema. Este servicio se encarga de cargar la plantilla, reemplazar las variables con los valores proporcionados y enviar el correo electrónico.

```csharp
public async Task<bool> SendEmailAsync(string templateName, string toEmail, Dictionary<string, string> templateData)
{
    // Cargar la plantilla desde la base de datos
    var template = await _emailTemplateRepository.GetByNameAsync(templateName);
    if (template == null || !template.IsActive)
    {
        _logger.LogError($"Plantilla de correo electrónico no encontrada o inactiva: {templateName}");
        return false;
    }

    // Reemplazar variables en el asunto
    string subject = template.Subject;
    foreach (var kvp in templateData)
    {
        subject = subject.Replace($"{{{kvp.Key}}}", kvp.Value);
    }

    // Reemplazar variables en el contenido HTML
    string htmlContent = template.HtmlContent;
    foreach (var kvp in templateData)
    {
        htmlContent = htmlContent.Replace($"{{{kvp.Key}}}", kvp.Value);
    }

    // Reemplazar variables en el contenido de texto plano
    string textContent = template.TextContent;
    foreach (var kvp in templateData)
    {
        textContent = textContent.Replace($"{{{kvp.Key}}}", kvp.Value);
    }

    // Enviar el correo electrónico
    return await SendEmailAsync(toEmail, subject, htmlContent, textContent);
}
```

## Personalización

Las plantillas pueden ser personalizadas según las necesidades del sistema. Se pueden modificar los siguientes aspectos:

1. **Contenido**: Texto, imágenes y estructura
2. **Estilos**: Colores, fuentes y diseño
3. **Variables**: Agregar o modificar variables según los datos disponibles

Para modificar una plantilla, se debe actualizar el registro correspondiente en la tabla `EmailTemplates`.

## Recomendaciones

1. **Pruebas**: Probar las plantillas con diferentes clientes de correo electrónico para garantizar una visualización correcta.
2. **Responsive Design**: Asegurarse de que las plantillas se vean bien en dispositivos móviles.
3. **Accesibilidad**: Utilizar texto alternativo para imágenes y asegurarse de que el contraste de colores sea adecuado.
4. **Contenido de texto plano**: Mantener actualizado el contenido de texto plano para clientes que no admiten HTML.

## Conclusión

Las plantillas de correo electrónico implementadas en el sistema de autenticación proporcionan una forma efectiva de comunicarse con los usuarios sobre eventos importantes relacionados con sus cuentas. Estas plantillas mejoran la experiencia del usuario, aumentan la seguridad del sistema y proporcionan información clara y relevante.
