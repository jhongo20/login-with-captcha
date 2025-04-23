-- Verificar la estructura actual de la tabla Routes
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable',
    c.is_identity AS 'IsIdentity',
    ISNULL(d.definition, '') AS 'DefaultValue'
FROM 
    sys.columns c
INNER JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
LEFT JOIN 
    sys.default_constraints d ON c.default_object_id = d.object_id
WHERE 
    c.object_id = OBJECT_ID('Routes');

-- Verificar si la columna IsActive existe y si permite nulos
IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('Routes') 
    AND name = 'IsActive' 
    AND is_nullable = 1
)
BEGIN
    -- Actualizar la columna IsActive para que no permita nulos y tenga un valor predeterminado de 1 (true)
    PRINT 'Actualizando columna IsActive para que no permita nulos y tenga valor predeterminado 1';
    
    -- Primero, actualizar cualquier valor NULL existente a 1 (true)
    UPDATE Routes SET IsActive = 1 WHERE IsActive IS NULL;
    
    -- Luego, modificar la columna para que no permita nulos
    ALTER TABLE Routes ALTER COLUMN IsActive bit NOT NULL;
    
    -- Finalmente, agregar una restricción de valor predeterminado si no existe
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.default_constraints 
        WHERE parent_object_id = OBJECT_ID('Routes') 
        AND parent_column_id = (
            SELECT column_id 
            FROM sys.columns 
            WHERE object_id = OBJECT_ID('Routes') 
            AND name = 'IsActive'
        )
    )
    BEGIN
        ALTER TABLE Routes ADD CONSTRAINT DF_Routes_IsActive DEFAULT 1 FOR IsActive;
    END
END
ELSE
BEGIN
    PRINT 'La columna IsActive ya está configurada correctamente o no existe';
END

-- Verificar nuevamente la estructura de la tabla Routes después de los cambios
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable',
    c.is_identity AS 'IsIdentity',
    ISNULL(d.definition, '') AS 'DefaultValue'
FROM 
    sys.columns c
INNER JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
LEFT JOIN 
    sys.default_constraints d ON c.default_object_id = d.object_id
WHERE 
    c.object_id = OBJECT_ID('Routes')
ORDER BY 
    c.column_id;
