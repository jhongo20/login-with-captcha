# Asociación de Módulos a Roles

## Descripción General

La funcionalidad de asociación de módulos a roles permite asignar permisos a nivel de módulo para los diferentes roles del sistema. Esto facilita la gestión de accesos y proporciona una capa adicional de seguridad al controlar qué módulos son accesibles para cada rol.

## Estructura de Datos

### Entidades Principales

1. **PermissionModule**
   - Entidad de relación entre permisos y módulos
   - Campos:
     - `Id` (Guid): Identificador único
     - `PermissionId` (Guid): ID del permiso asociado
     - `ModuleId` (Guid): ID del módulo asociado
     - `CreatedAt` (DateTime): Fecha de creación
     - `CreatedBy` (string): Usuario que creó la relación
     - `LastModifiedAt` (DateTime?): Fecha de última modificación
     - `LastModifiedBy` (string): Usuario que realizó la última modificación

### Relaciones

- Un **Módulo** puede estar asociado a múltiples **Permisos**
- Un **Permiso** puede estar asociado a múltiples **Módulos**
- Un **Rol** tiene acceso a un **Módulo** cuando tiene asignado el permiso `Modules.View` para ese módulo

## API Endpoints

### Obtener Módulos por Rol

```
GET /api/Modules/byRole/{roleId}
```

Devuelve todos los módulos a los que tiene acceso un rol específico.

**Parámetros:**
- `roleId` (Guid): ID del rol

**Respuesta:**
```json
[
  {
    "id": "guid",
    "name": "string",
    "description": "string",
    "route": "string",
    "icon": "string",
    "displayOrder": 0,
    "parentId": "guid",
    "isEnabled": true,
    "createdAt": "2025-04-21T00:00:00Z",
    "createdBy": "string",
    "updatedAt": "2025-04-21T00:00:00Z",
    "updatedBy": "string",
    "children": []
  }
]
```

### Asignar Módulo a Rol

```
POST /api/Modules/assign-to-role
```

Asigna un módulo a un rol, otorgándole acceso.

**Cuerpo de la Solicitud:**
```json
{
  "moduleId": "guid",
  "roleId": "guid"
}
```

**Respuesta:**
```json
{
  "message": "Módulo asignado correctamente al rol"
}
```

### Revocar Módulo de Rol

```
DELETE /api/Modules/revoke-from-role/{roleId}/{moduleId}
```

Revoca el acceso de un rol a un módulo específico.

**Parámetros:**
- `roleId` (Guid): ID del rol
- `moduleId` (Guid): ID del módulo

**Respuesta:**
```json
{
  "message": "Acceso al módulo revocado correctamente del rol"
}
```

### Asignar Ruta a Módulo

```
POST /api/Routes/assign-to-module
```

Asigna una ruta a un módulo específico.

**Cuerpo de la Solicitud:**
```json
{
  "routeId": "guid",
  "moduleId": "guid"
}
```

**Respuesta:**
```json
{
  "message": "Ruta asignada correctamente al módulo"
}
```

### Obtener Rutas por Módulo y Rol

```
GET /api/Routes/byModuleAndRole/{moduleId}/{roleId}
```

Devuelve todas las rutas de un módulo específico a las que tiene acceso un rol.

**Parámetros:**
- `moduleId` (Guid): ID del módulo
- `roleId` (Guid): ID del rol

**Respuesta:**
```json
[
  {
    "id": "guid",
    "name": "string",
    "description": "string",
    "path": "string",
    "httpMethod": "string",
    "displayOrder": 0,
    "requiresAuth": true,
    "isEnabled": true,
    "moduleId": "guid",
    "moduleName": "string"
  }
]
```

## Implementación

### Repositorio de Módulos

El repositorio de módulos (`ModuleRepository`) implementa los siguientes métodos:

1. **GetModulesByRoleAsync**: Obtiene todos los módulos asignados a un rol específico
2. **RoleHasModuleAccessAsync**: Verifica si un rol tiene acceso a un módulo específico
3. **AssignModuleToRoleAsync**: Asigna un módulo a un rol
4. **RevokeModuleFromRoleAsync**: Revoca el acceso de un rol a un módulo

### Controlador de Módulos

El controlador de módulos (`ModulesController`) expone los endpoints necesarios para gestionar la asociación de módulos a roles.

## Base de Datos

### Tabla PermissionModules

```sql
CREATE TABLE [dbo].[PermissionModules](
    [Id] [uniqueidentifier] NOT NULL,
    [PermissionId] [uniqueidentifier] NOT NULL,
    [ModuleId] [uniqueidentifier] NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] [nvarchar](50) NOT NULL,
    [LastModifiedAt] [datetime2](7) NULL,
    [LastModifiedBy] [nvarchar](50) NULL,
    CONSTRAINT [PK_PermissionModules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_PermissionModules_PermissionId_ModuleId] UNIQUE NONCLUSTERED 
    (
        [PermissionId] ASC,
        [ModuleId] ASC
    ),
    CONSTRAINT [FK_PermissionModules_Permissions_PermissionId] FOREIGN KEY([PermissionId])
        REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PermissionModules_Modules_ModuleId] FOREIGN KEY([ModuleId])
        REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
)
```

### Permiso Modules.View

Este permiso es necesario para la funcionalidad de asignación de módulos a roles. Se crea automáticamente durante la inicialización de la base de datos.

## Inicialización de la Base de Datos

La base de datos se inicializa automáticamente durante el arranque de la aplicación utilizando la clase `DatabaseInitializer`. Esta clase ejecuta las migraciones personalizadas que crean la tabla `PermissionModules` y agregan el permiso `Modules.View`.

## Scripts de Actualización

Para actualizar manualmente la base de datos, se proporciona el script `ActualizarBaseDeDatos.sql` que:

1. Crea la tabla `PermissionModules` si no existe
2. Agrega el permiso `Modules.View` si no existe
3. Asigna todos los módulos existentes al rol Admin

## Seguridad

- Solo los usuarios con el rol `Admin` pueden gestionar la asociación de módulos a roles
- La asignación de módulos a roles se audita mediante los campos de creación y modificación
- Las operaciones de asignación y revocación están protegidas por autorización basada en roles

## Consideraciones de Rendimiento

- Se han creado índices para mejorar el rendimiento de las consultas relacionadas con la asociación de módulos a roles
- La tabla `PermissionModules` utiliza claves foráneas con eliminación en cascada para mantener la integridad referencial
