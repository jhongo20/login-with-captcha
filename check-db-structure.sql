-- Verificar si la tabla UserSessions existe
SELECT 
    TABLE_NAME 
FROM 
    INFORMATION_SCHEMA.TABLES 
WHERE 
    TABLE_NAME = 'UserSessions';

-- Obtener información detallada sobre la tabla UserSessions
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE, 
    COLUMN_DEFAULT 
FROM 
    INFORMATION_SCHEMA.COLUMNS 
WHERE 
    TABLE_NAME = 'UserSessions' 
ORDER BY 
    ORDINAL_POSITION;

-- Verificar si hay alguna restricción en la tabla UserSessions
SELECT 
    tc.CONSTRAINT_NAME, 
    tc.CONSTRAINT_TYPE,
    kcu.COLUMN_NAME
FROM 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE 
    tc.TABLE_NAME = 'UserSessions';
