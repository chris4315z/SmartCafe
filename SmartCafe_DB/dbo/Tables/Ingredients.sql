CREATE TABLE [dbo].[Ingredients] (
    [IngredientID]   INT            NOT NULL,
    [IngredientName] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Ingredients] PRIMARY KEY CLUSTERED ([IngredientID] ASC)
);

