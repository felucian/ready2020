CREATE TABLE [dbo].[DatabaseVersion]
(
    [Id] INT NOT NULL, 
    [SchemaVersion] NVARCHAR(10) NOT NULL, 
    CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY ([Id])
)
