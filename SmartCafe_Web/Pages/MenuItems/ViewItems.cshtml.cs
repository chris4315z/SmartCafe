using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.MenuItems
{
    public class ViewItemsModel : PageModel
    {
        // This string is unused; can be removed unless needed later
        private string cmdText;

        // Holds the menu item information to display on the page
        public ItemView MenuItem { get; set; } = new ItemView();

        // Triggered when the page is accessed with a menu item ID
        public void OnGet(int id)
        {
            // Load the menu item information based on the provided ID
            PopulateMenuItemInfo(id);
        }

        // Loads menu item details from the database
        private void PopulateMenuItemInfo(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    // SQL query to get item details and join with item type
                    string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID" +
                                     " FROM MenuItem i " +
                                     "JOIN ItemType t ON i.ItemTypeID = t.ItemTypeID";

                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read(); // Read the first result row
                        // Populate the MenuItem model with values from the database
                        MenuItem.MenuItemID = reader.GetInt32(0);
                        MenuItem.ItemName = reader.GetString(1);
                        MenuItem.ItemImage = reader.GetString(2);
                        MenuItem.Price = reader.GetDecimal(3);
                        MenuItem.ItemTypeID = reader.GetInt32(4);

                        // Get the associated ingredient list
                        MenuItem.MenuItemIngredients = PopulateMenuItemIngredients(reader.GetInt32(0));
                    }
                }
            }
            catch
            {
                // Rethrow any exception to be handled by ASP.NET's error pipeline
                throw;
            }
        }

        // Loads the list of ingredient names associated with a menu item
        private List<string> PopulateMenuItemIngredients(int v)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    // SQL query to retrieve ingredient names for a given menu item
                    string cmdText = "SELECT n.IngredientName FROM Ingredients n " +
                                     "JOIN MenuItemIngredients mn ON n.IngredientID = mn.IngredientID " +
                                     "WHERE mn.MenuItemID = @MenuItemID";

                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    // Incorrect parameter name used here: should be "@MenuItemID" instead of "@id"
                    cmd.Parameters.AddWithValue("@id", v); // Potential bug: mismatched parameter name
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<string> ingredients = new List<string>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Add each ingredient name to the list
                            ingredients.Add(reader.GetString(0));
                        }
                    }
                    return ingredients;
                }
            }
            catch
            {
                // Rethrow exception for logging or upstream handling
                throw;
            }
        }
    }
}
