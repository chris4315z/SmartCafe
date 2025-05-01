using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace SmartCafe_Web.Pages.MenuItems
{
    // Only allow access to users in the "Admin" role
    [Authorize(Roles = "Admin")]
    // Automatically binds public properties during POST requests
    [BindProperties]
    public class AddMenuItemModel : PageModel
    {
        // Model representing the new menu item to be created
        public MenuItem NewItem { get; set; }

        // Dropdown list items (not used but defined)
        public List<SelectListItem> MenuItem { get; set; } = new List<SelectListItem>();

        // Dropdown for selecting item sizes
        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        // Dropdown for selecting item types (e.g., drink, food, etc.)
        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        // List of ingredients to be shown as checkboxes or similar UI
        public List<IngredientInfo> Ingredients { get; set; } = new List<IngredientInfo>();

        // List for order items (currently unused)
        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        // Stores the IDs of ingredients selected for the new menu item
        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();

        // GET request: populate dropdowns and ingredient lists
        public void OnGet()
        {
            // PopulateMenuItemList(); // (commented out - not used)
            PopulateMenuItemSizesList();
            PopulateItemTypeList();
            PopulateIngredientsList();
        }

        // POST request: save the new menu item to the database
        public IActionResult OnPost()
        {
            // Only proceed if form inputs are valid
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    // SQL command to insert new item and return the newly generated ID
                    string cmdText = "INSERT INTO [MenuItem] (ItemName, ItemImage, Price, ItemTypeID) " +
                        "VALUES (@itemName, @itemImage, @price, @itemTypeID); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@itemName", NewItem.ItemName);
                    cmd.Parameters.AddWithValue("@itemImage", NewItem.ItemImage);
                    cmd.Parameters.AddWithValue("@price", NewItem.Price);
                    cmd.Parameters.AddWithValue("@itemTypeID", NewItem.ItemTypeID);

                    // Get current logged-in user ID from claims (if needed for audit/logging)
                    int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    conn.Open();

                    // Execute and get the new MenuItemID
                    int menuItemID = int.Parse(cmd.ExecuteScalar().ToString());

                    // Insert selected ingredient mappings for this new menu item
                    for (int i = 0; i < SelectedMenuItemIngredientsIDs.Count; i++)
                    {
                        cmdText = "INSERT INTO [MenuItemIngredients] (MenuItemID, IngredientID) VALUES (@menuItemID, @ingredientID)";
                        cmd = new SqlCommand(cmdText, conn);
                        cmd.Parameters.AddWithValue("@menuItemID", menuItemID);
                        cmd.Parameters.AddWithValue("@ingredientID", SelectedMenuItemIngredientsIDs[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
                // Redirect to home page after successful save
                return RedirectToPage("/Index");
            }
            else
            {
                // If model validation fails, re-populate dropdowns and show the form again
                PopulateMenuItemSizesList();
                PopulateItemTypeList();
                PopulateIngredientsList();
                return Page();
            }
        }

        // Helper method to load sizes from DB into dropdown
        private void PopulateMenuItemSizesList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT SizeID, SizeName FROM MenuItemSizes";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var menuItemSize = new SelectListItem
                        {
                            Value = reader["SizeID"].ToString(),
                            Text = reader["SizeName"].ToString()
                        };
                        MenuItemSizes.Add(menuItemSize);
                    }

                    // Add a default selection option at the top
                    var defaultMenuItemSize = new SelectListItem
                    {
                        Value = "0",
                        Text = "--Select Size--"
                    };
                    MenuItemSizes.Insert(0, defaultMenuItemSize);
                }
            }
        }

        // Helper method to load item types from DB
        private void PopulateItemTypeList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT ItemTypeID, ItemTypeName FROM ItemType";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var itemType = new SelectListItem
                        {
                            Value = reader["ItemTypeID"].ToString(),
                            Text = reader["ItemTypeName"].ToString()
                        };
                        ItemType.Add(itemType);
                    }

                    // Add a default selection option at the top
                    var defaultItemType = new SelectListItem
                    {
                        Value = "0",
                        Text = "--Select Item Type--"
                    };
                    ItemType.Insert(0, defaultItemType);
                }
            }
        }

        // Helper method to load all ingredients from DB
        private void PopulateIngredientsList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT IngredientID, IngredientName FROM Ingredients";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var ingredient = new IngredientInfo
                        {
                            IngredientID = int.Parse(reader["IngredientID"].ToString()),
                            IngredientName = reader["IngredientName"].ToString(),
                            IsSelected = false // Default to not selected
                        };
                        Ingredients.Add(ingredient);
                    }
                }
            }
        }
    }

    // Helper class used for binding ingredient selection
    public class IngredientInfo
    {
        public bool IsSelected { get; set; } // Used for checkbox or selection logic
        public int IngredientID { get; set; } // ID from DB
        public string IngredientName { get; set; } // Display name
    }
}
