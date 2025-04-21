# Documentación del Módulo de Módulos

## Descripción General

El módulo de Módulos permite la gestión de los módulos del sistema, que representan las diferentes secciones o funcionalidades de la aplicación. Cada módulo puede tener submódulos, formando una estructura jerárquica.

## Entidad Module

La entidad `Module` tiene los siguientes campos:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | Guid | Identificador único del módulo |
| Name | string | Nombre del módulo (único) |
| Description | string | Descripción del módulo |
| Route | string | Ruta de navegación del módulo |
| Icon | string | Icono asociado al módulo |
| DisplayOrder | int | Orden de visualización |
| ParentId | Guid? | ID del módulo padre (null si es un módulo raíz) |
| IsEnabled | bool | Indica si el módulo está habilitado |
| CreatedAt | DateTime | Fecha de creación |
| CreatedBy | string | Usuario que creó el módulo |
| UpdatedAt | DateTime | Fecha de última actualización |
| UpdatedBy | string | Usuario que realizó la última actualización |

## API Endpoints

### Obtener todos los módulos

```
GET /api/Modules
```

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Administración",
    "description": "Módulo de administración del sistema",
    "route": "/admin",
    "icon": "fa-cogs",
    "displayOrder": 1,
    "parentId": null,
    "isEnabled": true,
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-21T15:00:00Z",
    "updatedBy": "admin"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules` en la sección GET
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Verás la lista de todos los módulos en el sistema

### Obtener un módulo por ID

```
GET /api/Modules/{id}
```

**Parámetros**
- `id` (Guid): ID del módulo a obtener

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Administración",
  "description": "Módulo de administración del sistema",
  "route": "/admin",
  "icon": "fa-cogs",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true,
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "admin",
  "updatedAt": "2025-04-21T15:00:00Z",
  "updatedBy": "admin"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules/{id}` en la sección GET
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo que deseas obtener (por ejemplo: `7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3`)
4. Haz clic en "Execute"
5. Verás los detalles del módulo específico

### Crear un módulo

```
POST /api/Modules
```

**Cuerpo de la solicitud**
```json
{
  "name": "Configuración",
  "description": "Módulo para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cog",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true
}
```

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Configuración",
  "description": "Módulo para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cog",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true,
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "admin",
  "updatedAt": "2025-04-21T15:00:00Z",
  "updatedBy": "admin"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules` en la sección POST
2. Haz clic en "Try it out"
3. Pega el siguiente JSON en el cuadro de texto del cuerpo de la solicitud:
```json
{
  "name": "Configuración",
  "description": "Módulo para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cog",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true
}
```
4. Haz clic en "Execute"
5. Verás los detalles del módulo creado, incluyendo su ID generado

### Actualizar un módulo

```
PUT /api/Modules/{id}
```

**Parámetros**
- `id` (Guid): ID del módulo a actualizar

**Cuerpo de la solicitud**
```json
{
  "name": "Configuración del Sistema",
  "description": "Módulo actualizado para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cogs",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true
}
```

**Respuesta**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Configuración del Sistema",
  "description": "Módulo actualizado para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cogs",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true,
  "createdAt": "2025-04-21T15:00:00Z",
  "createdBy": "admin",
  "updatedAt": "2025-04-21T15:30:00Z",
  "updatedBy": "admin"
}
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules/{id}` en la sección PUT
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo que deseas actualizar (por ejemplo: `7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3`)
4. Pega el siguiente JSON en el cuadro de texto del cuerpo de la solicitud:
```json
{
  "name": "Configuración del Sistema",
  "description": "Módulo actualizado para configuraciones del sistema",
  "route": "/config",
  "icon": "fa-cogs",
  "displayOrder": 4,
  "parentId": null,
  "isEnabled": true
}
```
5. Haz clic en "Execute"
6. Verás los detalles del módulo actualizado

### Eliminar un módulo

```
DELETE /api/Modules/{id}
```

**Parámetros**
- `id` (Guid): ID del módulo a eliminar

**Respuesta**
- Código 204 No Content (éxito)
- Código 400 Bad Request (si el módulo tiene submódulos)
- Código 404 Not Found (si el módulo no existe)

**Ejemplo en Swagger**
1. Navega a `/api/Modules/{id}` en la sección DELETE
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo que deseas eliminar (por ejemplo: `7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3`)
4. Haz clic en "Execute"
5. Si la operación es exitosa, verás un código de estado 204 (No Content)
6. Si el módulo tiene submódulos, verás un código de estado 400 (Bad Request) con un mensaje de error
7. Si el módulo no existe, verás un código de estado 404 (Not Found)

### Obtener módulos habilitados

```
GET /api/Modules/enabled
```

**Respuesta**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Administración",
    "description": "Módulo de administración del sistema",
    "route": "/admin",
    "icon": "fa-cogs",
    "displayOrder": 1,
    "parentId": null,
    "isEnabled": true,
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-21T15:00:00Z",
    "updatedBy": "admin"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules/enabled` en la sección GET
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Verás la lista de todos los módulos habilitados en el sistema

### Obtener submódulos de un módulo

```
GET /api/Modules/{id}/children
```

**Parámetros**
- `id` (Guid): ID del módulo padre

**Respuesta**
```json
[
  {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "name": "Usuarios",
    "description": "Gestión de usuarios",
    "route": "/admin/users",
    "icon": "fa-users",
    "displayOrder": 1,
    "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "isEnabled": true,
    "createdAt": "2025-04-21T15:00:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-21T15:00:00Z",
    "updatedBy": "admin"
  }
]
```

**Ejemplo en Swagger**
1. Navega a `/api/Modules/{id}/children` en la sección GET
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo padre (por ejemplo: `3fa85f64-5717-4562-b3fc-2c963f66afa6`)
4. Haz clic en "Execute"
5. Verás la lista de todos los submódulos del módulo padre especificado

## Validaciones

El sistema implementa las siguientes validaciones:

1. **Nombre único**: No se permiten módulos con el mismo nombre.
2. **Prevención de ciclos**: No se permite crear ciclos en la jerarquía de módulos (un módulo no puede ser su propio ancestro).
3. **Eliminación segura**: No se pueden eliminar módulos que tengan submódulos.

## Ejemplos de uso

### Crear un módulo principal

```http
POST /api/Modules
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Administración",
  "description": "Módulo de administración del sistema",
  "route": "/admin",
  "icon": "fa-cogs",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true
}
```

### Crear un submódulo

```http
POST /api/Modules
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Usuarios",
  "description": "Gestión de usuarios",
  "route": "/admin/users",
  "icon": "fa-users",
  "displayOrder": 1,
  "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isEnabled": true
}
```

### Obtener la jerarquía de módulos

Para obtener una jerarquía completa de módulos, se puede utilizar el endpoint `/api/Modules` y procesar la respuesta en el cliente para construir la estructura jerárquica basada en los campos `id` y `parentId`.

## Consideraciones de seguridad

- Todos los endpoints requieren autenticación mediante JWT.
- Se debe implementar autorización basada en roles para restringir quién puede crear, modificar o eliminar módulos.
