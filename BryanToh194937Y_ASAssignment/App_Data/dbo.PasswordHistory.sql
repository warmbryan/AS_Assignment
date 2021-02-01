CREATE TABLE [dbo].[PasswordHistory] (
    [Id]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Hash]      VARCHAR (MAX)    NOT NULL,
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn] DATETIME         DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [cPHUserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

