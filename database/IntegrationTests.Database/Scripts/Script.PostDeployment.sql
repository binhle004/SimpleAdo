DECLARE @Version int
SET @Version = 3

IF (EXISTS (SELECT TOP 1 1 FROM dbo.Settings WHERE Name = 'Version'))
BEGIN
	UPDATE dbo.Settings
	SET Value = @Version
	WHERE Name = 'Version'
END
ELSE
BEGIN 
	INSERT dbo.Settings (Name, Value)
	VALUES ('Version', @Version)
END;


IF NOT EXISTS (SELECT 1 FROM dbo.Product)
BEGIN 
	INSERT INTO dbo.Product (Name) VALUES ('Widget 1') , ('Widget 2'), ('Widget 3')
END;
