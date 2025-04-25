# Características de Seguridad en AuthSystem

## Descripción General

Este documento describe las características de seguridad implementadas en el sistema AuthSystem, incluyendo autenticación, autorización, bloqueo de cuentas y notificaciones de seguridad.

## Autenticación

### JWT (JSON Web Tokens)

El sistema utiliza tokens JWT para la autenticación:

- **Duración del token**: 60 minutos (configurable en appsettings.json)
- **Refresh token**: Válido por 7 días (configurable)
- **Almacenamiento**: Los tokens se almacenan en la base de datos para control de sesiones

### Protección con CAPTCHA

Para prevenir ataques automatizados, el sistema implementa protección con CAPTCHA:

- **Google reCAPTCHA**: Integración con reCAPTCHA v2
- **CAPTCHA interno**: Implementación propia como alternativa
- **Activación**: Se activa después de intentos fallidos de inicio de sesión

## Bloqueo de Cuentas

El sistema implementa un mecanismo de bloqueo de cuentas para prevenir ataques de fuerza bruta:

### Configuración

- **Número máximo de intentos fallidos**: 5 intentos (configurable en appsettings.json)
- **Duración del bloqueo**: 15 minutos (configurable en appsettings.json)
- **Activación**: Habilitado por defecto para todos los usuarios

### Funcionamiento

1. Cada intento fallido de inicio de sesión incrementa un contador para el usuario
2. Al alcanzar el límite de intentos fallidos (5 por defecto), la cuenta se bloquea automáticamente
3. Durante el bloqueo, el usuario no puede iniciar sesión aunque proporcione credenciales correctas
4. Después del período de bloqueo, la cuenta se desbloquea automáticamente
5. Un inicio de sesión exitoso reinicia el contador de intentos fallidos

### Notificaciones de Bloqueo

Cuando una cuenta es bloqueada, se envía automáticamente un correo electrónico al usuario con la siguiente información:

- Fecha y hora del bloqueo
- Duración del bloqueo
- Fecha y hora de desbloqueo automático
- Dirección IP desde donde se realizaron los intentos
- Dispositivo y navegador utilizados
- Instrucciones sobre qué hacer si el usuario no realizó los intentos
- Opciones para contactar a soporte

### Desbloqueo Manual

Los administradores pueden desbloquear manualmente una cuenta a través de:

- API endpoint: `POST /api/Users/{id}/unlock`
- Interfaz de administración

## Contraseñas

### Requisitos de Contraseñas

El sistema implementa los siguientes requisitos para contraseñas seguras:

- **Longitud mínima**: 8 caracteres
- **Complejidad**: Debe incluir mayúsculas, minúsculas, números y caracteres especiales
- **Almacenamiento**: Hasheadas con BCrypt

### Restablecimiento de Contraseña

El sistema permite a los usuarios restablecer sus contraseñas:

1. El usuario solicita un restablecimiento proporcionando su correo electrónico
2. Se genera un token de restablecimiento con validez de 1 hora
3. Se envía un correo electrónico con un enlace para restablecer la contraseña
4. Al cambiar la contraseña, se envía una notificación al usuario

## Notificaciones de Seguridad

El sistema envía notificaciones por correo electrónico para eventos relacionados con la seguridad:

### Tipos de Notificaciones

1. **Inicio de Sesión**: Notifica al usuario cuando se inicia sesión en su cuenta
2. **Bloqueo de Cuenta**: Notifica al usuario cuando su cuenta ha sido bloqueada
3. **Cambio de Contraseña**: Notifica al usuario cuando su contraseña ha sido cambiada
4. **Actividad Inusual**: Alerta al usuario sobre actividad sospechosa
5. **Cuenta por Expirar**: Notifica cuando una cuenta está por expirar

### Información Incluida

Las notificaciones incluyen información detallada para ayudar al usuario a identificar actividad no autorizada:

- Fecha y hora del evento
- Dirección IP
- Ubicación geográfica aproximada
- Dispositivo y navegador utilizados
- Instrucciones sobre qué hacer en caso de actividad no autorizada

## Auditoría y Registro

El sistema mantiene registros detallados de eventos de seguridad:

- Intentos de inicio de sesión (exitosos y fallidos)
- Bloqueos y desbloqueos de cuentas
- Cambios de contraseña
- Actividad administrativa

## Configuración de Seguridad

Las características de seguridad son configurables a través del archivo `appsettings.json`:

```json
"Security": {
  "MaxFailedLoginAttempts": 5,
  "LockoutDurationMinutes": 15,
  "PasswordRequireDigit": true,
  "PasswordRequireLowercase": true,
  "PasswordRequireUppercase": true,
  "PasswordRequireNonAlphanumeric": true,
  "PasswordMinLength": 8
}
```

## Mejores Prácticas Implementadas

1. **Defensa en profundidad**: Múltiples capas de seguridad
2. **Principio de mínimo privilegio**: Usuarios con acceso solo a lo que necesitan
3. **Validación de entradas**: Todas las entradas son validadas
4. **Protección contra ataques comunes**: XSS, CSRF, inyección SQL
5. **Notificaciones de seguridad**: Alertas en tiempo real para actividades sospechosas
