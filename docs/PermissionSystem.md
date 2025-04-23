# Sistema de Permisos en AuthSystem

## Introducción

El sistema de permisos de AuthSystem proporciona un control de acceso granular a las diferentes funcionalidades de la aplicación. Este documento describe la arquitectura del sistema de permisos, los tipos de permisos disponibles y cómo se utilizan para controlar el acceso a módulos y rutas.

## Arquitectura del Sistema de Permisos

El sistema de permisos se basa en tres entidades principales:

1. **Permisos**: Definen acciones específicas que pueden realizarse en el sistema.
2. **Roles**: Agrupan permisos para asignarlos a usuarios.
3. **Usuarios**: Tienen roles asignados que determinan sus permisos.

Además, el sistema utiliza las siguientes relaciones:

1. **RolePermissions**: Asocia permisos a roles.
2. **PermissionModules**: Asocia permisos a módulos.
3. **PermissionRoutes**: Asocia permisos a rutas.
4. **RoleRoutes**: Asocia roles directamente a rutas.

## Tipos de Permisos

Los permisos en AuthSystem siguen una nomenclatura estandarizada basada en el formato `[Entidad].[Acción]`. Por ejemplo:

### Permisos de Módulos
- `Modules.View`: Permite ver un módulo en el menú o interfaz.
- `Modules.Create`: Permite crear nuevos módulos.
- `Modules.Edit`: Permite editar módulos existentes.
- `Modules.Delete`: Permite eliminar módulos.

### Permisos de Rutas
- `Routes.View`: Permite ver o acceder a una ruta.
- `Routes.Create`: Permite crear nuevas rutas.
- `Routes.Edit`: Permite editar rutas existentes.
- `Routes.Delete`: Permite eliminar rutas.
- `Routes.AssignToModule`: Permite asignar rutas a módulos.

### Permisos de Usuarios
- `Users.View`: Permite ver usuarios.
- `Users.Create`: Permite crear nuevos usuarios.
- `Users.Edit`: Permite editar usuarios existentes.
- `Users.Delete`: Permite eliminar usuarios.

## Relación entre Módulos y Rutas

En AuthSystem, los módulos son contenedores lógicos que agrupan rutas relacionadas. Cada ruta debe pertenecer a un módulo, incluso si es el módulo especial "Sin Asignar".

### Módulo "Sin Asignar"

El módulo "Sin Asignar" es un módulo especial que contiene todas las rutas que no están asignadas a ningún módulo específico. Este módulo se crea automáticamente durante la inicialización del sistema y tiene las siguientes características:

- **Nombre**: Sin Asignar
- **Descripción**: Módulo para rutas sin asignación
- **Ruta**: /unassigned
- **Icono**: fa-question-circle
- **Orden de visualización**: 9999 (aparece al final de la lista)

## Control de Acceso

El control de acceso en AuthSystem se realiza en varios niveles:

1. **Nivel de Módulo**: Un usuario puede ver un módulo si tiene el permiso `Modules.View` para ese módulo.
2. **Nivel de Ruta**: Un usuario puede acceder a una ruta si:
   - Tiene el permiso `Routes.View` para esa ruta, o
   - Su rol tiene acceso directo a esa ruta a través de la tabla `RoleRoutes`.

## Tablas de Relación

### PermissionModules

La tabla `PermissionModules` define qué permisos son necesarios para acceder a cada módulo.

Estructura:
- `Id`: Identificador único
- `PermissionId`: ID del permiso
- `ModuleId`: ID del módulo
- `CreatedAt`: Fecha de creación
- `CreatedBy`: Usuario que creó el registro
- `LastModifiedAt`: Fecha de última modificación
- `LastModifiedBy`: Usuario que realizó la última modificación

### PermissionRoutes

La tabla `PermissionRoutes` define qué permisos son necesarios para acceder a cada ruta.

Estructura:
- `Id`: Identificador único
- `PermissionId`: ID del permiso
- `RouteId`: ID de la ruta
- `CreatedAt`: Fecha de creación
- `CreatedBy`: Usuario que creó el registro
- `LastModifiedAt`: Fecha de última modificación
- `LastModifiedBy`: Usuario que realizó la última modificación

## Flujo de Autorización

Cuando un usuario intenta acceder a una ruta, el sistema realiza las siguientes verificaciones:

1. Verifica si el usuario está autenticado.
2. Obtiene los roles del usuario.
3. Verifica si alguno de los roles del usuario tiene acceso directo a la ruta a través de la tabla `RoleRoutes`.
4. Si no, verifica si alguno de los roles del usuario tiene los permisos necesarios para acceder a la ruta a través de la tabla `PermissionRoutes`.
5. Si la ruta pertenece a un módulo, también verifica si el usuario tiene acceso al módulo a través de la tabla `PermissionModules`.

## Mejores Prácticas

1. **Asignar permisos a roles, no directamente a usuarios**: Esto facilita la gestión de permisos.
2. **Utilizar permisos específicos**: Crear permisos específicos para acciones específicas.
3. **Mantener la nomenclatura estandarizada**: Seguir el formato `[Entidad].[Acción]` para los nombres de permisos.
4. **Asignar todas las rutas a módulos**: Evitar rutas sin módulo asignado para mantener una estructura clara.
5. **Utilizar el módulo "Sin Asignar" temporalmente**: Las rutas en el módulo "Sin Asignar" deben ser revisadas y asignadas a módulos apropiados.

## Conclusión

El sistema de permisos de AuthSystem proporciona un control de acceso flexible y granular a las diferentes funcionalidades de la aplicación. La combinación de permisos, roles, módulos y rutas permite definir políticas de acceso complejas adaptadas a las necesidades específicas de la organización.
