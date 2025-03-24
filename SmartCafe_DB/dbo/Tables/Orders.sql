CREATE TABLE [dbo].[Orders] (
    [OrderID]      INT             NOT NULL,
    [SystemUserID] NVARCHAR (100)  NULL,
    [OrderDate]    DATETIME        NULL,
    [TotalPrice]   DECIMAL (10, 2) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderID] ASC),
    CONSTRAINT [FK_Orders_AccountType] FOREIGN KEY ([SystemUserID]) REFERENCES [dbo].[AccountType] ([AccountTypeID]) ON DELETE CASCADE ON UPDATE CASCADE
);

