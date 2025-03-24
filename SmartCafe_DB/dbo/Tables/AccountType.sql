CREATE TABLE [dbo].[AccountType] (
    [AccountTypeID]   NVARCHAR (100) NOT NULL,
    [AccountTypeName] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([AccountTypeID] ASC)
);

