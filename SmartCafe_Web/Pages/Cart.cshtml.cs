using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartCafe_Web.Model;
using System.Text.Json;
using System.Linq;

namespace SmartCafe_Web.Pages
{
    public class CartModel : PageModel
    {
        // Property to hold items currently in the cart
        public List<ItemView> CartItems { get; set; } = new List<ItemView>();

        public void OnGet()
        {
            // Retrieve the cart from session storage if it exists
            var cartJson = HttpContext.Session.GetString("Cart");

            // Deserialize the JSON string back into a list of ItemView objects
            if (!string.IsNullOrEmpty(cartJson))
            {
                CartItems = JsonSerializer.Deserialize<List<ItemView>>(cartJson) ?? new List<ItemView>();
            }
        }

        public IActionResult OnPostRemove(int id)
        {
            // Retrieve the cart from session
            var cartJson = HttpContext.Session.GetString("Cart");

            // If no cart exists in session, start with an empty list; otherwise, deserialize it
            List<ItemView> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<ItemView>()
                : JsonSerializer.Deserialize<List<ItemView>>(cartJson);

            // Find the item to remove by MenuItemID
            var itemToRemove = cartItems.FirstOrDefault(item => item.MenuItemID == id);

            // If item was found, remove it from the list
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }

            // Save the updated cart back into session storage
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));

            // Refresh the cart page to reflect the updated state
            return RedirectToPage("/Cart");
        }
    }
}
