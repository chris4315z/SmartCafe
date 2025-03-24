CREATE TABLE [dbo].[SystemUser] (
    [SystemUserID]             INT             NOT NULL,
    [SystemUserFirstName]      NVARCHAR (50)   NOT NULL,
    [SystemUserLastName]       NVARCHAR (500)  NOT NULL,
    [SystemEmailAddress]       NVARCHAR (500)  NOT NULL,
    [SystemUserName]           NVARCHAR (20)   NOT NULL,
    [SystemUserPhoneNumber]    DECIMAL (10, 2) NULL,
    [SystemUserPassword]       NVARCHAR (100)  NOT NULL,
    [SystemUserBirthday]       NVARCHAR (100)  NOT NULL,
    [SystemUserProfilePicture] NVARCHAR (500)  NULL,
    [AccountTypeID]            NVARCHAR (100)  NOT NULL,
    CONSTRAINT [PK_SystemUser] PRIMARY KEY CLUSTERED ([SystemUserID] ASC),
    CONSTRAINT [FK_SystemUser_AccountType] FOREIGN KEY ([AccountTypeID]) REFERENCES [dbo].[AccountType] ([AccountTypeID]) ON DELETE CASCADE ON UPDATE CASCADE
);

