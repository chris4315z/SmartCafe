using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.MenuItems
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class EditMenuItemModel : PageModel
    {
        // Properties to hold data for the view
        public MenuItem CurrentItem { get; set; }

        public List<SelectListItem> MenuItem { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        public List<IngredientInfo> Ingredients { get; set; } = new List<IngredientInfo>();

        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        // List of selected ingredient IDs for the menu item
        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();

        // Runs when the page is first accessed (GET)
        public void OnGet(int id)
        {
            PopulateMenuItemDetails(id);                    // Load item details from DB
            PopulateItemTypeList();                         // Load dropdown list of item types
            SelectedMenuItemIngredientsIDs = PopulateSelectedMenuItemIngredientsIDs(id);  // Get selected ingredient IDs
            PopulateIngredientsList();                      // Load ingredient options and mark selected ones
        }

        // Retrieves the ingredient IDs currently assigned to the menu item
        private List<int> PopulateSelectedMenuItemIngredientsIDs(int id)
        {
            List<int> selectedMenuItemIngredientsIDs = new List<int>();
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT IngredientID FROM MenuItemIngredients WHERE MenuItemID = @menuItemID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@menuItemID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        selectedMenuItemIngredientsIDs.Add(reader.GetInt32(0));
                    }
                }
            }
            return selectedMenuItemIngredientsIDs;
        }

        // Retrieves the main details of the menu item from the DB
        private void PopulateMenuItemDetails(int id)
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT MenuItemID, ItemName, ItemImage, Price, ItemTypeID FROM MenuItem WHERE MenuItemID = @menuItemID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@menuItemID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CurrentItem = new MenuItem
                        {
                            MenuItemID = reader.GetInt32(0),
                            ItemName = reader.GetString(1),
                            ItemImage = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ItemTypeID = reader.GetInt32(4)
                        };
                    }
                }
            }
        }

        // Loads the full ingredient list from the DB and marks selected ones
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
                        var ingredient = new IngredientInfo();
                        ingredient.IngredientID = int.Parse(reader["IngredientID"].ToString());
                        ingredient.IngredientName = reader["IngredientName"].ToString();
                        Ingredients.Add(ingredient);

                        // Set whether the ingredient is currently selected for the item
                        ingredient.IsSelected = SelectedMenuItemIngredientsIDs.Contains(ingredient.IngredientID);
                    }
                }
            }
        }

        // Loads list of item types for dropdown (e.g., food, drink, etc.)
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
                        var itemType = new SelectListItem();
                        itemType.Value = reader["ItemTypeID"].ToString();
                        itemType.Text = reader["ItemTypeName"].ToString();
                        ItemType.Add(itemType);
                    }

                    // Add default "Select Item Type" option at the top
                    var defaultItemType = new SelectListItem();
                    defaultItemType.Value = "0";
                    defaultItemType.Text = "--Select Item Type--";
                    ItemType.Insert(0, defaultItemType);
                }
            }
        }

        // Called when the form is submitted (POST)
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // If form has validation errors, reload dropdowns and ingredient list
                PopulateItemTypeList();
                PopulateIngredientsList();
                return Page();
            }

            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                // Update the MenuItem table with new values
                string updateQuery = @"
                    UPDATE MenuItem 
                    SET ItemName = @ItemName, 
                        ItemImage = @ItemImage, 
                        Price = @Price, 
                        ItemTypeID = @ItemTypeID 
                    WHERE MenuItemID = @MenuItemID";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@ItemName", CurrentItem.ItemName);
                cmd.Parameters.AddWithValue("@ItemImage", CurrentItem.ItemImage);
                cmd.Parameters.AddWithValue("@Price", CurrentItem.Price);
                cmd.Parameters.AddWithValue("@ItemTypeID", CurrentItem.ItemTypeID);
                cmd.Parameters.AddWithValue("@MenuItemID", CurrentItem.MenuItemID);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Remove all existing ingredients linked to this item
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM MenuItemIngredients WHERE MenuItemID = @MenuItemID", conn);
                deleteCmd.Parameters.AddWithValue("@MenuItemID", CurrentItem.MenuItemID);
                deleteCmd.ExecuteNonQuery();

                // Re-insert selected ingredients
                foreach (int ingredientId in SelectedMenuItemIngredientsIDs)
                {
                    SqlCommand insertCmd = new SqlCommand("INSERT INTO MenuItemIngredients (MenuItemID, IngredientID) VALUES (@MenuItemID, @IngredientID)", conn);
                    insertCmd.Parameters.AddWithValue("@MenuItemID", CurrentItem.MenuItemID);
                    insertCmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                    insertCmd.ExecuteNonQuery();
                }
            }

            // Redirect back to the search or listing page
            return RedirectToPage("/MenuItems/SearchMenuItems", new { id = CurrentItem.MenuItemID });
        }
    }
}
