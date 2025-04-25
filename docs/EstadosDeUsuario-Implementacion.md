# Implementación de Estados de Usuario en AuthSystem

## Resumen

Este documento describe la implementación de múltiples estados de usuario en el sistema AuthSystem, reemplazando el enfoque binario anterior (activo/inactivo) por un modelo más granular que permite una gestión más precisa de los estados de usuario.

## Cambios Realizados

### 1. Creación del Enum UserStatus

Se creó un nuevo enum para representar los diferentes estados posibles de un usuario:

```csharp
public enum UserStatus
{
    Active = 1,      // Usuario activo, puede iniciar sesión
    Inactive = 2,    // Usuario inactivo, no puede iniciar sesión
    Locked = 3,      // Usuario bloqueado por intentos fallidos
    Suspended = 4,   // Usuario suspendido por un administrador
    Deleted = 5      // Usuario marcado como eliminado (eliminación lógica)
}
```

### 2. Modificación de la Entidad User

Se modificó la entidad `User` para incluir el nuevo campo `UserStatus` y mantener la compatibilidad con `IsActive`:

```csharp
public class User : BaseEntity
{
    // Campos existentes...

    public UserStatus UserStatus { get; set; } = UserStatus.Active;

    public bool IsActive 
    { 
        get => UserStatus == UserStatus.Active;
        set => UserStatus = value ? UserStatus.Active : UserStatus.Inactive;
    }

    // Otros campos...
}
```

### 3. Actualización del Repositorio de Usuarios

Se actualizaron los métodos del repositorio para utilizar el nuevo campo `UserStatus`:

```csharp
// Obtener usuarios activos
public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
{
    return await _dbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .Where(u => u.UserStatus == UserStatus.Active)
        .ToListAsync(cancellationToken);
}

// Obtener usuarios por estado
public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
{
    return await _dbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .Where(u => u.UserStatus == status)
        .ToListAsync(cancellationToken);
}

// Actualizar estado de usuario
public async Task<bool> UpdateUserStatusAsync(Guid userId, UserStatus status, CancellationToken cancellationToken = default)
{
    var user = await _dbSet.FindAsync(new object[] { userId }, cancellationToken);
    if (user == null)
    {
        return false;
    }

    user.UserStatus = status;
    user.LastModifiedAt = DateTime.UtcNow;
    
    _dbSet.Update(user);
    return true;
}

// Obtener usuario por nombre de usuario sin filtrar por estado
public async Task<User> GetByUsernameIncludingInactiveAsync(string username, CancellationToken cancellationToken = default)
{
    if (string.IsNullOrEmpty(username))
    {
        throw new ArgumentException("El nombre de usuario no puede ser nulo o vacío", nameof(username));
    }

    return await _dbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
```

### 4. Actualización del Controlador de Usuarios

Se modificó el método `UpdateUser` en el controlador para procesar el campo `UserStatus`:

```csharp
// Actualizar el estado del usuario si se especifica
if (request.UserStatus.HasValue)
{
    user.UserStatus = request.UserStatus.Value;
    
    // Registrar el cambio de estado en el log
    _logger.LogInformation($"Estado del usuario {user.Username} actualizado a {user.UserStatus}");
    
    // Enviar notificaciones según el nuevo estado
    if (user.UserStatus == UserStatus.Suspended)
    {
        await _userNotificationService.SendAccountSuspendedEmailAsync(user);
    }
    else if (user.UserStatus == UserStatus.Active)
    {
        await _userNotificationService.SendAccountActivatedEmailAsync(user);
    }
}
```

### 5. Actualización del Modelo de Solicitud

Se actualizó el modelo `UpdateUserRequest` para incluir el campo `UserStatus`:

```csharp
public class UpdateUserRequest
{
    // Otros campos...

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Estado del usuario
    /// </summary>
    public UserStatus? UserStatus { get; set; }

    // Otros campos...
}
```

### 6. Mejora en el Proceso de Autenticación

Se modificó el método `Login` en el controlador de autenticación para manejar diferentes estados de usuario:

```csharp
// Si no se encuentra el usuario, verificar si existe pero con un estado diferente a Active
if (user == null)
{
    var userRepository = _unitOfWork.Users as IUserRepository;
    if (userRepository != null)
    {
        var inactiveUser = await userRepository.GetByUsernameIncludingInactiveAsync(request.Username);
        if (inactiveUser != null)
        {
            // El usuario existe pero tiene un estado diferente a Active
            string statusMessage = inactiveUser.UserStatus switch
            {
                UserStatus.Inactive => "Su cuenta está inactiva. Por favor contacte al administrador para activarla.",
                UserStatus.Locked => "Su cuenta está bloqueada debido a múltiples intentos fallidos de inicio de sesión. Por favor contacte al administrador.",
                UserStatus.Suspended => "Su cuenta ha sido suspendida. Por favor contacte al administrador para más información.",
                UserStatus.Deleted => "Esta cuenta ha sido eliminada y ya no está disponible.",
                _ => "Su cuenta no está activa. Por favor contacte al administrador."
            };
            
            return StatusCode(403, new ErrorResponse
            {
                Message = statusMessage
            });
        }
    }
    
    return Unauthorized(new ErrorResponse
    {
        Message = "Nombre de usuario o contraseña incorrectos"
    });
}
```

## Flujo de Trabajo

### Actualización de Estado de Usuario

1. El administrador accede a la API a través de Swagger o una interfaz de usuario.
2. Utiliza el endpoint `PUT /api/Users/{id}` para actualizar los datos de un usuario.
3. Incluye el campo `UserStatus` con el valor deseado en la solicitud.
4. El sistema actualiza el estado del usuario y envía notificaciones según corresponda.

### Autenticación con Diferentes Estados

1. Un usuario intenta iniciar sesión a través del endpoint `/api/Auth/login-with-google-recaptcha`.
2. Si el usuario tiene un estado diferente a `Active`, el sistema muestra un mensaje específico:
   - **Inactive**: "Su cuenta está inactiva. Por favor contacte al administrador para activarla."
   - **Locked**: "Su cuenta está bloqueada debido a múltiples intentos fallidos de inicio de sesión. Por favor contacte al administrador."
   - **Suspended**: "Su cuenta ha sido suspendida. Por favor contacte al administrador para más información."
   - **Deleted**: "Esta cuenta ha sido eliminada y ya no está disponible."

## Beneficios

1. **Gestión más granular**: Permite a los administradores gestionar usuarios con mayor precisión.
2. **Mejor experiencia de usuario**: Proporciona mensajes específicos según el estado del usuario.
3. **Compatibilidad con código existente**: Mantiene la compatibilidad con el campo `IsActive`.
4. **Auditoría mejorada**: Facilita el seguimiento de cambios en el estado de los usuarios.

## Consideraciones Técnicas

1. **Migración de datos**: Se ha implementado una migración para agregar el campo `UserStatus` a la base de datos.
2. **Compatibilidad hacia atrás**: Se mantiene la propiedad `IsActive` para garantizar la compatibilidad con el código existente.
3. **Notificaciones**: Se envían notificaciones por correo electrónico cuando cambia el estado de un usuario.

## Próximos Pasos

1. Implementar filtros en la interfaz de usuario para mostrar usuarios por estado.
2. Agregar razones para suspensión/bloqueo.
3. Implementar mecanismos de reactivación automática para usuarios bloqueados temporalmente.
4. Mejorar las plantillas de correo electrónico para las notificaciones de cambio de estado.
