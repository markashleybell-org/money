CREATE TABLE [dbo].[Categories] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [AccountID]    INT           NOT NULL,
    [Name]         NVARCHAR (64) NOT NULL,
    [DisplayOrder] INT           NOT NULL,
    [Deleted]      BIT           NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Categories_AccountID_Accounts_ID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([ID])
);

