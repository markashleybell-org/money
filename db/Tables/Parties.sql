CREATE TABLE [dbo].[Parties] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [AccountID] INT           NOT NULL,
    [Name]      NVARCHAR (64) NOT NULL,
    [Deleted]   BIT           NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Parties] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Parties_AccountID_Accounts_ID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([ID])
);

