CREATE TABLE [dbo].[MenuItemIngredients] (
    [MenuItemID]   INT NOT NULL,
    [IngredientID] INT NOT NULL,
    CONSTRAINT [PK_MenuItemIngredients] PRIMARY KEY CLUSTERED ([MenuItemID] ASC),
    CONSTRAINT [FK_MenuItemIngredients_Ingredients] FOREIGN KEY ([IngredientID]) REFERENCES [dbo].[Ingredients] ([IngredientID]) ON DELETE CASCADE ON UPDATE CASCADE
);

