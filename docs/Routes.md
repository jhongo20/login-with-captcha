# Sistema de Rutas

## Descripción

El sistema de rutas permite gestionar las URLs y endpoints de la aplicación, vinculándolos a módulos y roles para un control de acceso granular. Las rutas representan los endpoints de la API o las páginas de la aplicación, y pueden ser asignadas a roles específicos para controlar quién tiene acceso a cada funcionalidad.

## Entidades

### Route

Representa una ruta o endpoint en el sistema.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | Guid | Identificador único de la ruta |
| Name | string | Nombre descriptivo de la ruta (único dentro del módulo) |
| Description | string | Descripción detallada de la ruta |
| Path | string | URL o path de la ruta (debe ser único con el método HTTP) |
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

### RoleRoute

Representa la relación entre un rol y una ruta, permitiendo asignar acceso a rutas específicas a roles determinados.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | Guid | Identificador único de la relación |
| RoleId | Guid | ID del rol |
| RouteId | Guid | ID de la ruta |
| IsActive | bool | Indica si la relación está activa |
| CreatedAt | DateTime | Fecha de creación |
| CreatedBy | string | Usuario que creó la relación |
| LastModifiedAt | DateTime | Fecha de última modificación |
| LastModifiedBy | string | Usuario que realizó la última modificación |

## API Endpoints

### Obtener todas las rutas

```
GET /api/Routes
```

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Obtener estadísticas",
    "description": "Obtiene las estadísticas del dashboard",
    "path": "/api/dashboard/stats",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
    "moduleName": "Dashboard",
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "System",
    "lastModifiedAt": "2025-04-21T15:00:00Z",
    "lastModifiedBy": "System"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes` en la sección GET
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Verás la lista de todas las rutas en el sistema

### Obtener una ruta por ID

```
GET /api/Routes/{id}
```

**Parámetros**
- `id` (Guid): ID de la ruta a obtener

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Obtener estadísticas",
  "description": "Obtiene las estadísticas del dashboard",
  "path": "/api/dashboard/stats",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "isActive": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
  "moduleName": "Dashboard",
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "System",
  "lastModifiedAt": "2025-04-21T15:00:00Z",
  "lastModifiedBy": "System"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/{id}` en la sección GET
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que deseas obtener
4. Haz clic en "Execute"
5. Verás los detalles de la ruta específica

### Obtener rutas habilitadas

```
GET /api/Routes/enabled
```

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Obtener estadísticas",
    "description": "Obtiene las estadísticas del dashboard",
    "path": "/api/dashboard/stats",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
    "moduleName": "Dashboard",
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "System",
    "lastModifiedAt": "2025-04-21T15:00:00Z",
    "lastModifiedBy": "System"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/enabled` en la sección GET
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Verás la lista de todas las rutas habilitadas en el sistema

### Obtener rutas por módulo

```
GET /api/Routes/byModule/{moduleId}
```

**Parámetros**
- `moduleId` (Guid): ID del módulo

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Obtener estadísticas",
    "description": "Obtiene las estadísticas del dashboard",
    "path": "/api/dashboard/stats",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
    "moduleName": "Dashboard",
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "System",
    "lastModifiedAt": "2025-04-21T15:00:00Z",
    "lastModifiedBy": "System"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/byModule/{moduleId}` en la sección GET
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo
4. Haz clic en "Execute"
5. Verás la lista de todas las rutas asociadas al módulo especificado

### Obtener rutas por rol

```
GET /api/Routes/byRole/{roleId}
```

**Parámetros**
- `roleId` (Guid): ID del rol

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Obtener estadísticas",
    "description": "Obtiene las estadísticas del dashboard",
    "path": "/api/dashboard/stats",
    "httpMethod": "GET",
    "displayOrder": 1,
    "requiresAuth": true,
    "isEnabled": true,
    "isActive": true,
    "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
    "moduleName": "Dashboard",
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "System",
    "lastModifiedAt": "2025-04-21T15:00:00Z",
    "lastModifiedBy": "System"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/byRole/{roleId}` en la sección GET
2. Haz clic en "Try it out"
3. Ingresa el ID del rol
4. Haz clic en "Execute"
5. Verás la lista de todas las rutas asignadas al rol especificado

### Crear una ruta

```
POST /api/Routes
```

**Cuerpo de la solicitud**
```json
{
  "name": "Obtener perfil de usuario",
  "description": "Obtiene el perfil del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Obtener perfil de usuario",
  "description": "Obtiene el perfil del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "isActive": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
  "moduleName": "Dashboard",
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "admin",
  "lastModifiedAt": "2025-04-21T15:00:00Z",
  "lastModifiedBy": "admin"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes` en la sección POST
2. Haz clic en "Try it out"
3. Pega el siguiente JSON en el cuadro de texto del cuerpo de la solicitud:
```json
{
  "name": "Obtener perfil de usuario",
  "description": "Obtiene el perfil del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```
4. Haz clic en "Execute"
5. Verás los detalles de la ruta creada, incluyendo su ID generado

### Actualizar una ruta

```
PUT /api/Routes/{id}
```

**Parámetros**
- `id` (Guid): ID de la ruta a actualizar

**Cuerpo de la solicitud**
```json
{
  "name": "Obtener perfil de usuario actualizado",
  "description": "Obtiene el perfil actualizado del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 2,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Obtener perfil de usuario actualizado",
  "description": "Obtiene el perfil actualizado del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 2,
  "requiresAuth": true,
  "isEnabled": true,
  "isActive": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3",
  "moduleName": "Dashboard",
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "admin",
  "lastModifiedAt": "2025-04-21T15:30:00Z",
  "lastModifiedBy": "admin"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/{id}` en la sección PUT
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que deseas actualizar
4. Pega el siguiente JSON en el cuadro de texto del cuerpo de la solicitud:
```json
{
  "name": "Obtener perfil de usuario actualizado",
  "description": "Obtiene el perfil actualizado del usuario actual",
  "path": "/api/users/profile",
  "httpMethod": "GET",
  "displayOrder": 2,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```
5. Haz clic en "Execute"
6. Verás los detalles de la ruta actualizada

### Eliminar una ruta

```
DELETE /api/Routes/{id}
```

**Parámetros**
- `id` (Guid): ID de la ruta a eliminar

**Respuesta**
- Código 204 No Content (éxito)
- Código 404 Not Found (si la ruta no existe)

**Ejemplo en Swagger**
1. Navega a `/api/Routes/{id}` en la sección DELETE
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que deseas eliminar
4. Haz clic en "Execute"
5. Si la operación es exitosa, verás un código de estado 204 (No Content)

### Asignar una ruta a un rol

```
POST /api/Routes/assign
```

**Cuerpo de la solicitud**
```json
{
  "roleId": "d7e350e8-5fb7-4517-b8da-6f602d66a3a9",
  "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Respuesta**
```json
{
  "message": "Ruta asignada correctamente al rol"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/assign` en la sección POST
2. Haz clic en "Try it out"
3. Pega el siguiente JSON en el cuadro de texto del cuerpo de la solicitud:
```json
{
  "roleId": "d7e350e8-5fb7-4517-b8da-6f602d66a3a9",
  "routeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
4. Haz clic en "Execute"
5. Verás un mensaje de confirmación si la operación fue exitosa

### Revocar una ruta de un rol

```
DELETE /api/Routes/revoke/{roleId}/{routeId}
```

**Parámetros**
- `roleId` (Guid): ID del rol
- `routeId` (Guid): ID de la ruta

**Respuesta**
```json
{
  "message": "Acceso a la ruta revocado correctamente del rol"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Routes/revoke/{roleId}/{routeId}` en la sección DELETE
2. Haz clic en "Try it out"
3. Ingresa el ID del rol y el ID de la ruta
4. Haz clic en "Execute"
5. Verás un mensaje de confirmación si la operación fue exitosa

## Validaciones

El sistema implementa las siguientes validaciones:

1. **Nombre único dentro del módulo**: No se pueden crear dos rutas con el mismo nombre dentro del mismo módulo.
2. **Path y método HTTP únicos**: No se pueden crear dos rutas con el mismo path y método HTTP.
3. **Verificación de existencia del módulo**: Al crear o actualizar una ruta, se verifica que el módulo exista.
4. **Verificación de existencia del rol**: Al asignar una ruta a un rol, se verifica que el rol exista.
5. **Verificación de acceso al módulo**: Para asignar una ruta a un rol, el rol debe tener acceso al módulo al que pertenece la ruta.

## Permisos

El sistema de rutas utiliza los siguientes permisos:

- `Routes.View`: Permite ver las rutas
- `Routes.Create`: Permite crear nuevas rutas
- `Routes.Edit`: Permite editar rutas existentes y asignar/revocar rutas a roles
- `Routes.Delete`: Permite eliminar rutas

Estos permisos deben ser asignados a los roles que necesiten gestionar las rutas del sistema.

## Integración con el sistema de módulos

Las rutas están vinculadas a módulos, lo que permite una organización jerárquica de la aplicación:

1. Cada ruta pertenece a un módulo específico.
2. Para asignar una ruta a un rol, el rol debe tener acceso al módulo correspondiente.
3. La revocación de acceso a un módulo implica la pérdida de acceso a todas sus rutas.

Esta integración garantiza un control de acceso coherente y granular en toda la aplicación.
