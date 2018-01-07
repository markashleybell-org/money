IF OBJECT_ID('[dbo].[NetWorth]') IS NOT NULL DROP PROC [dbo].[NetWorth]

GO

CREATE PROC NetWorth 
    @UserID int
AS

    SET NOCOUNT ON 

    SELECT 
        a.ID,
        a.Name,
        a.Type,
        a.StartingBalance,
        a.StartingBalance + ISNULL(SUM(e.Amount), 0) AS CurrentBalance,
        a.IsIncludedInNetWorth
    FROM   
        Accounts a
    LEFT JOIN
        Entries e ON e.AccountID = a.ID
    WHERE
        a.UserID = @UserID
    GROUP BY
        a.ID,
        a.Name,
        a.Type,
        a.StartingBalance,
        a.IsIncludedInNetWorth,
        a.DisplayOrder
    ORDER BY
        a.DisplayOrder

GO

-- EXEC NetWorth 1
