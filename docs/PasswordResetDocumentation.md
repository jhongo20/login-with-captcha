# Documentación de Restablecimiento de Contraseña

## Descripción General

El sistema de restablecimiento de contraseña permite a los usuarios recuperar el acceso a sus cuentas cuando han olvidado sus credenciales. Este proceso está diseñado para ser seguro, fácil de usar y transparente, notificando a los usuarios en cada paso del proceso.

## Arquitectura

La funcionalidad de restablecimiento de contraseña está implementada siguiendo la arquitectura de capas de la aplicación:

### Capa de Dominio
- **Entidades**: `User` con propiedades `PasswordResetToken` y `PasswordResetTokenExpiry`
- **Interfaces**: `IPasswordResetService` define los contratos para la generación y validación de tokens

### Capa de Infraestructura
- **Servicios**: 
  - `PasswordResetService`: Implementa la lógica de generación y validación de tokens
  - `UserNotificationService`: Envía notificaciones por correo electrónico

### Capa de API
- **Controladores**: `AuthController` expone los endpoints para solicitar y confirmar el restablecimiento

## Flujo de Trabajo

### 1. Solicitud de Restablecimiento

**Endpoint**: `POST /api/Auth/request-password-reset`

**Payload**:
```json
{
  "email": "usuario@ejemplo.com"
}
```

**Proceso**:
1. El usuario proporciona su dirección de correo electrónico
2. El sistema verifica si el correo existe y si el usuario está activo
3. Se genera un token aleatorio seguro con una expiración de 1 hora
4. Se envía un correo electrónico al usuario con un enlace para restablecer la contraseña

**Consideraciones de Seguridad**:
- Por razones de seguridad, el sistema siempre responde con un mensaje genérico, independientemente de si el correo existe o no
- El token se genera usando un generador de números aleatorios criptográficamente seguro
- El token tiene una expiración limitada (1 hora)

### 2. Confirmación de Restablecimiento

**Endpoint**: `POST /api/Auth/confirm-password-reset`

**Payload**:
```json
{
  "email": "usuario@ejemplo.com",
  "token": "token_generado_previamente",
  "newPassword": "nueva_contraseña",
  "confirmPassword": "nueva_contraseña"
}
```

**Proceso**:
1. El usuario proporciona su correo, el token recibido y la nueva contraseña
2. El sistema valida el token y verifica que no haya expirado
3. Si el token es válido, se actualiza la contraseña del usuario
4. Se envía una notificación al usuario informando del cambio de contraseña
5. Se limpian los datos de restablecimiento (token y fecha de expiración)

**Consideraciones de Seguridad**:
- La contraseña se almacena hasheada usando BCrypt
- Se envía una notificación con detalles del cambio (IP, dispositivo, navegador) para alertar de posibles accesos no autorizados
- Se actualiza la fecha del último cambio de contraseña para fines de auditoría

## Plantillas de Correo Electrónico

### 1. Solicitud de Restablecimiento (PasswordReset)

Esta plantilla se utiliza para enviar el enlace de restablecimiento al usuario. Incluye:
- Saludo personalizado con el nombre del usuario
- Enlace para restablecer la contraseña
- Información sobre la expiración del enlace
- Instrucciones sobre qué hacer si el usuario no solicitó el restablecimiento

### 2. Confirmación de Cambio (PasswordChanged)

Esta plantilla se envía después de que la contraseña ha sido cambiada exitosamente. Incluye:
- Confirmación del cambio de contraseña
- Fecha y hora del cambio
- Dirección IP desde donde se realizó el cambio
- Dispositivo y navegador utilizados
- Instrucciones sobre qué hacer si el usuario no realizó el cambio

## Configuración

La funcionalidad de restablecimiento de contraseña utiliza las siguientes configuraciones en `appsettings.json`:

```json
"AppSettings": {
  "FrontendBaseUrl": "http://localhost:3001"
}
```

- `FrontendBaseUrl`: URL base del frontend para construir el enlace de restablecimiento de contraseña

## Consideraciones para Desarrollo y Pruebas

### Pruebas Manuales
1. Solicitar restablecimiento con un correo existente
2. Verificar que se recibe el correo con el enlace
3. Utilizar el enlace para restablecer la contraseña
4. Verificar que se recibe la notificación de cambio de contraseña
5. Verificar que se puede iniciar sesión con la nueva contraseña

### Posibles Mejoras Futuras
- Implementar límites de intentos para prevenir ataques de fuerza bruta
- Añadir verificación en dos pasos para el proceso de restablecimiento
- Implementar análisis de riesgo basado en patrones de comportamiento del usuario
- Permitir a los administradores configurar la duración de la expiración del token

## Diagrama de Secuencia

```
Usuario                   Frontend                  API                     Servicios                 Base de Datos
   |                         |                       |                          |                          |
   | Solicita restablecer    |                       |                          |                          |
   |------------------------>|                       |                          |                          |
   |                         | request-password-reset|                          |                          |
   |                         |---------------------->|                          |                          |
   |                         |                       | Busca usuario            |                          |
   |                         |                       |------------------------->|                          |
   |                         |                       |                          | Consulta                 |
   |                         |                       |                          |------------------------->|
   |                         |                       |                          | Resultado                |
   |                         |                       |                          |<-------------------------|
   |                         |                       | Genera token             |                          |
   |                         |                       |------------------------->|                          |
   |                         |                       |                          | Guarda token             |
   |                         |                       |                          |------------------------->|
   |                         |                       |                          | Confirmación             |
   |                         |                       |                          |<-------------------------|
   |                         |                       | Envía correo             |                          |
   |                         |                       |------------------------->|                          |
   |                         | Respuesta OK          |                          |                          |
   |                         |<----------------------|                          |                          |
   | Notificación            |                       |                          |                          |
   |<------------------------|                       |                          |                          |
   |                         |                       |                          |                          |
   | Accede al enlace        |                       |                          |                          |
   |------------------------>|                       |                          |                          |
   | Ingresa nueva contraseña|                       |                          |                          |
   |------------------------>|                       |                          |                          |
   |                         | confirm-password-reset|                          |                          |
   |                         |---------------------->|                          |                          |
   |                         |                       | Valida token             |                          |
   |                         |                       |------------------------->|                          |
   |                         |                       |                          | Verifica token           |
   |                         |                       |                          |------------------------->|
   |                         |                       |                          | Resultado                |
   |                         |                       |                          |<-------------------------|
   |                         |                       | Actualiza contraseña     |                          |
   |                         |                       |------------------------->|                          |
   |                         |                       |                          | Guarda cambios           |
   |                         |                       |                          |------------------------->|
   |                         |                       |                          | Confirmación             |
   |                         |                       |                          |<-------------------------|
   |                         |                       | Envía notificación       |                          |
   |                         |                       |------------------------->|                          |
   |                         | Respuesta OK          |                          |                          |
   |                         |<----------------------|                          |                          |
   | Notificación de cambio  |                       |                          |                          |
   |<------------------------|                       |                          |                          |
```
