using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Text.Json;

namespace SmartCafe_Web.Pages.MenuItems
{
    [Authorize]
    [BindProperties]
    public class SearchMenuItemsModel : PageModel
    {
        public List<ItemView> MenuItem { get; set; } = new List<ItemView>();
        public bool CanManageMenuItems { get; set; }

        public void OnGet()
        {
            CanManageMenuItems = User.IsInRole("Admin");
            PopulateMenuItemList();
        }

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

        private List<string> PopulateMenuItemIngredients(int menuItemID)
        {
            List<string> items = new List<string>();
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT n.IngredientName FROM Ingredients n " +
                                 "JOIN MenuItemIngredients mn ON n.IngredientID = mn.IngredientID " +
                                 "WHERE mn.MenuItemID = @MenuItemID"; //INNER JOIN
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MenuItemID", menuItemID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        items.Add(reader.GetString(0));
                    }
                }
            }
            return items;
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!(User.IsInRole("Admin")))
            {
                // User not allowed
                return Forbid();
            }

            // delete the item from the database
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "DELETE FROM MenuItem WHERE MenuItemID = @MenuItemID";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MenuItemID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            PopulateMenuItemList();
            return Page();
        }

        // Add item to the cart
        public IActionResult OnGetAddToCart(int id)
        {
            // Retrieve the MenuItem from the database
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

            // Get current cart items from session
            var cartJson = HttpContext.Session.GetString("Cart");
            List<ItemView> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<ItemView>()
                : JsonSerializer.Deserialize<List<ItemView>>(cartJson);

            // Add item to the cart
            cartItems.Add(menuItem);

            // Save updated cart back to session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));

            // Redirect to the Cart page
            return RedirectToPage("/Cart");
        }
    }
}
