ALTER PROC Dashboard
    @UserID INT
AS

    SET NOCOUNT ON

    DECLARE @Accounts TABLE (
        ID INT,
        Name NVARCHAR(64),
        Type INT,
        StartingBalance DECIMAL(18,2),
        CurrentBalance DECIMAL(18,2),
        IsIncludedInNetWorth BIT,
        IsDormant BIT,
        LatestMonthlyBudgetID INT DEFAULT 0,
        BalanceAtStartOfMonthlyBudget DECIMAL(18,2) DEFAULT 0,
        DisplayOrder INT,
        NumberLast4Digits NVARCHAR(4)
    )

    DECLARE @LatestMonthlyBudgets TABLE (
        ID INT,
        AccountID INT,
        StartDate DATETIME,
        EndDate DATETIME
    )

    DECLARE @Entries TABLE (
        ID INT,
        AccountID INT,
        MonthlyBudgetID INT,
        CategoryID INT,
        PartyID INT,
        Amount DECIMAL(18,2)
    )

    DECLARE @BudgetCategories TABLE (
        ID INT,
        AccountID INT,
        Name NVARCHAR(64),
        Amount DECIMAL(18,2),
        Spent DECIMAL(18,2),
        Remaining DECIMAL(18,2),
        DisplayOrder INT
    )

    DECLARE @UncategorisedCategories TABLE (
        ID INT,
        AccountID INT,
        Name NVARCHAR(64),
        Amount DECIMAL(18,2),
        Spent DECIMAL(18,2),
        Remaining DECIMAL(18,2),
        DisplayOrder INT
    )

    DECLARE @BudgetStartBalances TABLE (
        AccountID INT,
        Balance DECIMAL(18,2)
    )

    -- Populate the accounts table for this user
    INSERT INTO
        @Accounts
    EXEC
        AccountList @UserID = @UserID

    -- Get the ID of the latest budget for each account (if one exists)
    -- Pattern demonstrated here: https://stackoverflow.com/a/8749095/43140
    INSERT INTO
        @LatestMonthlyBudgets
    SELECT
        b1.ID,
        b1.AccountID,
        b1.StartDate,
        b1.EndDate
    FROM
        MonthlyBudgets AS b1
    LEFT OUTER JOIN
        MonthlyBudgets AS b2
    ON
        b1.AccountID = b2.AccountID
    AND
        (b1.EndDate < b2.EndDate OR (b1.EndDate = b2.EndDate AND b1.ID < b2.ID))
    WHERE
        b2.AccountID IS NULL
    AND
        b1.EndDate >= GETDATE()

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
        Entries e ON e.AccountID = a.ID AND (e.Date <= b.StartDate AND e.MonthlyBudgetID != b.ID)
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
        bc.Amount - ISNULL(SUM(e.Amount), 0) AS Remaining,
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
        'Available' AS Name,
        (a.BalanceAtStartOfMonthlyBudget +
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.Amount > 0), 0) +
        ISNULL((SELECT SUM(bc.Amount) FROM @BudgetCategories bc WHERE bc.AccountID = a.ID), 0)) * -1 AS Amount,
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.CategoryID IS NULL AND e.Amount < 0), 0) AS Spent,
        0 AS Remaining,
        1000000 AS DisplayOrder
    FROM
        @Accounts a
    GROUP BY
        a.ID,
        a.CurrentBalance,
        a.BalanceAtStartOfMonthlyBudget,
        a.LatestMonthlyBudgetID

    UPDATE
        ucc
    SET
        ucc.Remaining = ((ucc.Amount - ucc.Spent) + ISNULL((SELECT SUM(e.Remaining) FROM @BudgetCategories e WHERE e.AccountID = ucc.AccountID AND e.Remaining > 0), 0))
    FROM
        @UncategorisedCategories ucc

    INSERT INTO
        @BudgetCategories
    SELECT
        *
    FROM
        @UncategorisedCategories
    WHERE
        Amount < 0

    SELECT * FROM @Accounts WHERE CurrentBalance != 0 ORDER BY DisplayOrder
    SELECT * FROM @Accounts ORDER BY DisplayOrder
    SELECT * FROM @BudgetCategories ORDER BY AccountID, DisplayOrder

GO


ALTER PROC DashboardAccount
    @UserID INT,
    @AccountID INT
AS

    SET NOCOUNT ON

    DECLARE @Accounts TABLE (
        ID INT,
        Name NVARCHAR(64),
        Type INT,
        StartingBalance DECIMAL(18,2),
        CurrentBalance DECIMAL(18,2),
        IsIncludedInNetWorth BIT,
        IsDormant BIT,
        LatestMonthlyBudgetID INT DEFAULT 0,
        BalanceAtStartOfMonthlyBudget DECIMAL(18,2) DEFAULT 0,
        DisplayOrder INT,
        NumberLast4Digits NVARCHAR(4)
    )

    DECLARE @LatestMonthlyBudgets TABLE (
        ID INT,
        AccountID INT,
        StartDate DATETIME,
        EndDate DATETIME
    )

    DECLARE @Entries TABLE (
        ID INT,
        AccountID INT,
        MonthlyBudgetID INT,
        CategoryID INT,
        PartyID INT,
        Amount DECIMAL(18,2)
    )

    DECLARE @BudgetCategories TABLE (
        ID INT,
        AccountID INT,
        Name NVARCHAR(64),
        Amount DECIMAL(18,2),
        Spent DECIMAL(18,2),
        Remaining DECIMAL(18,2),
        DisplayOrder INT
    )

    DECLARE @UncategorisedCategories TABLE (
        ID INT,
        AccountID INT,
        Name NVARCHAR(64),
        Amount DECIMAL(18,2),
        Spent DECIMAL(18,2),
        Remaining DECIMAL(18,2),
        DisplayOrder INT
    )

    DECLARE @BudgetStartBalances TABLE (
        AccountID INT,
        Balance DECIMAL(18,2)
    )

    -- Populate the accounts table for this user
    INSERT INTO
        @Accounts
    EXEC
        AccountList @UserID = @UserID, @AccountID = @AccountID

    -- Get the ID of the latest budget for this account
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
        Entries e ON e.AccountID = a.ID AND (e.Date <= b.StartDate AND e.MonthlyBudgetID != b.ID)
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
        bc.Amount - ISNULL(SUM(e.Amount), 0) AS Remaining,
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
        'Available' AS Name,
        (a.BalanceAtStartOfMonthlyBudget +
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.Amount > 0), 0) +
        ISNULL((SELECT SUM(bc.Amount) FROM @BudgetCategories bc WHERE bc.AccountID = a.ID), 0)) * -1 AS Amount,
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.CategoryID IS NULL AND e.Amount < 0), 0) AS Spent,
        0 AS Remaining,
        1000000 AS DisplayOrder
    FROM
        @Accounts a
    GROUP BY
        a.ID,
        a.CurrentBalance,
        a.BalanceAtStartOfMonthlyBudget,
        a.LatestMonthlyBudgetID

    UPDATE
        ucc
    SET
        ucc.Remaining = ((ucc.Amount - ucc.Spent) + ISNULL((SELECT SUM(e.Remaining) FROM @BudgetCategories e WHERE e.AccountID = ucc.AccountID AND e.Remaining > 0), 0))
    FROM
        @UncategorisedCategories ucc

    INSERT INTO
        @BudgetCategories
    SELECT
        *
    FROM
        @UncategorisedCategories
    WHERE
        Amount < 0

    SELECT * FROM @Accounts ORDER BY DisplayOrder
    SELECT * FROM @BudgetCategories ORDER BY AccountID, DisplayOrder

GO

