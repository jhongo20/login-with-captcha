# Documentación del Sistema de Autenticación

## Descripción General

El sistema de autenticación implementado proporciona una solución completa para la gestión de usuarios, roles, permisos y sesiones. Utiliza JWT (JSON Web Tokens) para la autenticación y autorización, con soporte para refresh tokens que permiten renovar la sesión sin necesidad de volver a introducir credenciales.

## Arquitectura

El sistema sigue una arquitectura de Clean Architecture con las siguientes capas:

1. **Domain**: Contiene las entidades del negocio, interfaces y contratos.
   - Entidades principales: User, Role, Permission, UserSession
   - Interfaces para repositorios y servicios

2. **Application**: Contiene la lógica de aplicación, DTOs y servicios.
   - Organizado por características (Features)
   - Utiliza patrón CQRS (Commands/Queries)

3. **Infrastructure**: Implementa las interfaces del dominio y proporciona servicios técnicos.
   - Persistencia: ApplicationDbContext, UnitOfWork, Repositories
   - Servicios: JwtService, CaptchaService, AccountLockoutService

4. **API**: Expone los servicios como API REST.
   - Controladores para autenticación, usuarios, roles y permisos
   - Middleware para manejo de errores, autenticación, etc.

## Características Principales

1. **Autenticación basada en JWT**:
   - Generación de tokens JWT con claims personalizados
   - Refresh tokens para renovar la sesión
   - Invalidación de tokens (logout)

2. **Gestión de Usuarios**:
   - Registro de usuarios
   - Inicio de sesión
   - Bloqueo de cuentas tras múltiples intentos fallidos

3. **Protección contra ataques**:
   - CAPTCHA para proteger el formulario de login
   - Google reCAPTCHA para una protección más robusta
   - Bloqueo de cuentas después de múltiples intentos fallidos

4. **Gestión de Roles y Permisos**:
   - Asignación de roles a usuarios
   - Asignación de permisos a roles
   - Autorización basada en roles y permisos

## Endpoints de Autenticación

### 1. Login

**Endpoint**: `/api/Auth/login`

**Método**: POST

**Descripción**: Inicia sesión con un usuario y contraseña, devolviendo un token JWT y un refresh token.

**Parámetros de entrada**:
```json
{
  "username": "string",
  "password": "string",
  "isLdapUser": false
}
```

**Respuesta exitosa**:
```json
{
  "id": "string",
  "username": "string",
  "email": "string",
  "fullName": "string",
  "token": "string",
  "refreshToken": "string",
  "roles": [
    "string"
  ],
  "permissions": [
    "string"
  ]
}
```

**Ejemplo de uso en Swagger**:
1. Navega a `/api/Auth/login` en Swagger
2. Haz clic en "Try it out"
3. Introduce los siguientes datos:
   ```json
   {
     "username": "admin",
     "password": "Admin123!",
     "isLdapUser": false
   }
   ```
4. Haz clic en "Execute"
5. Guarda el `token` y `refreshToken` para usarlos en otras peticiones

### 2. Login con Google reCAPTCHA

**Endpoint**: `/api/Auth/login-with-google-recaptcha`

**Método**: POST

**Descripción**: Inicia sesión con un usuario, contraseña y token de Google reCAPTCHA.

**Parámetros de entrada**:
```json
{
  "username": "string",
  "password": "string",
  "recaptchaToken": "string",
  "isLdapUser": false
}
```

**Respuesta exitosa**:
```json
{
  "id": "string",
  "username": "string",
  "email": "string",
  "fullName": "string",
  "token": "string",
  "refreshToken": "string",
  "roles": [
    "string"
  ],
  "permissions": [
    "string"
  ]
}
```

**Ejemplo de uso en Swagger**:
1. Navega a `/api/Auth/login-with-google-recaptcha` en Swagger
2. Haz clic en "Try it out"
3. Introduce los siguientes datos:
   ```json
   {
     "username": "admin",
     "password": "Admin123!",
     "recaptchaToken": "03AGdBq24PBCbwiDRaS_MJ...", // Token generado por reCAPTCHA
     "isLdapUser": false
   }
   ```
4. Haz clic en "Execute"
5. Guarda el `token` y `refreshToken` para usarlos en otras peticiones

### 3. Refresh Token

**Endpoint**: `/api/Auth/refresh-token`

**Método**: POST

**Descripción**: Renueva un token JWT utilizando un refresh token.

**Parámetros de entrada**:
```json
{
  "refreshToken": "string"
}
```

**Respuesta exitosa**:
```json
{
  "id": "string",
  "username": "string",
  "email": "string",
  "fullName": "string",
  "token": "string",
  "refreshToken": "string",
  "roles": [
    "string"
  ],
  "permissions": [
    "string"
  ]
}
```

**Ejemplo de uso en Swagger**:
1. Navega a `/api/Auth/refresh-token` en Swagger
2. Haz clic en "Try it out"
3. Introduce el refresh token obtenido en el login:
   ```json
   {
     "refreshToken": "tu_refresh_token_aquí"
   }
   ```
4. Haz clic en "Execute"
5. Recibirás un nuevo `token` y `refreshToken`

### 4. Logout

**Endpoint**: `/api/Auth/logout`

**Método**: POST

**Descripción**: Cierra la sesión del usuario, invalidando el refresh token.

**Parámetros de entrada**:
```json
{
  "refreshToken": "string"
}
```

**Respuesta exitosa**:
```json
{
  "message": "Sesión cerrada correctamente"
}
```

**Ejemplo de uso en Swagger**:
1. Navega a `/api/Auth/logout` en Swagger
2. Haz clic en "Try it out"
3. Introduce el refresh token que deseas invalidar:
   ```json
   {
     "refreshToken": "tu_refresh_token_aquí"
   }
   ```
4. Haz clic en "Execute"
5. Recibirás un mensaje de confirmación

## Flujo de Autenticación

1. **Inicio de sesión**:
   - El usuario proporciona sus credenciales (username/password)
   - El sistema valida las credenciales
   - Si son válidas, genera un token JWT y un refresh token
   - El refresh token se almacena en la base de datos
   - Se devuelven ambos tokens al cliente

2. **Uso del token JWT**:
   - El cliente incluye el token JWT en el header Authorization de sus peticiones
   - El servidor valida el token JWT
   - Si es válido, procesa la petición

3. **Renovación del token JWT**:
   - Cuando el token JWT expira, el cliente puede usar el refresh token para obtener uno nuevo
   - El cliente envía el refresh token al endpoint `/api/Auth/refresh-token`
   - El servidor valida el refresh token contra la base de datos
   - Si es válido, genera un nuevo token JWT y un nuevo refresh token
   - Se invalida el refresh token anterior
   - Se devuelven los nuevos tokens al cliente

4. **Cierre de sesión**:
   - El cliente envía el refresh token al endpoint `/api/Auth/logout`
   - El servidor invalida el refresh token en la base de datos
   - La sesión se considera cerrada

## Seguridad

1. **Protección de contraseñas**:
   - Las contraseñas se almacenan hasheadas con BCrypt
   - No se almacenan contraseñas en texto plano

2. **Protección contra ataques de fuerza bruta**:
   - Bloqueo de cuentas después de múltiples intentos fallidos
   - Tiempo de bloqueo configurable

3. **Protección contra bots**:
   - CAPTCHA para proteger el formulario de login
   - Google reCAPTCHA para una protección más robusta

4. **Seguridad de tokens**:
   - Los tokens JWT tienen un tiempo de expiración corto (60 minutos por defecto)
   - Los refresh tokens tienen un tiempo de expiración más largo (7 días por defecto)
   - Los refresh tokens se validan contra la base de datos
   - Los refresh tokens se invalidan al usarse

## Configuración

La configuración del sistema se encuentra en el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "S3cr3t_K3y!2023_AuthSystem_JWT_Token_Key_Must_Be_Long_Enough",
    "Issuer": "AuthSystem",
    "Audience": "AuthSystemClient",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Captcha": {
    "RecaptchaSecretKey": "tu_clave_secreta_aquí",
    "RecaptchaPublicKey": "tu_clave_pública_aquí"
  },
  "Security": {
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 15,
    "PasswordRequireDigit": true,
    "PasswordRequireLowercase": true,
    "PasswordRequireUppercase": true,
    "PasswordRequireNonAlphanumeric": true,
    "PasswordMinLength": 8
  }
}
```

## Pruebas

Se han incluido pruebas unitarias para los componentes principales del sistema:

- Pruebas para el JwtService
- Pruebas para el AuthController
- Pruebas para los repositorios

Para ejecutar las pruebas, utiliza el siguiente comando:

```bash
dotnet test
```

## Solución de Problemas

### Problema con Refresh Tokens

Si experimentas problemas con los refresh tokens, verifica lo siguiente:

1. Asegúrate de que la tabla `UserSessions` existe en la base de datos
2. Verifica que el refresh token se está enviando correctamente en las peticiones
3. Comprueba que el refresh token no ha expirado
4. Verifica que el refresh token no ha sido invalidado (por ejemplo, por un logout)

### Problema con el Login

Si experimentas problemas con el login, verifica lo siguiente:

1. Asegúrate de que el usuario existe en la base de datos
2. Verifica que la contraseña es correcta
3. Comprueba que el usuario no está bloqueado
4. Verifica que el usuario está activo

## Ejemplos de Código

### Cliente JavaScript

```javascript
// Función para iniciar sesión
async function login(username, password) {
  const response = await fetch('/api/Auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username,
      password,
      isLdapUser: false
    })
  });
  
  const data = await response.json();
  
  if (response.ok) {
    // Guardar tokens en localStorage
    localStorage.setItem('token', data.token);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  } else {
    throw new Error(data.message);
  }
}

// Función para refrescar el token
async function refreshToken() {
  const refreshToken = localStorage.getItem('refreshToken');
  
  if (!refreshToken) {
    throw new Error('No hay refresh token disponible');
  }
  
  const response = await fetch('/api/Auth/refresh-token', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      refreshToken
    })
  });
  
  const data = await response.json();
  
  if (response.ok) {
    // Actualizar tokens en localStorage
    localStorage.setItem('token', data.token);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  } else {
    // Si hay un error, limpiar tokens y redirigir al login
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    throw new Error(data.message);
  }
}

// Función para cerrar sesión
async function logout() {
  const refreshToken = localStorage.getItem('refreshToken');
  
  if (!refreshToken) {
    return;
  }
  
  const response = await fetch('/api/Auth/logout', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    },
    body: JSON.stringify({
      refreshToken
    })
  });
  
  // Limpiar tokens independientemente del resultado
  localStorage.removeItem('token');
  localStorage.removeItem('refreshToken');
  
  const data = await response.json();
  
  if (!response.ok) {
    throw new Error(data.message);
  }
  
  return data;
}
```

## Conclusión

El sistema de autenticación implementado proporciona una solución completa y segura para la gestión de usuarios, roles, permisos y sesiones. Utiliza las mejores prácticas de seguridad y ofrece una experiencia de usuario fluida con soporte para refresh tokens.
