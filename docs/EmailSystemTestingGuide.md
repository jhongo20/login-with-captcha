# Guía de Pruebas del Sistema de Correos Electrónicos

## Introducción

Esta guía proporciona instrucciones detalladas para probar el sistema de correos electrónicos de AuthSystem. Las pruebas están diseñadas para verificar que el sistema pueda enviar correos electrónicos correctamente utilizando diferentes plantillas y configuraciones.

## Requisitos Previos

Antes de comenzar las pruebas, asegúrese de tener lo siguiente:

1. La aplicación AuthSystem ejecutándose en modo de desarrollo.
2. Acceso a una cuenta de correo electrónico para recibir los correos de prueba.
3. Un token JWT válido con los permisos necesarios para acceder a los endpoints de correo electrónico.
4. PowerShell o Postman para realizar solicitudes HTTP.

## Pruebas de Configuración

### Verificar la Configuración SMTP

1. Abra el archivo `appsettings.json` y verifique que la configuración de correo electrónico sea correcta:

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

2. Si está utilizando Gmail, asegúrese de que la contraseña sea una contraseña de aplicación generada en la configuración de seguridad de la cuenta de Google.

## Pruebas Unitarias

### Prueba Directa con la Aplicación de Consola

1. Navegue a la carpeta `Scripts/EmailTest`.
2. Ejecute la aplicación de consola:

```powershell
dotnet run
```

3. Verifique que se envíe un correo electrónico de prueba y que se muestre el mensaje "¡Correo enviado correctamente a jhongopruebas@gmail.com!".
4. Compruebe la bandeja de entrada del destinatario para confirmar que el correo se recibió correctamente.

### Prueba con el Script de PowerShell

1. Navegue a la carpeta `Scripts`.
2. Ejecute el script `TestEmailEndpoint.ps1`:

```powershell
.\TestEmailEndpoint.ps1
```

3. Verifique que se muestre un mensaje de éxito con un código de estado 200.
4. Compruebe la bandeja de entrada del destinatario para confirmar que el correo se recibió correctamente.

## Pruebas de Integración

### Prueba de Envío de Correo con Plantilla

1. Obtenga un token JWT válido iniciando sesión en la aplicación o utilizando un token existente.
2. Utilice Postman o PowerShell para enviar una solicitud POST al endpoint `/api/Email/send`:

```powershell
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$emailData = @{
    templateName = "UserCreated"
    email = "destinatario@ejemplo.com"
    templateData = @{
        FullName = "Nombre Completo"
        Username = "nombre_usuario"
        Email = "destinatario@ejemplo.com"
        CurrentDate = (Get-Date -Format "dd/MM/yyyy")
    }
    attachments = @()
}

$jsonBody = $emailData | ConvertTo-Json -Depth 5

Invoke-RestMethod -Uri "http://localhost:5031/api/Email/send" -Method Post -Headers $headers -Body $jsonBody
```

3. Verifique que la respuesta sea exitosa (código 200) y que el correo se reciba correctamente.

### Prueba de Validación de Datos

1. Envíe una solicitud sin especificar el nombre de la plantilla:

```powershell
$emailData = @{
    email = "destinatario@ejemplo.com"
    templateData = @{
        FullName = "Nombre Completo"
    }
    attachments = @()
}
```

2. Verifique que la respuesta sea un error 400 (Bad Request) con un mensaje indicando que el nombre de la plantilla es obligatorio.

3. Envíe una solicitud sin especificar el correo electrónico del destinatario:

```powershell
$emailData = @{
    templateName = "UserCreated"
    templateData = @{
        FullName = "Nombre Completo"
    }
    attachments = @()
}
```

4. Verifique que la respuesta sea un error 400 (Bad Request) con un mensaje indicando que el correo electrónico es obligatorio.

### Prueba de Plantilla Inexistente

1. Envíe una solicitud con un nombre de plantilla que no existe:

```powershell
$emailData = @{
    templateName = "PlantillaInexistente"
    email = "destinatario@ejemplo.com"
    templateData = @{
        FullName = "Nombre Completo"
    }
    attachments = @()
}
```

2. Verifique que la respuesta sea un error 400 (Bad Request) o 404 (Not Found) con un mensaje indicando que la plantilla no existe.

## Pruebas de Rendimiento

### Prueba de Envío Múltiple

1. Cree un script que envíe múltiples correos electrónicos en secuencia:

```powershell
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$startTime = Get-Date

for ($i = 1; $i -le 10; $i++) {
    $emailData = @{
        templateName = "UserCreated"
        email = "destinatario@ejemplo.com"
        templateData = @{
            FullName = "Usuario $i"
            Username = "usuario$i"
            Email = "destinatario@ejemplo.com"
            CurrentDate = (Get-Date -Format "dd/MM/yyyy")
        }
        attachments = @()
    }

    $jsonBody = $emailData | ConvertTo-Json -Depth 5

    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5031/api/Email/send" -Method Post -Headers $headers -Body $jsonBody
        Write-Host "Correo $i enviado correctamente"
    } catch {
        Write-Host "Error al enviar el correo $i: $($_.Exception.Message)"
    }

    # Esperar un segundo entre envíos para no sobrecargar el servidor SMTP
    Start-Sleep -Seconds 1
}

$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host "Tiempo total: $($duration.TotalSeconds) segundos"
```

2. Ejecute el script y verifique que todos los correos se envíen correctamente.
3. Analice el tiempo total y el tiempo promedio por correo para evaluar el rendimiento del sistema.

## Solución de Problemas

### Registro de Errores

Si alguna de las pruebas falla, verifique los registros de la aplicación para obtener más información sobre el error:

1. Revise la consola donde se ejecuta la aplicación para ver los mensajes de registro.
2. Busque mensajes de error relacionados con el envío de correos electrónicos, como problemas de conexión SMTP, errores de autenticación o problemas con las plantillas.

### Problemas Comunes y Soluciones

#### Error 400 (Bad Request)

- **Causa**: Formato incorrecto de la solicitud o datos faltantes.
- **Solución**: Verifique que el cuerpo de la solicitud JSON tenga el formato correcto y contenga todos los campos requeridos.

#### Error de Conexión SMTP

- **Causa**: Problemas para conectarse al servidor SMTP.
- **Solución**: Verifique la configuración SMTP, las credenciales y asegúrese de que el puerto no esté bloqueado por un firewall.

#### Error de Autenticación

- **Causa**: Credenciales incorrectas para el servidor SMTP.
- **Solución**: Para Gmail, utilice una contraseña de aplicación generada en la configuración de seguridad de la cuenta de Google.

#### Plantilla No Encontrada

- **Causa**: La plantilla especificada no existe en la base de datos.
- **Solución**: Verifique que la plantilla exista y esté activa en la base de datos.

## Conclusión

Esta guía de pruebas proporciona un conjunto completo de pruebas para verificar el funcionamiento correcto del sistema de correos electrónicos de AuthSystem. Al seguir estas pruebas, puede asegurarse de que el sistema pueda enviar correos electrónicos correctamente utilizando diferentes plantillas y configuraciones.

Si encuentra algún problema durante las pruebas, consulte la sección de solución de problemas o la documentación principal del sistema de correos electrónicos para obtener más información.
