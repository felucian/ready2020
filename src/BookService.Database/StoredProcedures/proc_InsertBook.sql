CREATE PROCEDURE [dbo].[proc_InsertBook]
	@Title nvarchar(50),
	@Author nvarchar(50),
	@Year smallint,
	@Price money,
	@Id int output
AS
	INSERT INTO Books (Title,Author,[Year],Price) VALUES (@Title,@Author,@Year,@Price)
	SELECT @Id = SCOPE_IDENTITY()

RETURN 0
