CREATE TABLE [dbo].[MenuItem] (
    [MenuItemID] INT             NOT NULL,
    [ItemName]   NVARCHAR (50)   NOT NULL,
    [ItemImage]  NVARCHAR (10)   NOT NULL,
    [Price]      DECIMAL (10, 2) NOT NULL,
    [ItemTypeID] INT             NOT NULL,
    CONSTRAINT [PK_MenuItems] PRIMARY KEY CLUSTERED ([MenuItemID] ASC),
    CONSTRAINT [FK_MenuItems_ItemType] FOREIGN KEY ([ItemTypeID]) REFERENCES [dbo].[ItemType] ([ItemTypeID]) ON DELETE CASCADE ON UPDATE CASCADE
);

