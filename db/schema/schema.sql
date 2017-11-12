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
    [IsMainAccount] BIT NOT NULL,
    [IsIncludedInNetWorth] BIT NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [StartingBalance] DECIMAL(18,2) NOT NULL,
    [CurrentBalance] DECIMAL(18,2) NOT NULL
)

-- Create Categories
CREATE TABLE [Categories] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [AccountID] INT NOT NULL,
    [Name] NVARCHAR(64) NOT NULL
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
    [PartyID] INT NOT NULL,
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

-- Create Entries_UpdateAccountBalance trigger
IF OBJECT_ID(N'[Entries_UpdateAccountBalance]') IS NOT NULL
BEGIN
    DROP TRIGGER [Entries_UpdateAccountBalance]
END

GO

CREATE TRIGGER 
    [Entries_UpdateAccountBalance]
ON 
    [Entries]
FOR 
    INSERT, 
    UPDATE, 
    DELETE
AS
BEGIN
    
    DECLARE @AccountsWithChangedEntries TABLE (ID int)

    INSERT INTO 
        @AccountsWithChangedEntries 
    SELECT 
        [AccountID] 
    FROM 
        [DELETED] 
    UNION ALL 
    SELECT 
        [AccountID] 
    FROM 
        [INSERTED]

    UPDATE 
        [a]
    SET 
        [a].[CurrentBalance] = [a].[StartingBalance] + ISNULL((
            SELECT 
                SUM([e].[Amount]) 
            FROM 
                [Entries] [e] 
            WHERE 
                [e].[AccountID] = [a].[ID]
        ), 0) 
    FROM 
        [Accounts] [a]
    INNER JOIN
        @AccountsWithChangedEntries [ac] ON [ac].[ID] = [a].[ID]
    
END

GO

PRINT 'Triggers Created'
