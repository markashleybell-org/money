CREATE TABLE [dbo].[PersistentSessions] (
    [UserID]           INT            NOT NULL,
    [SeriesIdentifier] NVARCHAR (256) NOT NULL,
    [Token]            NVARCHAR (256) NOT NULL,
    [Created]          DATETIME       NOT NULL,
    [Expires]          DATETIME       NOT NULL,
    CONSTRAINT [FK_PersistentSessions_UserID_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

