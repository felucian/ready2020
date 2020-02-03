/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

MERGE INTO DatabaseVersion AS Target 
USING (VALUES (1,N'1.0.1'))
AS Source (Id, [SchemaVersion])
ON TARGET.Id = Source.Id
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, [SchemaVersion])
VALUES (Id, [SchemaVersion])
--update if version already exists
WHEN MATCHED THEN 
UPDATE SET [SchemaVersion] = Source.[SchemaVersion];


MERGE INTO Books AS Target 
USING (VALUES 
  (1, N'W. Shakespeare',N'Romeo and Juliet',1596,12), 
  (2, N'I. Asimov',N'I Robot',1950,16), 
  (3, N'J. Tolkien',N'Lord of the rings',1955,12.5), 
  (4, N'D. Brown',N'The da vinci code',2003,13) 
) 
AS Source (Id, Author, Title, [Year], Price) 
ON Target.Id = Source.Id
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Author, Title, [Year], Price)
VALUES (Author, Title, [Year], Price)
--delete non existing rows
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;


MERGE INTO BookReviews AS Target 
USING (VALUES 
	(1, 1, N'Jon Doe',N'An extremely entertaining play by Shakespeare. The slapstick humour is refreshing!',N'Thrilling',5), 
	(2,1,N'Mark White',N'Absolutely fun and entertaining. The play lacks thematic depth when compared to other plays by Shakespeare', N'Thrilling',5 ),
	(3,2,N'Mark White',N'It was fun to read Asimov''s description of machine learning from the 40s/50s', N'Must Read',4 ),
	(4,2,N'Edward Johnson',N'It is a relatively short book and will keep you entertained the entire time', N'Classic SciFi',5 ),
	(5,3,N'Mark White',N'I felt that this book was beautiful, Tolkien’s imagination is legendary.', N'Legendary',5 ),
	(6,3,N'Joe Green',N'Tolkien really creates his world from below the ground up', N'Amazing',5 ),
	(7,4,N'Mark Thompson',N'A fanciful tale of secret societies, secret codes, and espionage that keeps you interested until the end', N'Fanciful Tale',5 ),
	(8,4,'Henry Kyle','This book is suspenseful, thought provoking, and above all extremely entertaining', 'Great Read',5 )
) 
AS Source (Id, BookId, Author, [Description], Title, Rating) 
ON Target.Id = Source.Id
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (BookId, Author, [Description], Title, Rating)
VALUES (BookId, Author, [Description], Title, Rating)
--delete non existing rows
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;


