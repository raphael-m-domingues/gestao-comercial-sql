USE master;
GO

IF DB_ID(N'DB_GestaoComercial') IS NULL
BEGIN
    CREATE DATABASE DB_GestaoComercial;

    PRINT 'Banco DB_GestaoComercial criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'O banco DB_GestaoComercial já existe.';
END;
GO

ALTER DATABASE DB_GestaoComercial
SET RECOVERY SIMPLE;
GO

USE DB_GestaoComercial;
GO

SELECT
    name AS NomeBanco,
    state_desc AS Estado,
    recovery_model_desc AS ModeloRecuperacao
FROM sys.databases
WHERE name = DB_NAME();
GO