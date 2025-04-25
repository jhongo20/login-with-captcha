# AuthSystem - Sistema de Autenticación y Gestión de Usuarios

## Descripción General

AuthSystem es un sistema completo de autenticación y gestión de usuarios desarrollado con ASP.NET Core, siguiendo los principios de Clean Architecture. El sistema proporciona una API REST para la autenticación de usuarios, gestión de roles y permisos, y administración de organizaciones.

## Características Principales

- **Autenticación**: Soporte para autenticación local y LDAP/Active Directory
- **Autorización**: Sistema de roles y permisos granular
- **Gestión de Usuarios**: CRUD completo de usuarios con estados múltiples
- **Seguridad**: Protección con CAPTCHA, bloqueo de cuentas, contraseñas hasheadas
- **Auditoría**: Registro de actividades y cambios en el sistema

## Estructura del Proyecto

El proyecto sigue una arquitectura limpia con las siguientes capas:

1. **Domain**: Contiene las entidades del negocio, interfaces y contratos
2. **Application**: Contiene la lógica de aplicación, DTOs, servicios y características
3. **Infrastructure**: Implementa las interfaces del dominio y proporciona servicios técnicos
4. **API**: Expone los servicios como API REST
5. **Tests**: Pruebas unitarias e integración

## Documentación

- [Implementación de Estados de Usuario](./EstadosDeUsuario-Implementacion.md)

## Requisitos

- .NET 8.0 o superior
- SQL Server (o compatible con Entity Framework Core)
- SMTP Server (para envío de correos)

## Configuración

La configuración del sistema se realiza a través del archivo `appsettings.json`. Las principales secciones son:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "AuthSystem",
    "Audience": "AuthSystemClient",
    "ExpirationMinutes": 60
  },
  "CaptchaSettings": {
    "SiteKey": "...",
    "SecretKey": "..."
  },
  "AccountLockoutSettings": {
    "MaxFailedAttempts": 5,
    "LockoutDurationMinutes": 30
  }
}
```

## Endpoints Principales

### Autenticación

- `POST /api/Auth/login`: Inicio de sesión básico
- `POST /api/Auth/login-with-captcha`: Inicio de sesión con CAPTCHA
- `POST /api/Auth/login-with-google-recaptcha`: Inicio de sesión con Google reCAPTCHA
- `POST /api/Auth/refresh-token`: Actualización de token JWT
- `POST /api/Auth/logout`: Cierre de sesión

### Usuarios

- `GET /api/Users`: Obtener todos los usuarios
- `GET /api/Users/{id}`: Obtener usuario por ID
- `POST /api/Users`: Crear nuevo usuario
- `PUT /api/Users/{id}`: Actualizar usuario
- `DELETE /api/Users/{id}`: Eliminar usuario

### Roles

- `GET /api/Roles`: Obtener todos los roles
- `GET /api/Roles/{id}`: Obtener rol por ID
- `POST /api/Roles`: Crear nuevo rol
- `PUT /api/Roles/{id}`: Actualizar rol
- `DELETE /api/Roles/{id}`: Eliminar rol

### Permisos

- `GET /api/Permissions`: Obtener todos los permisos
- `GET /api/Permissions/{id}`: Obtener permiso por ID
- `POST /api/Permissions`: Crear nuevo permiso
- `PUT /api/Permissions/{id}`: Actualizar permiso
- `DELETE /api/Permissions/{id}`: Eliminar permiso

## Estados de Usuario

El sistema implementa múltiples estados para los usuarios:

1. **Active (1)**: Usuario activo, puede iniciar sesión
2. **Inactive (2)**: Usuario inactivo, no puede iniciar sesión
3. **Locked (3)**: Usuario bloqueado por intentos fallidos
4. **Suspended (4)**: Usuario suspendido por un administrador
5. **Deleted (5)**: Usuario marcado como eliminado (eliminación lógica)

Para más detalles sobre la implementación de estados de usuario, consulte la [documentación específica](./EstadosDeUsuario-Implementacion.md).
