CREATE TABLE [dbo].[Users] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [FirstName]   NVARCHAR (30)  NOT NULL,
    [LastName]    NVARCHAR (30)  NOT NULL,
    [CCNo]        NVARCHAR (MAX) NOT NULL,
    [CCExpiry]    NVARCHAR (MAX) NOT NULL,
    [CCCVV]       NVARCHAR (MAX) NOT NULL,
    [Email]       NVARCHAR (320)  NOT NULL,
    [PasswordHash]    NVARCHAR (MAX) NOT NULL,
    [PasswordSalt] NVARCHAR(MAX) NOT NULL,
    [DateOfBirth] DATE NOT NULL, 
    [IV] NVARCHAR(MAX) NOT NULL, 
    [Key] NVARCHAR(MAX) NOT NULL
);