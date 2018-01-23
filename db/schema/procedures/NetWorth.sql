IF OBJECT_ID('[dbo].[NetWorth]') IS NOT NULL DROP PROC [dbo].[NetWorth]

GO

CREATE PROC NetWorth 
    @UserID INT
AS

    SET NOCOUNT ON 

    DECLARE @Accounts TABLE (
        ID INT, 
        Name NVARCHAR(64), 
        Type INT, 
        StartingBalance DECIMAL(18,2),
        CurrentBalance DECIMAL(18,2),
        IsIncludedInNetWorth BIT
    )

    INSERT INTO @Accounts (ID, Name, Type, StartingBalance, CurrentBalance, IsIncludedInNetWorth) EXEC AccountList @UserID

    SELECT * FROM @Accounts WHERE CurrentBalance != 0
GO

-- EXEC NetWorth 1
