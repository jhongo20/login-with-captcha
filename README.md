# Sistema de Autenticación y Gestión de Módulos

Este proyecto implementa un sistema completo de autenticación y gestión de módulos para aplicaciones web, siguiendo los principios de Clean Architecture.

## Características principales

### Sistema de Autenticación
- Autenticación basada en JWT
- Soporte para usuarios locales y LDAP/Active Directory
- Gestión de roles y permisos
- Bloqueo de cuentas después de múltiples intentos fallidos
- Protección con CAPTCHA
- Refresh tokens para mantener la sesión

### Gestión de Módulos
- Estructura jerárquica de módulos y submódulos
- Ordenamiento personalizable
- Activación/desactivación de módulos
- Rutas e iconos configurables
- Validaciones para prevenir ciclos y duplicados
- Asociación de módulos a roles para control de acceso

### Gestión de Rutas
- CRUD completo para rutas
- Vinculación de rutas a módulos
- Asignación de rutas a roles
- Control de acceso basado en roles y rutas

## Estructura del proyecto

El proyecto sigue una arquitectura de Clean Architecture con las siguientes capas:

- **Domain**: Entidades, interfaces y contratos
- **Application**: Lógica de aplicación, DTOs, servicios
- **Infrastructure**: Implementaciones de interfaces, servicios técnicos
- **API**: Controladores REST
- **Tests**: Pruebas unitarias e integración

## Documentación

La documentación detallada está disponible en la carpeta `/docs`:

- [Documentación de Módulos](./docs/Modules.md)
- [Guía de Integración de Módulos](./docs/ModulesIntegrationGuide.md)
- [Guía de Pruebas para Módulos](./docs/ModulesTestingGuide.md)
- [Asociación de Módulos a Roles](./docs/ModuleRoleAssociation.md)
- [Guía de Integración para Módulos y Roles](./docs/ModuleRoleIntegrationGuide.md)
- [Guía de Pruebas para Módulos y Roles](./docs/ModuleRoleTestingGuide.md)

## Requisitos

- .NET 8.0
- SQL Server
- Visual Studio 2022 o superior (recomendado)

## Instalación

1. Clonar el repositorio
2. Configurar la cadena de conexión en `appsettings.json`
3. Ejecutar las migraciones: `dotnet ef database update`
4. Ejecutar la aplicación: `dotnet run --project AuthSystem.API`

## Uso

La API estará disponible en `http://localhost:5031`. Puede acceder a la documentación Swagger en `http://localhost:5031/swagger`.

### Autenticación

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "your_password"
}
```

### Gestión de Módulos

```http
GET /api/Modules
Authorization: Bearer {token}
```

## Licencia

Este proyecto está licenciado bajo [MIT License](LICENSE).
