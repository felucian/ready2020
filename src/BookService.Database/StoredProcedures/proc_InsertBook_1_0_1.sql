CREATE PROCEDURE [dbo].[proc_InsertBook_1_0_1]
	@Title nvarchar(50),
	@Author nvarchar(50),
	@Year smallint,
	@Price money,
	@Id int output,
	@Pages smallint,
	@IsValidForGift bit
AS
	INSERT INTO Books (Title,Author,[Year],Price,Pages,IsValidForGift) 
				VALUES (@Title,@Author,@Year,@Price,@Pages,@IsValidForGift)
	SELECT @Id = SCOPE_IDENTITY()

RETURN 0
