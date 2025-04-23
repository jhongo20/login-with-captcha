# Integración entre Rutas y Permisos

## Introducción

Este documento describe la integración entre el sistema de rutas y el sistema de permisos en AuthSystem, explicando cómo se relacionan las rutas con los permisos y roles, y cómo configurar correctamente estas relaciones.

## Arquitectura de la Integración

La integración entre rutas y permisos se basa en tres entidades principales:

1. **Route**: Representa un endpoint de la API o una página de la aplicación
2. **Permission**: Representa un permiso específico en el sistema
3. **Role**: Representa un rol de usuario con un conjunto de permisos

Las relaciones entre estas entidades se establecen a través de:

- **RoleRoute**: Relaciona roles con rutas (acceso directo)
- **PermissionRoute**: Relaciona permisos con rutas (acceso basado en permisos)
- **RolePermission**: Relaciona roles con permisos (asignación de permisos a roles)

## Flujo de Autorización

El flujo de autorización para acceder a una ruta sigue estos pasos:

1. El usuario intenta acceder a una ruta específica
2. El sistema verifica si el usuario está autenticado (si la ruta requiere autenticación)
3. El sistema verifica si el usuario tiene acceso a la ruta a través de:
   - Acceso directo: El rol del usuario está asignado directamente a la ruta
   - Acceso basado en permisos: El rol del usuario tiene permisos que están asignados a la ruta

## Configuración de Relaciones

### Asignar una Ruta a un Rol

Para permitir que un rol acceda directamente a una ruta:

```http
POST /api/Routes/assign
```

Cuerpo de la solicitud:
```json
{
  "routeId": "72b01367-3dba-4205-849a-335257acd3aa",
  "roleId": "d7e350e8-5fb7-4517-b8da-6f602d66a3a9"
}
```

### Asignar un Permiso a una Ruta

Para requerir un permiso específico para acceder a una ruta:

```http
POST /api/PermissionRoutes/assign/{routeId}/{permissionId}
```

### Revocar un Permiso de una Ruta

Para eliminar el requisito de un permiso para acceder a una ruta:

```http
DELETE /api/PermissionRoutes/revoke/{routeId}/{permissionId}
```

## Consideraciones Importantes

1. **Jerarquía de Módulos**: Una ruta siempre pertenece a un módulo. Para asignar una ruta a un rol, el rol debe tener acceso al módulo que contiene la ruta.

2. **Permisos Implícitos**: Algunos permisos pueden implicar otros permisos. Por ejemplo, un permiso de "Administrador" puede incluir implícitamente todos los demás permisos.

3. **Activación/Desactivación**: Tanto las rutas como las relaciones entre rutas, roles y permisos pueden ser activadas o desactivadas usando el campo `IsActive`.

## Ejemplos de Uso

### Escenario 1: Acceso Basado en Roles

Supongamos que queremos que todos los usuarios con el rol "Admin" tengan acceso a la ruta `/api/users`:

1. Crear la ruta:
```http
POST /api/Routes
```
```json
{
  "name": "Obtener Usuarios",
  "description": "Obtiene la lista de usuarios",
  "path": "/api/users",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "70d4253b-8b9f-4c90-871b-98c4073050fd"
}
```

2. Asignar la ruta al rol "Admin":
```http
POST /api/Routes/assign
```
```json
{
  "routeId": "72b01367-3dba-4205-849a-335257acd3aa",
  "roleId": "d7e350e8-5fb7-4517-b8da-6f602d66a3a9"
}
```

### Escenario 2: Acceso Basado en Permisos

Supongamos que queremos que solo los usuarios con el permiso "users.view" puedan acceder a la ruta `/api/users`:

1. Crear la ruta (como en el escenario 1)

2. Asignar el permiso "users.view" a la ruta:
```http
POST /api/PermissionRoutes/assign/{routeId}/{permissionId}
```

3. Asegurarse de que el rol "Admin" tenga el permiso "users.view":
```http
POST /api/Permissions/{permissionId}/assign-to-role
```
```json
{
  "roleId": "d7e350e8-5fb7-4517-b8da-6f602d66a3a9"
}
```

## Buenas Prácticas

1. **Granularidad de Permisos**: Crear permisos con la granularidad adecuada para permitir un control preciso del acceso.

2. **Nomenclatura Consistente**: Utilizar una nomenclatura consistente para rutas y permisos, como `recurso.acción` (ej. `users.view`, `users.create`).

3. **Documentación**: Mantener documentación actualizada de todas las rutas y sus requisitos de permisos.

4. **Pruebas**: Realizar pruebas exhaustivas de las relaciones entre rutas, roles y permisos para garantizar que el acceso esté correctamente configurado.

## Solución de Problemas Comunes

1. **Error 401 (No autorizado)**: El usuario no está autenticado o el token ha expirado.

2. **Error 403 (Prohibido)**: El usuario está autenticado pero no tiene los permisos necesarios para acceder a la ruta.

3. **Error 500 al asignar rutas o permisos**: Verificar que las entidades existan y estén activas, y que la relación no esté duplicada.
