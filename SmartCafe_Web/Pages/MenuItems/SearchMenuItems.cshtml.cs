using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Text.Json;

namespace SmartCafe_Web.Pages.MenuItems
{
    [Authorize] // Only authorized users can access this page
    [BindProperties]
    public class SearchMenuItemsModel : PageModel
    {
        // List to hold menu items for display
        public List<ItemView> MenuItem { get; set; } = new List<ItemView>();

        // Flag to determine if the current user has admin rights
        public bool CanManageMenuItems { get; set; }

        // Handles GET request to display the menu items
        public void OnGet()
        {
            CanManageMenuItems = User.IsInRole("Admin"); // Check if user is admin
            PopulateMenuItemList(); // Load all menu items from database
        }

        // Retrieves all menu items and their details from the database
        private void PopulateMenuItemList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID, t.ItemTypeName " +
                                 "FROM MenuItem i " +
                                 "JOIN ItemType t ON i.ItemTypeID = t.ItemTypeID";

                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Construct a menu item object with ingredient list
                        ItemView items = new ItemView
                        {
                            MenuItemID = reader.GetInt32(0),
                            ItemName = reader.GetString(1),
                            ItemImage = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ItemTypeID = reader.GetInt32(4),
                            ItemTypeName = reader.GetString(5),
                            MenuItemIngredients = PopulateMenuItemIngredients(reader.GetInt32(0))
                        };
                        MenuItem.Add(items);
                    }
                }
            }
        }

        // Gets the list of ingredients for a specific menu item
        private List<string> PopulateMenuItemIngredients(int menuItemID)
        {
            List<string> items = new List<string>();
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT n.IngredientName FROM Ingredients n " +
                                 "JOIN MenuItemIngredients mn ON n.IngredientID = mn.IngredientID " +
                                 "WHERE mn.MenuItemID = @MenuItemID";

                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MenuItemID", menuItemID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        items.Add(reader.GetString(0)); // Add ingredient name to list
                    }
                }
            }
            return items;
        }

        // Handles POST request to delete a menu item
        public IActionResult OnPostDelete(int id)
        {
            if (!(User.IsInRole("Admin")))
            {
                // Prevent deletion if user is not an admin
                return Forbid();
            }

            // Delete the specified menu item from the database
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "DELETE FROM MenuItem WHERE MenuItemID = @MenuItemID";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MenuItemID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Refresh the list after deletion
            PopulateMenuItemList();
            return Page();
        }

        // Adds the selected menu item to the user's shopping cart
        public IActionResult OnGetAddToCart(int id)
        {
            // Retrieve item details from the database
            var menuItem = new ItemView();
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID, t.ItemTypeName " +
                                 "FROM MenuItem i " +
                                 "JOIN ItemType t ON i.ItemTypeID = t.ItemTypeID " +
                                 "WHERE i.MenuItemID = @MenuItemID";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MenuItemID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Build menu item object
                    menuItem = new ItemView
                    {
                        MenuItemID = reader.GetInt32(0),
                        ItemName = reader.GetString(1),
                        ItemImage = reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        ItemTypeID = reader.GetInt32(4),
                        ItemTypeName = reader.GetString(5),
                        MenuItemIngredients = PopulateMenuItemIngredients(reader.GetInt32(0))
                    };
                }
            }

            // Retrieve the cart from session storage
            var cartJson = HttpContext.Session.GetString("Cart");
            List<ItemView> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<ItemView>()
                : JsonSerializer.Deserialize<List<ItemView>>(cartJson);

            // Add the selected item to the cart
            cartItems.Add(menuItem);

            // Save the updated cart back to the session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));

            // Redirect to the cart page
            return RedirectToPage("/Cart");
        }
    }
}
