CREATE PROCEDURE [dbo].[proc_GetSchemaVersion]
AS
	SELECT SchemaVersion FROM DatabaseVersion WHERE Id = 1
RETURN 0