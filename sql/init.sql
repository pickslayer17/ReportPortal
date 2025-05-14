-- 0. Создание базы, если нет
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ReportPortal')
BEGIN
    CREATE DATABASE ReportPortal;
END
GO

-- 1. Логин
IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'ReportPortalUser')
BEGIN
    CREATE LOGIN ReportPortalUser WITH PASSWORD = 'sgbn3w805tyh-hgb-9h345bg';
END
GO

-- 2. Пользователь в базе
USE ReportPortal;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ReportPortalUser')
BEGIN
    CREATE USER ReportPortalUser FOR LOGIN ReportPortalUser;
END
GO

-- 3. Права
IF NOT EXISTS (
    SELECT * FROM sys.database_role_members rm
    JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    JOIN sys.database_principals u ON rm.member_principal_id = u.principal_id
    WHERE r.name = 'db_owner' AND u.name = 'ReportPortalUser'
)
BEGIN
    ALTER ROLE db_owner ADD MEMBER ReportPortalUser;
END
GO