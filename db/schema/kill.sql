USE [master]

GO

IF EXISTS (SELECT * FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
BEGIN
    DECLARE @kill varchar(8000) = '';
    SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'
    FROM master..sysprocesses 
    WHERE dbid = db_id('$(DatabaseName)')

    EXEC(@kill)

    DROP DATABASE $(DatabaseName)
END

GO
