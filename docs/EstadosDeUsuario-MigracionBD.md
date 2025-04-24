# Documento de Migración: Estados de Usuario

## Resumen de Cambios

Este documento describe los cambios realizados en la base de datos para implementar la nueva funcionalidad de estados de usuario en el sistema AuthSystem. Se ha agregado un nuevo campo `UserStatus` a la tabla `Users` para permitir una gestión más granular de los estados de usuario.

## Cambios en el Esquema

### Tabla `Users`

Se ha agregado una nueva columna:

```sql
ALTER TABLE Users ADD UserStatus INT NOT NULL DEFAULT 1;
```

Donde los valores posibles son:
- 1: Active (Activo)
- 2: Inactive (Inactivo)
- 3: Locked (Bloqueado)
- 4: Suspended (Suspendido)
- 5: Deleted (Eliminado)

### Migración de Datos Existentes

Para los datos existentes, se ha aplicado la siguiente regla de migración:

```sql
UPDATE Users
SET UserStatus = CASE 
    WHEN IsActive = 1 THEN 1 -- Active
    ELSE 2 -- Inactive
END;
```

## Relación con Campo Existente

Se ha mantenido el campo `IsActive` por razones de compatibilidad con el código existente. Este campo ahora se calcula a partir del valor de `UserStatus`:

- `IsActive = true` cuando `UserStatus = 1` (Active)
- `IsActive = false` para cualquier otro valor de `UserStatus`

## Script de Migración Completo

```sql
-- Agregar la columna UserStatus a la tabla Users
ALTER TABLE Users ADD UserStatus INT NOT NULL DEFAULT 1;

-- Actualizar los valores de UserStatus basados en IsActive
UPDATE Users
SET UserStatus = CASE 
    WHEN IsActive = 1 THEN 1 -- Active
    ELSE 2 -- Inactive
END;

-- Crear un índice para mejorar el rendimiento de las consultas por estado
CREATE INDEX IX_Users_UserStatus ON Users(UserStatus);
```

## Verificación de la Migración

Para verificar que la migración se ha aplicado correctamente, ejecute la siguiente consulta:

```sql
SELECT 
    UserStatus,
    COUNT(*) as UserCount,
    CASE 
        WHEN UserStatus = 1 THEN 'Active'
        WHEN UserStatus = 2 THEN 'Inactive'
        WHEN UserStatus = 3 THEN 'Locked'
        WHEN UserStatus = 4 THEN 'Suspended'
        WHEN UserStatus = 5 THEN 'Deleted'
        ELSE 'Unknown'
    END as StatusName
FROM Users
GROUP BY UserStatus
ORDER BY UserStatus;
```

## Rollback

En caso de que sea necesario revertir los cambios, ejecute el siguiente script:

```sql
-- Eliminar el índice
DROP INDEX IF EXISTS IX_Users_UserStatus ON Users;

-- Eliminar la columna UserStatus
ALTER TABLE Users DROP COLUMN UserStatus;
```

## Impacto en el Rendimiento

La adición de esta columna y su índice correspondiente tiene un impacto mínimo en el rendimiento de la base de datos. Las consultas que antes filtraban por `IsActive` ahora filtrarán por `UserStatus`, con un rendimiento similar.

## Consideraciones para Entornos de Producción

Al aplicar esta migración en entornos de producción, tenga en cuenta lo siguiente:

1. **Ventana de Mantenimiento**: Programe la migración durante una ventana de mantenimiento planificada.
2. **Respaldo**: Realice un respaldo completo de la base de datos antes de aplicar los cambios.
3. **Aplicación Gradual**: Considere aplicar los cambios primero en un entorno de prueba antes de aplicarlos en producción.
4. **Monitoreo**: Monitoree el rendimiento de la base de datos después de aplicar los cambios para detectar cualquier problema.

## Dependencias

Esta migración debe aplicarse antes de desplegar la nueva versión de la aplicación que utiliza el campo `UserStatus`.

## Contacto

Para cualquier consulta relacionada con esta migración, contacte al equipo de desarrollo a través de desarrollo@example.com.
