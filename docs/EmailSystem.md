# Sistema de Correos Electrónicos

## Introducción

El sistema de correos electrónicos de AuthSystem proporciona una forma flexible y potente de enviar notificaciones por correo electrónico a los usuarios. Está diseñado para ser fácilmente extensible y personalizable, permitiendo la creación y gestión de plantillas de correo electrónico a través de una API REST.

## Arquitectura

El sistema de correos electrónicos sigue la arquitectura limpia del proyecto AuthSystem y se compone de los siguientes componentes:

### Capa de Dominio (Domain)

- **EmailTemplate**: Entidad que representa una plantilla de correo electrónico.
- **IEmailService**: Interfaz que define los métodos para enviar correos electrónicos.
- **IEmailTemplateRepository**: Interfaz que define los métodos para gestionar las plantillas de correo electrónico.

### Capa de Infraestructura (Infrastructure)

- **EmailService**: Implementación del servicio de correo electrónico utilizando MailKit.
- **EmailTemplateRepository**: Implementación del repositorio de plantillas de correo electrónico.
- **UserNotificationService**: Servicio para enviar notificaciones específicas a los usuarios.

### Capa de API (API)

- **EmailTemplatesController**: Controlador para gestionar las plantillas de correo electrónico.
- **EmailController**: Controlador para enviar correos electrónicos.

## Plantillas Predefinidas

El sistema incluye las siguientes plantillas predefinidas:

1. **UserCreated**: Enviada cuando se crea un nuevo usuario.
2. **UserUpdated**: Enviada cuando se actualiza un usuario existente.
3. **ActivationCode**: Enviada cuando se genera un código de activación para un usuario.
4. **PasswordReset**: Enviada cuando un usuario solicita restablecer su contraseña.

## Configuración

La configuración del servicio de correo electrónico se realiza en el archivo `appsettings.json`:

```json
"Email": {
  "SenderName": "AuthSystem",
  "SenderEmail": "jhongopruebas@gmail.com",
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "Username": "jhongopruebas@gmail.com",
  "Password": "tnoeowgsvuhfxfcb",
  "UseSsl": true
}
```

> **Nota importante**: Para Gmail, se debe utilizar una contraseña de aplicación generada en la configuración de seguridad de la cuenta de Google, no la contraseña principal de la cuenta.

## Implementación Técnica

### Configuración SMTP para Gmail

El servicio de correo electrónico está configurado para trabajar con Gmail utilizando el protocolo SMTP. Para Gmail, es necesario utilizar la opción `SecureSocketOptions.StartTls` en lugar de `SslOnConnect` para cumplir con los requisitos de seguridad de Gmail:

```csharp
var secureSocketOptions = SecureSocketOptions.StartTls;
await client.ConnectAsync(_configuration["Email:SmtpServer"], 
                          int.Parse(_configuration["Email:SmtpPort"]), 
                          secureSocketOptions);
```

### Controlador de Correos Electrónicos

El controlador `EmailController` proporciona un endpoint para enviar correos electrónicos utilizando plantillas:

```csharp
[HttpPost("send")]
public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
{
    // Validación y procesamiento de la solicitud
    // ...
    
    var result = await _emailService.SendEmailAsync(
        request.TemplateName,
        request.Email,
        request.TemplateData ?? new Dictionary<string, string>(),
        request.Attachments);
        
    // Manejo de la respuesta
    // ...
}
```

> **Nota**: Es importante utilizar el atributo `[FromBody]` para asegurar que ASP.NET Core deserialice correctamente el cuerpo de la solicitud JSON.

### Modelo de Solicitud de Correo Electrónico

El modelo `SendEmailRequest` define la estructura de la solicitud para enviar un correo electrónico:

```csharp
public class SendEmailRequest
{
    public string TemplateName { get; set; }
    public string Email { get; set; }
    public Dictionary<string, string> TemplateData { get; set; }
    public List<EmailAttachment> Attachments { get; set; }
}
```

## Uso del Sistema

### Gestión de Plantillas

#### Obtener todas las plantillas

```http
GET /api/EmailTemplates
```

#### Obtener una plantilla por ID

```http
GET /api/EmailTemplates/{id}
```

#### Crear una nueva plantilla

```http
POST /api/EmailTemplates
```

Ejemplo de cuerpo de la solicitud:

```json
{
  "name": "CustomTemplate",
  "subject": "Asunto del correo",
  "htmlContent": "<p>Contenido HTML con variables como {{VariableName}}</p>",
  "textContent": "Contenido de texto plano con variables como {{VariableName}}",
  "description": "Descripción de la plantilla",
  "isActive": true
}
```

#### Actualizar una plantilla existente

```http
PUT /api/EmailTemplates/{id}
```

#### Eliminar una plantilla

```http
DELETE /api/EmailTemplates/{id}
```

### Envío de Correos Electrónicos

#### Enviar un correo utilizando una plantilla

```http
POST /api/Email/send
```

Ejemplo de cuerpo de la solicitud:

```json
{
  "templateName": "UserCreated",
  "email": "usuario@example.com",
  "templateData": {
    "FullName": "Nombre Completo",
    "Username": "nombre_usuario",
    "Email": "usuario@example.com",
    "CurrentDate": "2023-01-01"
  },
  "attachments": []
}
```

## Herramientas de Prueba

### Script de Prueba PowerShell

Se ha creado un script de PowerShell para probar el envío de correos electrónicos a través del endpoint `/api/Email/send`:

```powershell
# TestEmailEndpoint.ps1
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$emailData = @{
    templateName = "UserCreated"
    email = "jhongopruebas@gmail.com"
    templateData = @{
        FullName = "Juan Pérez"
        Username = "juanperez"
        Email = "juan.perez@ejemplo.com"
        CurrentDate = "23/04/2025"
    }
    attachments = @()
}

$jsonBody = $emailData | ConvertTo-Json -Depth 5

# Realizar la solicitud
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/Email/send" -Method Post -Headers $headers -Body $jsonBody -ErrorAction Stop
    
    Write-Host "Respuesta recibida:"
    Write-Host "Código de estado: $($response.StatusCode)"
    Write-Host "Contenido: $($response.Content)"
} catch {
    Write-Host "Error al enviar el correo electrónico:"
    Write-Host "Código de estado: $($_.Exception.Response.StatusCode.value__)"
    # ...
}
```

### Aplicación de Prueba C#

También se ha creado una aplicación de consola C# para probar directamente el envío de correos electrónicos utilizando MailKit:

```csharp
// EmailTest/Program.cs
static async Task Main(string[] args)
{
    Console.WriteLine("Iniciando prueba de envío de correo electrónico...");

    // Configuración del correo electrónico
    string smtpServer = "smtp.gmail.com";
    int smtpPort = 587;
    string username = "jhongopruebas@gmail.com";
    string password = "tnoeowgsvuhfxfcb";
    string senderName = "AuthSystem";
    string senderEmail = "jhongopruebas@gmail.com";

    // Destinatario y asunto
    string recipientEmail = "jhongopruebas@gmail.com";
    string subject = "Correo de prueba desde AuthSystem";

    // Contenido del correo
    string htmlContent = @"
    <html>
    <head>
        <style>
            body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
            .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
            .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
            .content { padding: 20px; background-color: #f9f9f9; }
            .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
        </style>
    </head>
    <body>
        <div class=""container"">
            <div class=""header"">
                <h1>Correo de prueba</h1>
            </div>
            <div class=""content"">
                <p>Hola,</p>
                <p>Este es un correo de prueba enviado desde el sistema AuthSystem.</p>
                <p>Si estás recibiendo este correo, significa que la configuración de correo electrónico está funcionando correctamente.</p>
                <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
            </div>
            <div class=""footer"">
                <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
            </div>
        </div>
    </body>
    </html>";

    // Código para enviar el correo utilizando MailKit
    // ...
}
```

## Solución de Problemas Comunes

### Error 400 (Bad Request) al enviar correos

Si se recibe un error 400 al intentar enviar correos electrónicos a través del endpoint `/api/Email/send`, verifique lo siguiente:

1. **Formato de la solicitud**: Asegúrese de que el cuerpo de la solicitud JSON tenga el formato correcto y contenga todos los campos requeridos.
2. **Plantilla existente**: Verifique que la plantilla especificada en `templateName` exista en la base de datos.
3. **Datos de la plantilla**: Asegúrese de proporcionar todos los datos necesarios para la plantilla en el campo `templateData`.
4. **Autenticación**: Verifique que el token de autenticación sea válido y tenga los permisos necesarios.

### Problemas de conexión SMTP

Si hay problemas para conectarse al servidor SMTP, verifique lo siguiente:

1. **Configuración SMTP**: Asegúrese de que los valores en `appsettings.json` sean correctos.
2. **Credenciales**: Para Gmail, utilice una contraseña de aplicación generada en la configuración de seguridad de la cuenta de Google.
3. **Opciones de seguridad**: Para Gmail, utilice `SecureSocketOptions.StartTls` en lugar de `SslOnConnect`.
4. **Firewall**: Asegúrese de que el puerto SMTP (587 para Gmail) no esté bloqueado por un firewall.

## Extensión del Sistema

Para añadir una nueva funcionalidad de notificación por correo electrónico:

1. Crear una nueva plantilla de correo electrónico utilizando la API o insertarla directamente en la base de datos.
2. Añadir un nuevo método en `UserNotificationService` para enviar el correo.
3. Llamar al método desde el controlador o servicio correspondiente.

## Consideraciones de Seguridad

- Las contraseñas SMTP se almacenan en la configuración y deben protegerse adecuadamente.
- No se deben incluir datos sensibles en los correos electrónicos.
- Se recomienda utilizar conexiones seguras (SSL/TLS) para el envío de correos.
- Para entornos de producción, considere utilizar un servicio de correo electrónico dedicado como SendGrid, Mailgun o Amazon SES.
