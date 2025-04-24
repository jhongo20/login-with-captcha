# Gestión de Usuarios Inactivos - AuthSystem

## Descripción

Este documento describe las nuevas funcionalidades implementadas en el sistema AuthSystem para la gestión de usuarios inactivos. Estas mejoras permiten a los administradores ver, gestionar y reactivar usuarios que han sido desactivados en el sistema.

## Endpoints Implementados

### 1. Listar todos los usuarios (incluyendo inactivos)

Este endpoint permite obtener una lista completa de todos los usuarios del sistema, independientemente de su estado de activación.

- **URL**: `/api/Users/all`
- **Método**: `GET`
- **Autorización**: Requiere rol "Admin"
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Lista de objetos UserDto

Ejemplo de respuesta:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "usuario1",
    "email": "usuario1@ejemplo.com",
    "fullName": "Usuario Uno",
    "isActive": true,
    "userType": "Local",
    "createdAt": "2025-04-24T10:30:00",
    "roles": ["Admin", "User"]
  },
  {
    "id": "8a4b6c7d-9e0f-1a2b-3c4d-5e6f7a8b9c0d",
    "username": "usuario2",
    "email": "usuario2@ejemplo.com",
    "fullName": "Usuario Dos",
    "isActive": false,
    "userType": "Local",
    "createdAt": "2025-04-20T15:45:00",
    "roles": ["User"]
  }
]
```

### 2. Obtener un usuario específico (incluyendo inactivos)

Este endpoint permite obtener los detalles de un usuario específico por su ID, independientemente de su estado de activación.

- **URL**: `/api/Users/all/{id}`
- **Método**: `GET`
- **Autorización**: Requiere rol "Admin"
- **Parámetros de ruta**:
  - `id`: ID del usuario (GUID)
- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Objeto UserDto

Ejemplo de respuesta:
```json
{
  "id": "8a4b6c7d-9e0f-1a2b-3c4d-5e6f7a8b9c0d",
  "username": "usuario2",
  "email": "usuario2@ejemplo.com",
  "fullName": "Usuario Dos",
  "isActive": false,
  "userType": "Local",
  "createdAt": "2025-04-20T15:45:00",
  "roles": ["User"]
}
```

### 3. Actualizar un usuario (incluyendo inactivos)

Este endpoint permite actualizar los datos de un usuario, incluyendo la posibilidad de activar o desactivar usuarios.

- **URL**: `/api/Users/all/{id}`
- **Método**: `PUT`
- **Autorización**: Requiere rol "Admin"
- **Parámetros de ruta**:
  - `id`: ID del usuario (GUID)
- **Cuerpo de la solicitud**: Objeto UpdateUserRequest

Ejemplo de solicitud:
```json
{
  "username": "usuario2_actualizado",
  "email": "usuario2_nuevo@ejemplo.com",
  "fullName": "Usuario Dos Actualizado",
  "password": "NuevaContraseña123!",
  "confirmPassword": "NuevaContraseña123!",
  "isActive": true,
  "roles": ["User", "Manager"]
}
```

- **Respuesta exitosa**:
  - **Código**: 200 OK
  - **Contenido**: Objeto UserDto actualizado

Ejemplo de respuesta:
```json
{
  "id": "8a4b6c7d-9e0f-1a2b-3c4d-5e6f7a8b9c0d",
  "username": "usuario2_actualizado",
  "email": "usuario2_nuevo@ejemplo.com",
  "fullName": "Usuario Dos Actualizado",
  "isActive": true,
  "userType": "Local",
  "createdAt": "2025-04-20T15:45:00",
  "roles": ["User", "Manager"]
}
```

## Casos de Uso

### Gestión de Usuarios Inactivos

1. **Visualización de todos los usuarios**: Los administradores pueden ver una lista completa de todos los usuarios del sistema, incluyendo aquellos que han sido desactivados.

2. **Consulta de detalles de usuarios inactivos**: Los administradores pueden consultar los detalles específicos de un usuario inactivo para revisar su información.

3. **Reactivación de cuentas**: Los administradores pueden reactivar cuentas de usuario que fueron desactivadas, cambiando el valor de `isActive` a `true`.

4. **Actualización de usuarios inactivos**: Los administradores pueden modificar la información de usuarios inactivos sin necesidad de reactivarlos primero.

5. **Auditoría completa**: Los administradores tienen acceso a todos los usuarios del sistema para fines de auditoría y seguimiento.

## Implementación Técnica

### Nuevos Métodos en el Repositorio

1. **GetAllUsersIncludingInactiveAsync**:
   - Obtiene todos los usuarios sin filtrar por estado de activación.
   - Incluye relaciones con roles para obtener información completa.

2. **GetByIdIncludingInactiveAsync**:
   - Obtiene un usuario específico por ID sin filtrar por estado de activación.
   - Incluye relaciones con roles para obtener información completa.

### Modificaciones en el Controlador

Se han implementado tres nuevos endpoints en el controlador `UsersController`:

1. **GetAllUsersIncludingInactive**: Endpoint para listar todos los usuarios.
2. **GetUserByIdIncludingInactive**: Endpoint para obtener un usuario específico.
3. **UpdateUserIncludingInactive**: Endpoint para actualizar un usuario.

Los endpoints originales se mantienen para preservar la compatibilidad con el código existente.

## Consideraciones de Seguridad

- Todos los nuevos endpoints requieren el rol "Admin" para su acceso.
- Las operaciones de actualización verifican la existencia de nombres de usuario y correos electrónicos duplicados.
- Las contraseñas se almacenan utilizando hash seguro con BCrypt.
- Se mantiene un registro de auditoría con fechas de modificación y usuarios que realizaron los cambios.

## Ejemplos de Uso con cURL

### Listar todos los usuarios (incluyendo inactivos)

```bash
curl -X GET "http://localhost:5031/api/Users/all" \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json"
```

### Obtener un usuario específico (incluyendo inactivos)

```bash
curl -X GET "http://localhost:5031/api/Users/all/{id}" \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json"
```

### Actualizar un usuario (incluyendo inactivos)

```bash
curl -X PUT "http://localhost:5031/api/Users/all/{id}" \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json" \
     -d '{
           "username": "usuario2_actualizado",
           "email": "usuario2_nuevo@ejemplo.com",
           "fullName": "Usuario Dos Actualizado",
           "password": "NuevaContraseña123!",
           "confirmPassword": "NuevaContraseña123!",
           "isActive": true,
           "roles": ["User", "Manager"]
         }'
```
