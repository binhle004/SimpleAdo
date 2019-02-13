CREATE TABLE [dbo].[Product] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [Price]              DECIMAL (9, 2) CONSTRAINT [DF__Product__Price__267ABA7A] DEFAULT ((0)) NOT NULL,
    [DateModified]       DATETIME2 (7)  CONSTRAINT [DF_Product_DateModified] DEFAULT (sysdatetime()) NOT NULL,
    [ProductDescription] VARCHAR (MAX)  NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC)
);





