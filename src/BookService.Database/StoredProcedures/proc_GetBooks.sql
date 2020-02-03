CREATE PROCEDURE [dbo].[proc_GetBooks]
AS
	SELECT 
		[Id],
		[Title],
		[Author],
		[Year],
		[Price]
	FROM Books
RETURN 0