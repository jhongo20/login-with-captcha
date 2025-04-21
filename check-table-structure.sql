-- Consultar la estructura de la tabla UserSessions
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'UserSessions'
ORDER BY 
    ORDINAL_POSITION;

-- Consultar registros existentes
SELECT TOP 10 * FROM UserSessions;
