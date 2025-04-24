# Documentación Técnica: Estados de Usuario

## Resumen

Esta documentación describe la implementación de múltiples estados de usuario en el sistema AuthSystem, reemplazando el enfoque binario anterior (activo/inactivo) por un modelo más granular que permite una gestión más precisa de los estados de usuario.

## Arquitectura

La implementación sigue los principios de Clean Architecture del proyecto:

1. **Domain Layer**: Definición del enum `UserStatus` y modificación de la entidad `User`.
2. **Infrastructure Layer**: Implementación de los repositorios y servicios.
3. **API Layer**: Exposición de endpoints para gestionar los estados de usuario.

## Modelo de Datos

### Enum UserStatus

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

### Entidad User

Se ha modificado la entidad `User` para incluir el campo `UserStatus` y mantener `IsActive` como propiedad calculada:

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

## Cambios en la Base de Datos

Se ha agregado una nueva columna `UserStatus` a la tabla `Users`:

```sql
ALTER TABLE Users ADD UserStatus INT NOT NULL DEFAULT 1;

UPDATE Users
SET UserStatus = CASE 
    WHEN IsActive = 1 THEN 1 -- Active
    ELSE 2 -- Inactive
END;
```

## Repositorio

Se han actualizado los métodos del repositorio para utilizar el nuevo campo `UserStatus`:

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
```

## API Endpoints

Se ha agregado un nuevo endpoint para actualizar el estado de un usuario:

```
PATCH /api/Users/{id}/status
```

### Modelo de Solicitud

```csharp
public class UpdateUserStatusRequest
{
    [Required(ErrorMessage = "El estado del usuario es requerido")]
    public UserStatus Status { get; set; }
}
```

### Respuesta

```json
{
  "message": "Estado del usuario actualizado correctamente a Active"
}
```

## Notificaciones

Se han implementado notificaciones por correo electrónico para informar a los usuarios sobre cambios en su estado:

- `SendAccountSuspendedEmailAsync`: Cuando un usuario es suspendido.
- `SendAccountActivatedEmailAsync`: Cuando un usuario es activado.

## Compatibilidad con Código Existente

Se ha mantenido la propiedad `IsActive` para garantizar la compatibilidad con el código existente. Esta propiedad ahora se calcula basándose en `UserStatus`:

- `IsActive` es `true` cuando `UserStatus` es `UserStatus.Active`.
- `IsActive` es `false` para cualquier otro estado.

## Consideraciones de Seguridad

- Solo los administradores pueden cambiar el estado de un usuario.
- Los usuarios bloqueados o suspendidos no pueden iniciar sesión.
- Los usuarios marcados como eliminados no aparecen en las consultas regulares.

## Ejemplos de Uso

### Obtener Usuarios por Estado

```csharp
var suspendedUsers = await _userRepository.GetByStatusAsync(UserStatus.Suspended);
```

### Actualizar Estado de Usuario

```csharp
await _userRepository.UpdateUserStatusAsync(userId, UserStatus.Locked);
await _unitOfWork.SaveChangesAsync();
```

## Próximos Pasos

1. Implementar filtros en la interfaz de usuario para mostrar usuarios por estado.
2. Agregar razones para suspensión/bloqueo.
3. Implementar mecanismos de reactivación automática para usuarios bloqueados temporalmente.
