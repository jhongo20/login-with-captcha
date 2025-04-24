# Documentación Técnica - Gestión de Usuarios Inactivos

## Introducción

Este documento proporciona información técnica detallada sobre la implementación de las funcionalidades para gestionar usuarios inactivos en el sistema AuthSystem. Está dirigido a desarrolladores que necesiten entender, mantener o extender estas funcionalidades.

## Arquitectura

El sistema AuthSystem sigue una arquitectura de Clean Architecture con las siguientes capas:

1. **Domain**: Contiene las entidades del negocio, interfaces y contratos.
2. **Application**: Contiene la lógica de aplicación, DTOs, servicios y características.
3. **Infrastructure**: Implementa las interfaces del dominio y proporciona servicios técnicos.
4. **API**: Expone los servicios como API REST.

## Cambios Implementados

### 1. Interfaces del Dominio

Se han agregado nuevos métodos en la interfaz `IUserRepository` para permitir la gestión de usuarios inactivos:

```csharp
/// <summary>
/// Obtiene todos los usuarios incluyendo los inactivos
/// </summary>
/// <param name="cancellationToken">Token de cancelación</param>
/// <returns>Lista de todos los usuarios, activos e inactivos</returns>
Task<IEnumerable<User>> GetAllUsersIncludingInactiveAsync(CancellationToken cancellationToken = default);

/// <summary>
/// Obtiene un usuario por su ID incluyendo inactivos
/// </summary>
/// <param name="id">ID del usuario</param>
/// <param name="cancellationToken">Token de cancelación</param>
/// <returns>Usuario encontrado o null</returns>
Task<User> GetByIdIncludingInactiveAsync(Guid id, CancellationToken cancellationToken = default);
```

### 2. Implementación de Repositorios

Se han implementado los métodos definidos en la interfaz `IUserRepository` en la clase `UserRepository`:

```csharp
/// <summary>
/// Obtiene todos los usuarios incluyendo los inactivos
/// </summary>
/// <param name="cancellationToken">Token de cancelación</param>
/// <returns>Lista de todos los usuarios, activos e inactivos</returns>
public async Task<IEnumerable<User>> GetAllUsersIncludingInactiveAsync(CancellationToken cancellationToken = default)
{
    return await _dbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .ToListAsync(cancellationToken);
}

/// <summary>
/// Obtiene un usuario por su ID incluyendo inactivos
/// </summary>
/// <param name="id">ID del usuario</param>
/// <param name="cancellationToken">Token de cancelación</param>
/// <returns>Usuario encontrado o null</returns>
public async Task<User> GetByIdIncludingInactiveAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _dbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
}
```

### 3. Endpoints de API

Se han implementado tres nuevos endpoints en el controlador `UsersController`:

#### 3.1. Listar todos los usuarios (incluyendo inactivos)

```csharp
/// <summary>
/// Obtiene todos los usuarios incluyendo los inactivos
/// </summary>
/// <returns>Lista de todos los usuarios, activos e inactivos</returns>
[HttpGet("all")]
[Authorize(Roles = "Admin")]
[ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
[ProducesResponseType(typeof(ErrorResponse), 401)]
[ProducesResponseType(typeof(ErrorResponse), 403)]
[ProducesResponseType(typeof(ErrorResponse), 500)]
public async Task<IActionResult> GetAllUsersIncludingInactive()
{
    try
    {
        var users = await _unitOfWork.Users.GetAllUsersIncludingInactiveAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            IsActive = u.IsActive,
            UserType = u.UserType.ToString(),
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(userDtos);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener todos los usuarios incluyendo inactivos");
        return StatusCode(500, new ErrorResponse
        {
            Message = "Error al obtener todos los usuarios"
        });
    }
}
```

#### 3.2. Obtener un usuario específico (incluyendo inactivos)

```csharp
/// <summary>
/// Obtiene un usuario por su ID incluyendo inactivos
/// </summary>
/// <param name="id">ID del usuario</param>
/// <returns>Usuario</returns>
[HttpGet("all/{id}")]
[Authorize(Roles = "Admin")]
[ProducesResponseType(typeof(UserDto), 200)]
[ProducesResponseType(typeof(ErrorResponse), 401)]
[ProducesResponseType(typeof(ErrorResponse), 403)]
[ProducesResponseType(typeof(ErrorResponse), 404)]
[ProducesResponseType(typeof(ErrorResponse), 500)]
public async Task<IActionResult> GetUserByIdIncludingInactive(Guid id)
{
    try
    {
        var user = await _unitOfWork.Users.GetByIdIncludingInactiveAsync(id);
        if (user == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Usuario no encontrado"
            });
        }

        var roles = await _unitOfWork.Roles.GetByUserAsync(id);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            UserType = user.UserType.ToString(),
            CreatedAt = user.CreatedAt,
            Roles = roles.Select(r => r.Name).ToArray()
        };

        return Ok(userDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener el usuario con ID {UserId} incluyendo inactivos", id);
        return StatusCode(500, new ErrorResponse
        {
            Message = "Error al obtener el usuario"
        });
    }
}
```

#### 3.3. Actualizar un usuario (incluyendo inactivos)

```csharp
/// <summary>
/// Actualiza un usuario existente incluyendo inactivos
/// </summary>
/// <param name="id">ID del usuario</param>
/// <param name="request">Datos del usuario</param>
/// <returns>Usuario actualizado</returns>
[HttpPut("all/{id}")]
[Authorize(Roles = "Admin")]
[ProducesResponseType(typeof(UserDto), 200)]
[ProducesResponseType(typeof(ErrorResponse), 400)]
[ProducesResponseType(typeof(ErrorResponse), 401)]
[ProducesResponseType(typeof(ErrorResponse), 403)]
[ProducesResponseType(typeof(ErrorResponse), 404)]
[ProducesResponseType(typeof(ErrorResponse), 500)]
public async Task<IActionResult> UpdateUserIncludingInactive(Guid id, [FromBody] UpdateUserRequest request)
{
    try
    {
        // Validación del modelo
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Datos de usuario inválidos",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        // Obtener usuario incluyendo inactivos
        var user = await _unitOfWork.Users.GetByIdIncludingInactiveAsync(id);
        if (user == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Usuario no encontrado"
            });
        }

        // Verificaciones de unicidad
        // Actualización de datos
        // Actualización de roles
        // Guardar cambios

        return Ok(userDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId} incluyendo inactivos", id);
        return StatusCode(500, new ErrorResponse
        {
            Message = "Error al actualizar el usuario"
        });
    }
}
```

## Modelo de Datos

### Entidad User

La entidad `User` hereda de `BaseEntity` que contiene las siguientes propiedades comunes:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool IsActive { get; set; } = true;
}
```

La propiedad `IsActive` es la que determina si un usuario está activo o inactivo en el sistema.

### DTO de Usuario

El DTO `UserDto` utilizado para las respuestas de la API incluye la propiedad `IsActive`:

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public string UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string[] Roles { get; set; }
}
```

### Modelo de Actualización

El modelo `UpdateUserRequest` utilizado para las solicitudes de actualización incluye la propiedad `IsActive`:

```csharp
public class UpdateUserRequest
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    public string Username { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre completo debe tener entre 3 y 100 caracteres")]
    public string FullName { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden")]
    public string ConfirmPassword { get; set; }

    public bool? IsActive { get; set; }

    public List<string> Roles { get; set; }
}
```

## Flujo de Trabajo

### Listar Usuarios

1. El cliente hace una solicitud GET a `/api/Users/all`.
2. El controlador verifica la autenticación y autorización.
3. Se llama al método `GetAllUsersIncludingInactiveAsync` del repositorio.
4. Se mapean las entidades a DTOs.
5. Se devuelve la lista de usuarios.

### Obtener Usuario

1. El cliente hace una solicitud GET a `/api/Users/all/{id}`.
2. El controlador verifica la autenticación y autorización.
3. Se llama al método `GetByIdIncludingInactiveAsync` del repositorio.
4. Se verifica si el usuario existe.
5. Se mapea la entidad a DTO.
6. Se devuelve el usuario.

### Actualizar Usuario

1. El cliente hace una solicitud PUT a `/api/Users/all/{id}`.
2. El controlador verifica la autenticación y autorización.
3. Se valida el modelo de solicitud.
4. Se llama al método `GetByIdIncludingInactiveAsync` del repositorio.
5. Se verifica si el usuario existe.
6. Se verifican restricciones de unicidad (nombre de usuario, correo electrónico).
7. Se actualizan los datos del usuario.
8. Se actualizan los roles si es necesario.
9. Se guarda la entidad actualizada.
10. Se mapea la entidad a DTO.
11. Se devuelve el usuario actualizado.

## Consideraciones de Seguridad

- Todos los endpoints requieren autenticación y el rol "Admin".
- Se validan los datos de entrada para prevenir inyecciones y otros ataques.
- Se registran los cambios con información de auditoría (quién y cuándo).
- Las contraseñas se almacenan utilizando hash seguro con BCrypt.

## Pruebas

Para probar estos endpoints, se recomienda utilizar herramientas como Postman o Swagger UI. Asegúrese de incluir un token JWT válido con el rol "Admin" en el encabezado de autorización.

## Consideraciones para Futuras Mejoras

1. **Paginación**: Implementar paginación para el endpoint que lista todos los usuarios.
2. **Filtrado**: Agregar capacidades de filtrado para buscar usuarios por diferentes criterios.
3. **Ordenamiento**: Permitir ordenar los resultados por diferentes campos.
4. **Exportación**: Agregar funcionalidad para exportar la lista de usuarios a diferentes formatos (CSV, Excel).
5. **Historial de Cambios**: Implementar un sistema para registrar un historial completo de cambios en los usuarios.

## Referencias

- [Documentación de Entity Framework Core](https://docs.microsoft.com/es-es/ef/core/)
- [Documentación de ASP.NET Core](https://docs.microsoft.com/es-es/aspnet/core/?view=aspnetcore-8.0)
- [Documentación de Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
