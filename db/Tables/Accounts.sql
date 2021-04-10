CREATE TABLE [dbo].[Accounts] (
    [ID]                   INT             IDENTITY (1, 1) NOT NULL,
    [UserID]               INT             NOT NULL,
    [Name]                 NVARCHAR (64)   NOT NULL,
    [Type]                 INT             NOT NULL,
    [StartingBalance]      DECIMAL (18, 2) NOT NULL,
    [IsMainAccount]        BIT             NOT NULL,
    [IsIncludedInNetWorth] BIT             NOT NULL,
    [IsDormant]            BIT             NOT NULL,
    [DisplayOrder]         INT             NOT NULL,
    [NumberLast4Digits]    NVARCHAR (4)    NULL,
    [Deleted]              BIT             NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Accounts_UserID_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

