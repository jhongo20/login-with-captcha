# Guía para probar los endpoints de Rutas en Swagger

Esta guía te ayudará a probar todos los endpoints del sistema de rutas utilizando Swagger.

## Requisitos previos

1. Tener la aplicación en ejecución
2. Acceder a Swagger en http://localhost:5031/swagger
3. Tener credenciales de un usuario con rol de Administrador

## Paso 1: Autenticación

Antes de poder probar los endpoints de rutas, necesitas autenticarte:

1. Navega a la sección `/api/Auth/login`
2. Haz clic en "Try it out"
3. Ingresa las credenciales de administrador:
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```
4. Haz clic en "Execute"
5. Copia el token JWT de la respuesta
6. Haz clic en el botón "Authorize" en la parte superior de la página de Swagger
7. Ingresa el token en el formato: `Bearer {token}`
8. Haz clic en "Authorize"

## Paso 2: Obtener módulos disponibles

Antes de crear rutas, necesitamos conocer los módulos disponibles:

1. Navega a la sección `GET /api/Modules`
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Anota el ID de al menos un módulo para usarlo en los siguientes pasos

## Paso 3: Crear una nueva ruta

1. Navega a la sección `POST /api/Routes`
2. Haz clic en "Try it out"
3. Ingresa el siguiente JSON en el cuerpo de la solicitud (reemplaza el `moduleId` con el ID que anotaste en el paso anterior):
```json
{
  "name": "Obtener usuarios",
  "description": "Obtiene la lista de usuarios del sistema",
  "path": "/api/users",
  "httpMethod": "GET",
  "displayOrder": 1,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```
4. Haz clic en "Execute"
5. Verifica que la respuesta tenga un código de estado 201 (Created)
6. Anota el ID de la ruta creada para usarlo en los siguientes pasos

## Paso 4: Obtener todas las rutas

1. Navega a la sección `GET /api/Routes`
2. Haz clic en "Try it out"
3. Haz clic en "Execute"
4. Verifica que la respuesta incluya la ruta que acabas de crear

## Paso 5: Obtener una ruta específica

1. Navega a la sección `GET /api/Routes/{id}`
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que creaste en el paso 3
4. Haz clic en "Execute"
5. Verifica que la respuesta muestre los detalles de la ruta específica

## Paso 6: Obtener rutas por módulo

1. Navega a la sección `GET /api/Routes/byModule/{moduleId}`
2. Haz clic en "Try it out"
3. Ingresa el ID del módulo que usaste en el paso 3
4. Haz clic en "Execute"
5. Verifica que la respuesta incluya la ruta que creaste para ese módulo

## Paso 7: Actualizar una ruta

1. Navega a la sección `PUT /api/Routes/{id}`
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que creaste en el paso 3
4. Ingresa el siguiente JSON en el cuerpo de la solicitud (reemplaza el `moduleId` con el ID que usaste anteriormente):
```json
{
  "name": "Obtener usuarios actualizado",
  "description": "Obtiene la lista actualizada de usuarios del sistema",
  "path": "/api/users",
  "httpMethod": "GET",
  "displayOrder": 2,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 200 (OK) y muestre los detalles actualizados

## Paso 8: Asignar una ruta a un rol

1. Navega a la sección `GET /api/Roles` para obtener los roles disponibles
2. Haz clic en "Try it out" y luego "Execute"
3. Anota el ID del rol "Admin" (normalmente es `D7E350E8-5FB7-4517-B8DA-6F602D66A3A9`)
4. Navega a la sección `POST /api/Routes/assign`
5. Haz clic en "Try it out"
6. Ingresa el siguiente JSON en el cuerpo de la solicitud:
```json
{
  "roleId": "D7E350E8-5FB7-4517-B8DA-6F602D66A3A9",
  "routeId": "ID-DE-TU-RUTA"
}
```
7. Haz clic en "Execute"
8. Verifica que la respuesta tenga un código de estado 200 (OK)

## Paso 9: Obtener rutas por rol

1. Navega a la sección `GET /api/Routes/byRole/{roleId}`
2. Haz clic en "Try it out"
3. Ingresa el ID del rol que usaste en el paso 8
4. Haz clic en "Execute"
5. Verifica que la respuesta incluya la ruta que asignaste a ese rol

## Paso 10: Revocar una ruta de un rol

1. Navega a la sección `DELETE /api/Routes/revoke/{roleId}/{routeId}`
2. Haz clic en "Try it out"
3. Ingresa el ID del rol y el ID de la ruta que usaste en el paso 8
4. Haz clic en "Execute"
5. Verifica que la respuesta tenga un código de estado 200 (OK)
6. Verifica que la ruta ya no aparezca al obtener las rutas por rol (repite el paso 9)

## Paso 11: Eliminar una ruta

1. Navega a la sección `DELETE /api/Routes/{id}`
2. Haz clic en "Try it out"
3. Ingresa el ID de la ruta que creaste
4. Haz clic en "Execute"
5. Verifica que la respuesta tenga un código de estado 204 (No Content)
6. Verifica que la ruta ya no aparezca al obtener todas las rutas (repite el paso 4)

## Ejemplos adicionales

### Crear una ruta para un endpoint POST

```json
{
  "name": "Crear usuario",
  "description": "Crea un nuevo usuario en el sistema",
  "path": "/api/users",
  "httpMethod": "POST",
  "displayOrder": 3,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```

### Crear una ruta para un endpoint PUT

```json
{
  "name": "Actualizar usuario",
  "description": "Actualiza un usuario existente",
  "path": "/api/users/{id}",
  "httpMethod": "PUT",
  "displayOrder": 4,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```

### Crear una ruta para un endpoint DELETE

```json
{
  "name": "Eliminar usuario",
  "description": "Elimina un usuario existente",
  "path": "/api/users/{id}",
  "httpMethod": "DELETE",
  "displayOrder": 5,
  "requiresAuth": true,
  "isEnabled": true,
  "moduleId": "7b8e8c2f-d39a-4b1a-b11e-0e39d3b7d8f3"
}
```

## Solución de problemas comunes

### Error 401 Unauthorized

Si recibes un error 401, significa que tu token JWT ha expirado o no es válido:
1. Repite el paso 1 para obtener un nuevo token
2. Asegúrate de incluir el prefijo "Bearer " antes del token

### Error 403 Forbidden

Si recibes un error 403, significa que no tienes los permisos necesarios:
1. Asegúrate de estar utilizando una cuenta con rol de Administrador
2. Verifica que el rol tenga los permisos `routes.view`, `routes.create`, `routes.edit` y `routes.delete`

### Error 400 Bad Request

Si recibes un error 400, revisa los siguientes aspectos:
1. Asegúrate de que el formato del JSON sea correcto
2. Verifica que el `moduleId` sea válido
3. Comprueba que no estés intentando crear una ruta con un nombre que ya existe en el mismo módulo
4. Asegúrate de que no estés intentando crear una ruta con el mismo path y método HTTP que otra existente
