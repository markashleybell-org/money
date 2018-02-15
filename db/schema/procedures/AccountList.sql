IF OBJECT_ID('[dbo].[AccountList]') IS NOT NULL DROP PROC [dbo].[AccountList]

GO

CREATE PROC AccountList 
    @UserID INT
AS

    SET NOCOUNT ON

    SELECT 
        a.ID,
        a.Name,
        a.Type,
        a.StartingBalance,
        a.StartingBalance + ISNULL(SUM(e.Amount), 0) AS CurrentBalance,
        a.IsIncludedInNetWorth,
        a.IsDormant
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
        a.IsDormant,
        a.DisplayOrder
    ORDER BY
        a.DisplayOrder
GO

-- EXEC AccountList 1
