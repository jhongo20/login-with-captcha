# Guía de Pruebas para el Módulo de Módulos

## Introducción

Esta guía proporciona instrucciones para probar la funcionalidad del módulo de Módulos a través de la API. Se incluyen ejemplos de solicitudes y respuestas para cada endpoint, así como escenarios de prueba comunes.

## Requisitos previos

- Tener la API en ejecución
- Tener un token JWT válido para autenticación
- Herramientas recomendadas: Postman, Swagger UI, o cualquier cliente HTTP

## Autenticación

Todas las solicitudes a los endpoints de módulos requieren autenticación. Primero, obtenga un token JWT:

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "your_password"
}
```

Incluya el token en todas las solicitudes posteriores:

```
Authorization: Bearer {token}
```

## Escenarios de prueba

### 1. Crear módulos principales

#### Solicitud
```http
POST /api/Modules
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Dashboard",
  "description": "Panel principal del sistema",
  "route": "/dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true
}
```

#### Respuesta esperada
```json
{
  "id": "guid-generado",
  "name": "Dashboard",
  "description": "Panel principal del sistema",
  "route": "/dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true,
  "createdAt": "fecha-hora",
  "createdBy": "usuario-actual",
  "updatedAt": "fecha-hora",
  "updatedBy": "usuario-actual"
}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado (usa el endpoint `/api/Auth/login` y luego el botón "Authorize")
2. Navega a la sección `POST /api/Modules`
3. Haz clic en "Try it out"
4. Ingresa el siguiente JSON en el cuerpo de la solicitud:
```json
{
  "name": "Dashboard",
  "description": "Panel principal del sistema",
  "route": "/dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true
}
```
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 200 y contenga los datos del módulo creado
7. Guarda el ID generado para usarlo en pruebas posteriores

### 2. Crear submódulos

#### Solicitud
```http
POST /api/Modules
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Estadísticas",
  "description": "Estadísticas del sistema",
  "route": "/dashboard/stats",
  "icon": "fa-chart-bar",
  "displayOrder": 1,
  "parentId": "guid-del-modulo-dashboard",
  "isEnabled": true
}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `POST /api/Modules`
3. Haz clic en "Try it out"
4. Ingresa el siguiente JSON en el cuerpo de la solicitud (reemplaza el valor de `parentId` con el ID real del módulo Dashboard creado anteriormente):
```json
{
  "name": "Estadísticas",
  "description": "Estadísticas del sistema",
  "route": "/dashboard/stats",
  "icon": "fa-chart-bar",
  "displayOrder": 1,
  "parentId": "guid-del-modulo-dashboard",
  "isEnabled": true
}
```
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 200 y contenga los datos del submódulo creado

### 3. Obtener todos los módulos

#### Solicitud
```http
GET /api/Modules
Authorization: Bearer {token}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `GET /api/Modules`
3. Haz clic en "Try it out"
4. Haz clic en "Execute"
5. Verifica que la respuesta tenga un código de estado 200 y contenga una lista de módulos
6. Comprueba que los módulos creados en los pasos anteriores estén en la lista

### 4. Obtener un módulo específico

#### Solicitud
```http
GET /api/Modules/{id}
Authorization: Bearer {token}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `GET /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID del módulo Dashboard creado anteriormente
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 200 y contenga los datos del módulo Dashboard

### 5. Actualizar un módulo

#### Solicitud
```http
PUT /api/Modules/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Dashboard Actualizado",
  "description": "Panel principal actualizado",
  "route": "/dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true
}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `PUT /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID del módulo Dashboard creado anteriormente
5. Ingresa el siguiente JSON en el cuerpo de la solicitud:
```json
{
  "name": "Dashboard Actualizado",
  "description": "Panel principal actualizado",
  "route": "/dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 1,
  "parentId": null,
  "isEnabled": true
}
```
6. Haz clic en "Execute"
7. Verifica que la respuesta tenga un código de estado 200 y contenga los datos actualizados del módulo

### 6. Eliminar un módulo

#### Solicitud
```http
DELETE /api/Modules/{id}
Authorization: Bearer {token}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `DELETE /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID de un módulo que no tenga submódulos
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 204 (No Content)
7. Intenta obtener el módulo eliminado para confirmar que ya no existe

### 7. Obtener submódulos

#### Solicitud
```http
GET /api/Modules/{id}/children
Authorization: Bearer {token}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `GET /api/Modules/{id}/children`
3. Haz clic en "Try it out"
4. Ingresa el ID del módulo Dashboard creado anteriormente
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 200 y contenga una lista con el submódulo Estadísticas

### 8. Obtener módulos habilitados

#### Solicitud
```http
GET /api/Modules/enabled
Authorization: Bearer {token}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `GET /api/Modules/enabled`
3. Haz clic en "Try it out"
4. Haz clic en "Execute"
5. Verifica que la respuesta tenga un código de estado 200 y contenga una lista de módulos habilitados
6. Comprueba que solo se muestren los módulos con `isEnabled` establecido en `true`

## Casos de prueba para validaciones

### 1. Intentar crear un módulo con nombre duplicado

#### Solicitud
```http
POST /api/Modules
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Dashboard",  // Nombre que ya existe
  "description": "Otro panel",
  "route": "/otro-dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 2,
  "parentId": null,
  "isEnabled": true
}
```

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `POST /api/Modules`
3. Haz clic en "Try it out"
4. Ingresa el siguiente JSON en el cuerpo de la solicitud (usando el mismo nombre que un módulo existente):
```json
{
  "name": "Dashboard",
  "description": "Otro panel",
  "route": "/otro-dashboard",
  "icon": "fa-tachometer-alt",
  "displayOrder": 2,
  "parentId": null,
  "isEnabled": true
}
```
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 400 (Bad Request)
7. Comprueba que el mensaje de error indique que ya existe un módulo con ese nombre

### 2. Intentar crear un ciclo en la jerarquía

Para esta prueba, necesitarás:
- Un módulo principal (Módulo A)
- Un submódulo del Módulo A (Módulo B)

Luego intentarás establecer el Módulo A como hijo del Módulo B, lo que crearía un ciclo.

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `PUT /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID del Módulo A
5. Ingresa el siguiente JSON en el cuerpo de la solicitud (usando el ID del Módulo B como parentId):
```json
{
  "name": "Módulo A",
  "description": "Módulo principal",
  "route": "/modulo-a",
  "icon": "fa-folder",
  "displayOrder": 1,
  "parentId": "ID-DEL-MODULO-B",
  "isEnabled": true
}
```
6. Haz clic en "Execute"
7. Verifica que la respuesta tenga un código de estado 400 (Bad Request)
8. Comprueba que el mensaje de error indique que no se puede establecer el módulo padre porque crearía un ciclo

### 3. Intentar eliminar un módulo con submódulos

#### Prueba en Swagger
1. Asegúrate de estar autenticado
2. Navega a la sección `DELETE /api/Modules/{id}`
3. Haz clic en "Try it out"
4. Ingresa el ID del módulo Dashboard (que tiene el submódulo Estadísticas)
5. Haz clic en "Execute"
6. Verifica que la respuesta tenga un código de estado 400 (Bad Request)
7. Comprueba que el mensaje de error indique que no se puede eliminar el módulo porque tiene submódulos asociados

## Pruebas de rendimiento

Para probar el rendimiento de los endpoints de módulos, se recomienda:

1. Crear una cantidad significativa de módulos (por ejemplo, 100)
2. Medir el tiempo de respuesta de los endpoints principales
3. Verificar el comportamiento con diferentes niveles de jerarquía (módulos con múltiples niveles de submódulos)

## Pruebas de seguridad

1. Intentar acceder a los endpoints sin token de autenticación
2. Intentar acceder con un token expirado
3. Intentar acceder con un token válido pero sin los permisos necesarios

## Automatización de pruebas

Se recomienda automatizar estas pruebas utilizando herramientas como:

- Postman Collections
- Newman para ejecución de colecciones de Postman desde la línea de comandos
- JMeter para pruebas de carga
- Frameworks de prueba como xUnit o NUnit para pruebas de integración

## Solución de problemas comunes

### Error 401 Unauthorized
- Verificar que el token JWT es válido y no ha expirado
- Asegurarse de incluir el encabezado `Authorization: Bearer {token}` en todas las solicitudes

### Error 400 Bad Request
- Verificar que los datos enviados cumplen con las validaciones
- Revisar los mensajes de error para identificar el problema específico

### Error 404 Not Found
- Verificar que el ID del módulo existe
- Comprobar que la URL del endpoint es correcta

## Autenticación para las pruebas

Para obtener un token JWT y autorizar las solicitudes en Swagger:

1. Navega a la sección `POST /api/Auth/login`
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

Ahora todas tus solicitudes incluirán el token de autenticación.
