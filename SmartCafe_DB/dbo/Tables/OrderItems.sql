CREATE TABLE [dbo].[OrderItems] (
    [OrderItems] INT             NOT NULL,
    [OrderID]    INT             NULL,
    [MenuItemID] INT             NULL,
    [SizeID]     INT             NULL,
    [Quantity]   INT             NULL,
    [Subtotal]   DECIMAL (10, 2) NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([OrderItems] ASC),
    CONSTRAINT [FK_OrderItems_MenuItemSizes] FOREIGN KEY ([SizeID]) REFERENCES [dbo].[MenuItemSizes] ([SizeID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_OrderItems_OrderItems] FOREIGN KEY ([MenuItemID]) REFERENCES [dbo].[MenuItem] ([MenuItemID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE ON UPDATE CASCADE
);

