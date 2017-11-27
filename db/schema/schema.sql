CREATE DATABASE [$(DatabaseName)]

GO

USE [$(DatabaseName)]

-- Create Users
CREATE TABLE [Users] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [Password] NVARCHAR(2048) NOT NULL
)

-- Create Accounts
CREATE TABLE [Accounts] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [UserID] INT NOT NULL,
    [Name] NVARCHAR(64) NOT NULL,
    [StartingBalance] DECIMAL(18,2) NOT NULL,
    [IsMainAccount] BIT NOT NULL,
    [IsIncludedInNetWorth] BIT NOT NULL,
    [DisplayOrder] INT NOT NULL
)

-- Create Categories
CREATE TABLE [Categories] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [AccountID] INT NOT NULL,
    [Name] NVARCHAR(64) NOT NULL,
    [DisplayOrder] INT NOT NULL
)

-- Create Parties
CREATE TABLE [Parties] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [AccountID] INT NOT NULL,
    [Name] NVARCHAR(64) NOT NULL
)

-- Create MonthlyBudgets
CREATE TABLE [MonthlyBudgets] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [AccountID] INT NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL
)

-- Create Entries
CREATE TABLE [Entries] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [AccountID] INT NOT NULL,
    [MonthlyBudgetID] INT NULL,
    [CategoryID] INT NULL,
    [PartyID] INT NULL,
    [Date] DATETIME NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Note] NVARCHAR(64) NULL,
    [TransferGUID] UNIQUEIDENTIFIER NULL
)

-- Create Categories_MonthlyBudgets
CREATE TABLE [Categories_MonthlyBudgets] (
    [MonthlyBudgetID] INT NOT NULL,
    [CategoryID] INT NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL
)

GO

PRINT 'Tables Created'

-- Create MonthlyBudgets indexes
CREATE INDEX IX_MonthlyBudgets_StartDate_EndDate ON [MonthlyBudgets] ([StartDate], [EndDate])
CREATE INDEX IX_MonthlyBudgets_EndDate_StartDate ON [MonthlyBudgets] ([EndDate], [StartDate])

GO

PRINT 'Indexes Created'

-- Create Users constraints
ALTER TABLE [Users] WITH CHECK ADD CONSTRAINT PK_Users
PRIMARY KEY ([ID])

-- Create Accounts constraints
ALTER TABLE [Accounts] WITH CHECK ADD CONSTRAINT PK_Accounts
PRIMARY KEY ([ID])
ALTER TABLE [Accounts] WITH CHECK ADD CONSTRAINT FK_Accounts_UserID_Users_ID
FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])

-- Create Categories constraints
ALTER TABLE [Categories] WITH CHECK ADD CONSTRAINT PK_Categories
PRIMARY KEY ([ID])
ALTER TABLE [Categories] WITH CHECK ADD CONSTRAINT FK_Categories_AccountID_Accounts_ID
FOREIGN KEY ([AccountID]) REFERENCES [Accounts] ([ID])

-- Create Parties constraints
ALTER TABLE [Parties] WITH CHECK ADD CONSTRAINT PK_Parties
PRIMARY KEY ([ID])
ALTER TABLE [Parties] WITH CHECK ADD CONSTRAINT FK_Parties_AccountID_Accounts_ID
FOREIGN KEY ([AccountID]) REFERENCES [Accounts] ([ID])

-- Create MonthlyBudgets constraints
ALTER TABLE [MonthlyBudgets] WITH CHECK ADD CONSTRAINT PK_MonthlyBudgets
PRIMARY KEY ([ID])
ALTER TABLE [MonthlyBudgets] WITH CHECK ADD CONSTRAINT FK_MonthlyBudgets_AccountID_Accounts_ID
FOREIGN KEY ([AccountID]) REFERENCES [Accounts] ([ID])

-- Create Entries constraints
ALTER TABLE [Entries] WITH CHECK ADD CONSTRAINT PK_Entries
PRIMARY KEY ([ID])
ALTER TABLE [Entries] WITH CHECK ADD CONSTRAINT FK_Entries_AccountID_Accounts_ID
FOREIGN KEY ([AccountID]) REFERENCES [Accounts] ([ID])
ALTER TABLE [Entries] WITH CHECK ADD CONSTRAINT FK_Entries_MonthlyBudgetID_MonthlyBudgets_ID
FOREIGN KEY ([MonthlyBudgetID]) REFERENCES [MonthlyBudgets] ([ID])
ALTER TABLE [Entries] WITH CHECK ADD CONSTRAINT FK_Entries_CategoryID_Categories_ID
FOREIGN KEY ([CategoryID]) REFERENCES [Categories] ([ID])
ALTER TABLE [Entries] WITH CHECK ADD CONSTRAINT FK_Entries_PartyID_Parties_ID
FOREIGN KEY ([PartyID]) REFERENCES [Parties] ([ID])

-- Create Categories_MonthlyBudgets constraints
ALTER TABLE [Categories_MonthlyBudgets] WITH CHECK ADD CONSTRAINT PK_Categories_MonthlyBudgets
PRIMARY KEY ([MonthlyBudgetID], [CategoryID])
ALTER TABLE [Categories_MonthlyBudgets] WITH CHECK ADD CONSTRAINT FK_Categories_MonthlyBudgets_MonthlyBudgetID_MonthlyBudgets_ID
FOREIGN KEY ([MonthlyBudgetID]) REFERENCES [MonthlyBudgets] ([ID])
ALTER TABLE [Categories_MonthlyBudgets] WITH CHECK ADD CONSTRAINT FK_Categories_MonthlyBudgets_CategoryID_Categories_ID
FOREIGN KEY ([CategoryID]) REFERENCES [Categories] ([ID])

GO

PRINT 'Constraints Created'

IF OBJECT_ID('[dbo].[Dashboard]') IS NOT NULL DROP PROC [dbo].[Dashboard]

GO

CREATE PROC Dashboard 
    @UserID int
AS

    SET NOCOUNT ON 

    DECLARE @Accounts TABLE (ID INT, Name NVARCHAR(64), StartingBalance DECIMAL(18,2), CurrentBalance DECIMAL(18,2), LatestMonthlyBudgetID INT, BalanceAtStartOfMonthlyBudget DECIMAL(18,2))
    DECLARE @LatestMonthlyBudgets TABLE (ID INT, AccountID INT, StartDate DATETIME, EndDate DATETIME)
    DECLARE @Entries TABLE (ID INT, AccountID INT, MonthlyBudgetID INT, CategoryID INT, PartyID INT, Amount DECIMAL(18,2))
    DECLARE @BudgetCategories TABLE (ID INT, AccountID INT, Name NVARCHAR(64), Amount DECIMAL(18,2), Spent DECIMAL(18,2), DisplayOrder INT)
    DECLARE @BudgetStartBalances TABLE (AccountID INT, Balance DECIMAL(18,2))

    -- Populate the accounts table for this user
    INSERT INTO 
        @Accounts
    SELECT 
        a.ID,
        a.Name,
        a.StartingBalance,
        a.StartingBalance + ISNULL(SUM(e.Amount), 0),
        0,
        0
    FROM   
        Accounts a
    LEFT JOIN
        Entries e ON e.AccountID = a.ID
    WHERE  
        a.UserID = @UserID
    GROUP BY
        a.ID,
        a.Name,
        a.StartingBalance,
        a.DisplayOrder
    ORDER BY
        a.DisplayOrder

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
        (b1.EndDate < b2.EndDate OR (b1.EndDate = b2.EndDate AND b1.Id < b2.Id))
    WHERE 
        b2.AccountID IS NULL

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
        Entries e ON e.AccountID = a.ID AND e.Date < b.StartDate 
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
        ABS(ISNULL(SUM(e.Amount), 0)) AS Spent,
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
        @BudgetCategories 
    SELECT
        0 AS ID,
        a.ID AS AccountID,
        'Uncategorised' AS Name,
        ABS((a.BalanceAtStartOfMonthlyBudget +
        ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.Amount > 0), 0)) -
        ISNULL((SELECT SUM(bc.Amount) FROM @BudgetCategories bc WHERE bc.AccountID = a.ID), 0)) AS Amount,
        ABS(ISNULL((SELECT SUM(e.Amount) FROM @Entries e WHERE e.AccountID = a.ID AND e.CategoryID IS NULL AND e.Amount < 0), 0)) AS Spent,
        1000000 AS DisplayOrder
    FROM
        @Accounts a
    GROUP BY
        a.ID,
        a.CurrentBalance,
        a.BalanceAtStartOfMonthlyBudget,
        a.LatestMonthlyBudgetID
        
    SELECT * FROM @Accounts
    SELECT *, Amount - Spent AS Remaining FROM @BudgetCategories ORDER BY AccountID, DisplayOrder

GO

-- EXEC Dashboard 1

PRINT 'Procedures Created'
