using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SmartCafe_Web.Model
{
    public class MenuItem
    {
        // This is the menuitem model
        public int MenuItemID{ get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [Display(Name = "Item Name")]
        public String ItemName{ get; set; }

        [Display(Name = "Item Image")]
        public String ItemImage{ get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price {  get; set; }

        [Display(Name = "Item Type")]
        public int ItemTypeID { get; set; }

    }
}
