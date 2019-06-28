CREATE TABLE [dbo].[Users] (
    [ID]       INT             IDENTITY (1, 1) NOT NULL,
    [Email]    NVARCHAR (256)  NOT NULL,
    [Password] NVARCHAR (2048) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
);

