CREATE TABLE [dbo].[ItemType] (
    [ItemTypeID]   INT           NOT NULL,
    [ItemTypeName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ItemType] PRIMARY KEY CLUSTERED ([ItemTypeID] ASC)
);

