CREATE TABLE [dbo].[MonthlyBudgets] (
    [ID]        INT      IDENTITY (1, 1) NOT NULL,
    [AccountID] INT      NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate]   DATETIME NOT NULL,
    CONSTRAINT [PK_MonthlyBudgets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MonthlyBudgets_AccountID_Accounts_ID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_MonthlyBudgets_StartDate_EndDate]
    ON [dbo].[MonthlyBudgets]([StartDate] ASC, [EndDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MonthlyBudgets_EndDate_StartDate]
    ON [dbo].[MonthlyBudgets]([EndDate] ASC, [StartDate] ASC);

