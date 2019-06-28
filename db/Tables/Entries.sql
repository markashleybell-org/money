CREATE TABLE [dbo].[Entries] (
    [ID]              INT              IDENTITY (1, 1) NOT NULL,
    [AccountID]       INT              NOT NULL,
    [MonthlyBudgetID] INT              NULL,
    [CategoryID]      INT              NULL,
    [PartyID]         INT              NULL,
    [Date]            DATETIME         NOT NULL,
    [Amount]          DECIMAL (18, 2)  NOT NULL,
    [Note]            NVARCHAR (64)    NULL,
    [TransferGUID]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Entries] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Entries_AccountID_Accounts_ID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([ID]),
    CONSTRAINT [FK_Entries_CategoryID_Categories_ID] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([ID]),
    CONSTRAINT [FK_Entries_MonthlyBudgetID_MonthlyBudgets_ID] FOREIGN KEY ([MonthlyBudgetID]) REFERENCES [dbo].[MonthlyBudgets] ([ID]),
    CONSTRAINT [FK_Entries_PartyID_Parties_ID] FOREIGN KEY ([PartyID]) REFERENCES [dbo].[Parties] ([ID])
);

