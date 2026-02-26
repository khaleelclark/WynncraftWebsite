IF NOT EXISTS (
    SELECT 1 FROM sys.server_principals WHERE name = N'wynncraft_app'
)
BEGIN
    CREATE LOGIN [wynncraft_app]
        WITH PASSWORD = 'AVBKxuwXPSjI/q8/Kk/JjD7J17GU20cl9pmLlDrcEe0=';
END
ELSE
BEGIN
    ALTER LOGIN [wynncraft_app]
        WITH PASSWORD = 'AVBKxuwXPSjI/q8/Kk/JjD7J17GU20cl9pmLlDrcEe0=';
END
GO

USE [WynncraftDB];
GO

-- Create database user
IF NOT EXISTS (
    SELECT 1 FROM sys.database_principals WHERE name = N'wynncraft_app'
)
BEGIN
    CREATE USER [wynncraft_app]
        FOR LOGIN [wynncraft_app];
END
GO

-- Grant least-privilege access
ALTER ROLE db_datareader ADD MEMBER [wynncraft_app];
ALTER ROLE db_datawriter ADD MEMBER [wynncraft_app];
GO
