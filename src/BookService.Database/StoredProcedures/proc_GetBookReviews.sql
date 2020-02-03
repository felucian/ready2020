CREATE PROCEDURE [dbo].[proc_GetBookReviews]
	@BookId int
AS
	SELECT Id, BookId, Author, [Description], Title, Rating
	FROM BookReviews
	WHERE BookId = @BookId

RETURN 0
