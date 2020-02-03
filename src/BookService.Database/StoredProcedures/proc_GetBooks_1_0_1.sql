CREATE PROCEDURE [dbo].[proc_GetBooks_1_0_1]
AS
	SELECT 
		[Id],
		[Title],
		[Author],
		[Year],
		[Price],
		[Pages],
		[IsValidForGift] 
	FROM Books
RETURN 0