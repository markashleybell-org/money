ALTER TABLE [dbo].[Categories]
ADD [Deleted] BIT NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Parties]
ADD [Deleted] BIT NOT NULL DEFAULT 0
GO
