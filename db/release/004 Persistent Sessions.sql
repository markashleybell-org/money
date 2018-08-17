﻿IF OBJECT_ID('[dbo].[PersistentSessions]') IS NULL
BEGIN

    -- Create PersistentSessions
    CREATE TABLE [PersistentSessions] (
        [UserID] INT NOT NULL,
        [SeriesIdentifier] NVARCHAR(256) NOT NULL,
        [Token] NVARCHAR(256) NOT NULL,
        [Created] DATETIME NOT NULL,
        [Expires] DATETIME NOT NULL
    )

    ALTER TABLE [PersistentSessions] WITH CHECK ADD CONSTRAINT FK_PersistentSessions_UserID_Users_ID
    FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])

END
GO