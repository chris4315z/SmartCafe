using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartCafe_Web.Model;
using System.Text.Json;
using System.Linq;

namespace SmartCafe_Web.Pages
{
    public class CartModel : PageModel
    {
        public List<ItemView> CartItems { get; set; } = new List<ItemView>();

        public void OnGet()
        {
            // Get the cart items from session if available
            var cartJson = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                CartItems = JsonSerializer.Deserialize<List<ItemView>>(cartJson) ?? new List<ItemView>();
            }
        }

        public IActionResult OnPostRemove(int id)
        {
            // Get the current cart items from session
            var cartJson = HttpContext.Session.GetString("Cart");
            List<ItemView> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<ItemView>()
                : JsonSerializer.Deserialize<List<ItemView>>(cartJson);

            // Find and remove the item by MenuItemID
            var itemToRemove = cartItems.FirstOrDefault(item => item.MenuItemID == id);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);  // Remove the item from the cart
            }

            // Save the updated cart back into the session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));

            // Redirect to the cart page with updated cart items
            return RedirectToPage("/Cart");
        }
    }
}
