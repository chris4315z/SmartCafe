using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Ingredient
    {
        // This is the model for our ingredients
        public int IngredientID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Ingredient name cannot exceed 50 characters." )]
        [Display(Name = "Ingredient Name")]
        public string IngredientName { get; set; } = string.Empty;
    }
}
