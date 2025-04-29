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

        public MenuItem CurrentItem { get; set; }

        public List<SelectListItem> MenuItem { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        public List<IngredientInfo> Ingredients { get; set; } = new List<IngredientInfo>();

        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        //public List<MenuItemInfo> MenuItems { get; set; } = new List<MenuItemInfo>();

        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();
        public void OnGet(int id)
        {
            PopulateMenuItemDetails(id);
            PopulateItemTypeList();
            SelectedMenuItemIngredientsIDs = PopulateSelectedMenuItemIngredientsIDs(id);
            PopulateIngredientsList();
        }
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
                        if (SelectedMenuItemIngredientsIDs.Contains(ingredient.IngredientID))
                        {
                            ingredient.IsSelected = true;
                        }
                        else
                        {
                            ingredient.IsSelected = false;
                        }
                    }
                }
            }
        }

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
                    var defaultItemType = new SelectListItem();
                    defaultItemType.Value = "0";
                    defaultItemType.Text = "--Select Item Type--";
                    ItemType.Insert(0, defaultItemType);
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate data for the page in case of error
                PopulateItemTypeList();
                PopulateIngredientsList();
                return Page();
            }

            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
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

                // Update MenuItemIngredients (simple approach: delete and re-insert)
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM MenuItemIngredients WHERE MenuItemID = @MenuItemID", conn);
                deleteCmd.Parameters.AddWithValue("@MenuItemID", CurrentItem.MenuItemID);
                deleteCmd.ExecuteNonQuery();

                foreach (int ingredientId in SelectedMenuItemIngredientsIDs)
                {
                    SqlCommand insertCmd = new SqlCommand("INSERT INTO MenuItemIngredients (MenuItemID, IngredientID) VALUES (@MenuItemID, @IngredientID)", conn);
                    insertCmd.Parameters.AddWithValue("@MenuItemID", CurrentItem.MenuItemID);
                    insertCmd.Parameters.AddWithValue("@IngredientID", ingredientId);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/MenuItems/SearchMenuItems", new { id = CurrentItem.MenuItemID });
        }


    }
}
