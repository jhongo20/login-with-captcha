# Sistema de Rutas - Documentación

## Introducción

El sistema de rutas de AuthSystem proporciona una forma de gestionar las rutas (endpoints) de la aplicación, vinculándolas a módulos y roles. Esto permite un control granular sobre qué usuarios pueden acceder a qué funcionalidades de la API según sus roles asignados.

## Entidades

### Route (Ruta)

La entidad principal que representa una ruta en el sistema.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | Guid | Identificador único de la ruta |
| Name | string | Nombre de la ruta (único dentro del módulo) |
| Description | string | Descripción detallada de la ruta |
| Path | string | Ruta URL del endpoint (ej: `/api/users`) |
| HttpMethod | string | Método HTTP (GET, POST, PUT, DELETE, etc.) |
| DisplayOrder | int | Orden de visualización |
| RequiresAuth | bool | Indica si la ruta requiere autenticación |
| IsEnabled | bool | Indica si la ruta está habilitada |
| IsActive | bool | Indica si la ruta está activa en el sistema |
| ModuleId | Guid | ID del módulo al que pertenece la ruta |
| CreatedAt | DateTime | Fecha de creación |
| CreatedBy | string | Usuario que creó la ruta |
| LastModifiedAt | DateTime | Fecha de última modificación |
| LastModifiedBy | string | Usuario que realizó la última modificación |

### RoleRoute (Relación Rol-Ruta)

Entidad que establece la relación entre roles y rutas.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| RoleId | Guid | ID del rol |
| RouteId | Guid | ID de la ruta |
| IsActive | bool | Indica si la relación está activa |
| CreatedAt | DateTime | Fecha de creación |
| CreatedBy | string | Usuario que creó la relación |
| LastModifiedAt | DateTime | Fecha de última modificación |
| LastModifiedBy | string | Usuario que realizó la última modificación |

## Endpoints API

### Obtener todas las rutas

Obtiene un listado de todas las rutas registradas en el sistema.

- **URL**: `/api/Routes`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Array de objetos RouteDto

**Ejemplo de respuesta**:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Listar Usuarios",
    "description": "Obtiene todos los usuarios del sistema",
    "path": "/api/Users",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "moduleName": "Usuarios",
    "createdAt": "2025-04-21T10:00:00Z",
    "createdBy": "admin",
    "lastModifiedAt": "2025-04-21T10:00:00Z",
    "lastModifiedBy": "admin"
  }
]
```

### Obtener una ruta por ID

Obtiene los detalles de una ruta específica por su ID.

- **URL**: `/api/Routes/{id}`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `id`: ID de la ruta (Guid)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Objeto RouteDto
- **Respuesta de error**:
  - **Código**: 404 Not Found
  - **Contenido**: Mensaje de error

**Ejemplo de respuesta exitosa**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Listar Usuarios",
  "description": "Obtiene todos los usuarios del sistema",
  "path": "/api/Users",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "isActive": true,
  "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "moduleName": "Usuarios",
  "createdAt": "2025-04-21T10:00:00Z",
  "createdBy": "admin",
  "lastModifiedAt": "2025-04-21T10:00:00Z",
  "lastModifiedBy": "admin"
}
```

### Obtener rutas habilitadas

Obtiene un listado de todas las rutas habilitadas en el sistema.

- **URL**: `/api/Routes/enabled`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Array de objetos RouteDto

### Obtener rutas por módulo

Obtiene todas las rutas asociadas a un módulo específico.

- **URL**: `/api/Routes/byModule/{moduleId}`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `moduleId`: ID del módulo (Guid)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Array de objetos RouteDto
- **Respuesta de error**:
  - **Código**: 404 Not Found
  - **Contenido**: Mensaje de error

### Obtener rutas por rol

Obtiene todas las rutas asignadas a un rol específico.

- **URL**: `/api/Routes/byRole/{roleId}`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `roleId`: ID del rol (Guid)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Array de objetos RouteDto
- **Respuesta de error**:
  - **Código**: 404 Not Found
  - **Contenido**: Mensaje de error

### Obtener rutas por módulo y rol

Obtiene todas las rutas de un módulo específico a las que tiene acceso un rol determinado.

- **URL**: `/api/Routes/byModuleAndRole/{moduleId}/{roleId}`
- **Método**: `GET`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `moduleId`: ID del módulo (Guid)
  - `roleId`: ID del rol (Guid)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Array de objetos RouteDto
- **Respuesta de error**:
  - **Código**: 404 Not Found
  - **Contenido**: Mensaje de error

**Ejemplo de respuesta exitosa**:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Listar Usuarios",
    "description": "Obtiene todos los usuarios del sistema",
    "path": "/api/Users",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "moduleName": "Usuarios",
    "createdAt": "2025-04-21T10:00:00Z",
    "createdBy": "admin",
    "lastModifiedAt": "2025-04-21T10:00:00Z",
    "lastModifiedBy": "admin"
  }
]
```

### Crear una nueva ruta

Crea una nueva ruta en el sistema.

- **URL**: `/api/Routes`
- **Método**: `POST`
- **Autenticación**: Requerida (Rol: Admin)
- **Cuerpo de la solicitud**: Objeto CreateRouteRequest
- **Respuesta exitosa**:
  - **Código**: 201 Created
  - **Contenido**: Objeto RouteDto
- **Respuesta de error**:
  - **Código**: 400 Bad Request
  - **Contenido**: Mensajes de validación

**Ejemplo de solicitud**:
```json
{
  "name": "Listar Usuarios",
  "description": "Obtiene todos los usuarios del sistema",
  "path": "/api/Users",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Actualizar una ruta existente

Actualiza los datos de una ruta existente.

- **URL**: `/api/Routes/{id}`
- **Método**: `PUT`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `id`: ID de la ruta a actualizar (Guid)
- **Cuerpo de la solicitud**: Objeto UpdateRouteRequest
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Objeto RouteDto actualizado
- **Respuesta de error**:
  - **Código**: 400 Bad Request o 404 Not Found
  - **Contenido**: Mensajes de validación o error

**Ejemplo de solicitud**:
```json
{
  "name": "Listar Usuarios Actualizados",
  "description": "Obtiene todos los usuarios activos del sistema",
  "path": "/api/Users",
  "httpMethod": "GET",
  "displayOrder": 2,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Eliminar una ruta

Elimina (desactiva) una ruta existente.

- **URL**: `/api/Routes/{id}`
- **Método**: `DELETE`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `id`: ID de la ruta a eliminar (Guid)
- **Respuesta exitosa**:
  - **Código**: 204 No Content
- **Respuesta de error**:
  - **Código**: 404 Not Found
  - **Contenido**: Mensaje de error

### Asignar una ruta a un rol

Asigna una ruta específica a un rol.

- **URL**: `/api/Routes/assign`
- **Método**: `POST`
- **Autenticación**: Requerida (Rol: Admin)
- **Cuerpo de la solicitud**: Objeto AssignRouteToRoleRequest
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Mensaje de confirmación
- **Respuesta de error**:
  - **Código**: 400 Bad Request o 404 Not Found
  - **Contenido**: Mensajes de validación o error

**Ejemplo de solicitud**:
```json
{
  "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Revocar una ruta de un rol

Revoca el acceso de un rol a una ruta específica.

- **URL**: `/api/Routes/revoke/{roleId}/{routeId}`
- **Método**: `DELETE`
- **Autenticación**: Requerida (Rol: Admin)
- **Parámetros de URL**:
  - `roleId`: ID del rol (Guid)
  - `routeId`: ID de la ruta (Guid)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Mensaje de confirmación
- **Respuesta de error**:
  - **Código**: 400 Bad Request o 404 Not Found
  - **Contenido**: Mensajes de validación o error

## Modelos de Datos

### RouteDto

Modelo de transferencia de datos para rutas.

```csharp
public class RouteDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }
    public string HttpMethod { get; set; }
    public int DisplayOrder { get; set; }
    public bool RequiresAuth { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsActive { get; set; }
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public string LastModifiedBy { get; set; }
}
```

### CreateRouteRequest

Modelo para crear una nueva ruta.

```csharp
public class CreateRouteRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Path { get; set; }
    
    [Required]
    [StringLength(10)]
    public string HttpMethod { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool RequiresAuth { get; set; } = true;
    
    public bool IsEnabled { get; set; } = true;
    
    [Required]
    public Guid ModuleId { get; set; }
}
```

### UpdateRouteRequest

Modelo para actualizar una ruta existente.

```csharp
public class UpdateRouteRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Path { get; set; }
    
    [Required]
    [StringLength(10)]
    public string HttpMethod { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool RequiresAuth { get; set; }
    
    public bool IsEnabled { get; set; }
    
    [Required]
    public Guid ModuleId { get; set; }
}
```

### AssignRouteToRoleRequest

Modelo para asignar una ruta a un rol.

```csharp
public class AssignRouteToRoleRequest
{
    [Required]
    public Guid RouteId { get; set; }
    
    [Required]
    public Guid RoleId { get; set; }
}
```

## Ejemplos de Uso

### Autenticación

Antes de utilizar cualquier endpoint, es necesario autenticarse y obtener un token JWT.

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123$"
}
```

Respuesta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresIn": 3600
}
```

Luego, incluye el token en el encabezado de autorización:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Crear una nueva ruta

```http
POST /api/Routes
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "name": "Obtener Perfil",
  "description": "Obtiene el perfil del usuario actual",
  "path": "/api/Users/profile",
  "httpMethod": "GET",
  "displayOrder": 5,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Asignar una ruta a un rol

```http
POST /api/Routes/assign
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## Consideraciones Importantes

1. **Seguridad**: Todos los endpoints requieren autenticación y el rol de Administrador.
2. **Validaciones**:
   - El nombre de la ruta debe ser único dentro de un mismo módulo.
   - La combinación de path y método HTTP debe ser única en todo el sistema.
   - Para asignar una ruta a un rol, el rol debe tener acceso al módulo que contiene la ruta.
3. **Eliminación**: Las rutas no se eliminan físicamente de la base de datos, solo se marcan como inactivas (soft delete).
4. **Auditoría**: Todas las operaciones registran el usuario que las realizó y la fecha/hora.

## Integración con el Sistema de Módulos

El sistema de rutas está estrechamente integrado con el sistema de módulos. Cada ruta pertenece a un módulo específico, y para que un rol pueda acceder a una ruta, primero debe tener acceso al módulo correspondiente.

## Integración con el Sistema de Roles

El sistema de rutas se integra con el sistema de roles a través de la entidad RoleRoute, que establece qué roles tienen acceso a qué rutas. Esta relación permite un control granular de los permisos de acceso a nivel de endpoint.
