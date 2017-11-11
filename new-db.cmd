@ECHO OFF

SET SQLCMDPATH=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn\SQLCMD.EXE

lprun tools/generate-data-layer.linq
"%SQLCMDPATH%" -b -i db\schema\kill.sql -v DatabaseName="money"
"%SQLCMDPATH%" -b -i db\schema\schema.sql -v DatabaseName="money"
"%SQLCMDPATH%" -b -i db\schema\seed.sql -v DatabaseName="money"