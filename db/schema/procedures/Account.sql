IF OBJECT_ID('[dbo].[Account]') IS NOT NULL DROP PROC [dbo].[Account]

GO

CREATE PROC Account 
    @AccountID int
AS

    SET NOCOUNT ON 

    DECLARE @Accounts TABLE (ID INT, Name NVARCHAR(64), Type INT, StartingBalance DECIMAL(18,2), CurrentBalance DECIMAL(18,2), IsIncludedInNetWorth BIT, IsDormant BIT, LatestMonthlyBudgetID INT, BalanceAtStartOfMonthlyBudget DECIMAL(18,2))
    DECLARE @LatestMonthlyBudgets TABLE (ID INT, AccountID INT, StartDate DATETIME, EndDate DATETIME)
    DECLARE @Entries TABLE (ID INT, AccountID INT, MonthlyBudgetID INT, CategoryID INT, PartyID INT, Amount DECIMAL(18,2))
    DECLARE @BudgetCategories TABLE (ID INT, AccountID INT, Name NVARCHAR(64), Amount DECIMAL(18,2), Spent DECIMAL(18,2), DisplayOrder INT)
    DECLARE @UncategorisedCategories TABLE (ID INT, AccountID INT, Name NVARCHAR(64), Amount DECIMAL(18,2), Spent DECIMAL(18,2), DisplayOrder INT)
    DECLARE @BudgetStartBalances TABLE (AccountID INT, Balance DECIMAL(18,2))

    -- Populate the accounts table for this user
    INSERT INTO 
        @Accounts
    SELECT 
        a.ID,
        a.Name,
        a.Type,
        a.StartingBalance,
        a.StartingBalance + ISNULL(SUM(e.Amount), 0),
        a.IsIncludedInNetWorth,
        a.IsDormant,
        0,
        0
    FROM   
        Accounts a
    LEFT JOIN
        Entries e ON e.AccountID = a.ID
    WHERE  
        a.ID = @AccountID
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

    -- Get the ID of the latest budget for each account (if one exists)
    -- Pattern demonstrated here: https://stackoverflow.com/a/8749095/43140
    INSERT INTO
        @LatestMonthlyBudgets
    SELECT TOP 1
        b.ID,
        b.AccountID,
        b.StartDate,
        b.EndDate
    FROM 
        MonthlyBudgets AS b
    WHERE 
        b.AccountID = @AccountID 
    ORDER BY 
        b.EndDate DESC, 
        b.ID DESC

    -- Get the balance of each account at the start of this budget period
    INSERT INTO
        @BudgetStartBalances
    SELECT
        a.ID,
        a.StartingBalance + ISNULL(SUM(e.Amount), 0)
    FROM
        @Accounts a
    LEFT JOIN
        @LatestMonthlyBudgets b ON b.AccountID = a.ID
    LEFT JOIN    
        -- Note: we don't join onto @Entries because that only includes entries within the current budget
        Entries e ON e.AccountID = a.ID AND e.Date < b.StartDate AND e.MonthlyBudgetID != b.ID
    GROUP BY
        a.ID,
        a.StartingBalance

    UPDATE 
        a
    SET 
        a.LatestMonthlyBudgetID = b.ID,
        a.BalanceAtStartOfMonthlyBudget = s.Balance
    FROM 
        @Accounts a
    INNER JOIN 
        @BudgetStartBalances s ON s.AccountID = a.ID
    INNER JOIN 
        @LatestMonthlyBudgets b ON b.AccountID = a.ID

    -- Get all the entries for the current budgets
    INSERT INTO
        @Entries
    SELECT
        e.ID,
        e.AccountID,
        e.MonthlyBudgetID,
        e.CategoryID,
        e.PartyID,
        e.Amount
    FROM 
        Entries e
    INNER JOIN
        @Accounts a ON a.LatestMonthlyBudgetID = e.MonthlyBudgetID

    -- List all the categories and totals for each budget
    INSERT INTO
        @BudgetCategories
    SELECT
        c.ID,
        c.AccountID,
        c.Name,
        bc.Amount,
        ISNULL(SUM(e.Amount), 0) AS Spent,
        c.DisplayOrder
    FROM   
        @LatestMonthlyBudgets b
    INNER JOIN 
        Categories_MonthlyBudgets bc ON bc.MonthlyBudgetID = b.ID
    INNER JOIN 
        Categories c ON c.ID = bc.CategoryID
    LEFT JOIN
        @Entries e ON e.MonthlyBudgetID = b.ID AND e.CategoryID = c.ID
    GROUP BY 
        c.ID,
        c.AccountID,
        c.Name,
        bc.Amount,
        c.DisplayOrder

    INSERT INTO
        @UncategorisedCategories 
    SELECT
        0 AS ID,
        a.ID AS AccountID,
        'Uncategorised' AS Name,
        (a.BalanceAtStartOfMonthlyBudget +
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.Amount > 0), 0) +
        ISNULL((SELECT SUM(bc.Amount) FROM @BudgetCategories bc WHERE bc.AccountID = a.ID), 0)) * -1 AS Amount,
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.CategoryID IS NULL AND e.Amount < 0), 0) AS Spent,
        1000000 AS DisplayOrder
    FROM
        @Accounts a
    GROUP BY
        a.ID,
        a.CurrentBalance,
        a.BalanceAtStartOfMonthlyBudget,
        a.LatestMonthlyBudgetID

    INSERT INTO
        @BudgetCategories
    SELECT
        *
    FROM
        @UncategorisedCategories
    WHERE 
        Amount < 0
        
    SELECT * FROM @Accounts
    SELECT *, Amount - Spent AS Remaining FROM @BudgetCategories ORDER BY AccountID, DisplayOrder

GO

-- EXEC Account 7
