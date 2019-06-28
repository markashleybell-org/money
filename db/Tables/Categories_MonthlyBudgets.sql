CREATE TABLE [dbo].[Categories_MonthlyBudgets] (
    [MonthlyBudgetID] INT             NOT NULL,
    [CategoryID]      INT             NOT NULL,
    [Amount]          DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_Categories_MonthlyBudgets] PRIMARY KEY CLUSTERED ([MonthlyBudgetID] ASC, [CategoryID] ASC),
    CONSTRAINT [FK_Categories_MonthlyBudgets_CategoryID_Categories_ID] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([ID]),
    CONSTRAINT [FK_Categories_MonthlyBudgets_MonthlyBudgetID_MonthlyBudgets_ID] FOREIGN KEY ([MonthlyBudgetID]) REFERENCES [dbo].[MonthlyBudgets] ([ID])
);

