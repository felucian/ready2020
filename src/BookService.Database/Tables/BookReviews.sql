CREATE TABLE [dbo].[BookReviews]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BookId] INT NOT NULL, 
    [Author] NVARCHAR(50) NOT NULL, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(200) NOT NULL, 
    [Rating] TINYINT NOT NULL, 
    CONSTRAINT [FK_BookReviews_ToTable] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id])
)
