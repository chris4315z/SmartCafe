using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.MenuItems
{
    public class SearchMenuItemsModel : PageModel
    {
        public List<ItemView> Items { get; set; } = new List<ItemView>();
        public void OnGet()
        {
            PopulateMenuItemList();
        }

        private void PopulateMenuItemList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID" + " FROM MenuItem i " +
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
                            //IngredientName = reader.GetString(3),
                            MenuItemIngredients = PopulateMenuItemIngredients(reader.GetInt32(0))
                        };
                        Items.Add(items);
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
        // :)
    }
}