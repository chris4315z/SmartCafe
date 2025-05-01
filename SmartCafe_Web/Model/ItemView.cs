using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class ItemView
    {
        // This is the item view model
        [Display(Name = "Item ID")]
        public int MenuItemID { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Item Image")]
        public string ItemImage { get; set; }
        public decimal Price { get; set; }

        [Display(Name = "Item Type")]
        public int ItemTypeID { get; set; }

        [Display(Name = "Item Type")]
        public string ItemTypeName { get; set; }  // Add this property

        [Display(Name = "Ingredient Name")]
        public string IngredientName { get; set; }

        [Display(Name = "Item Ingredients")]
        public List<string> MenuItemIngredients { get; set; } = new List<string>();
    }
}
