using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.MenuItems
{
    
    public class ViewItemsModel : PageModel
    {
        private string cmdText;

        public ItemView MenuItem { get; set; } = new ItemView();
        
        public void OnGet(int id)
        {
            PopulateMenuItemInfo(id);
        }

        private void PopulateMenuItemInfo(int id)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID" + " FROM MenuItem i " +
                    "JOIN ItemType t ON i.ItemTypeID = t.ItemTypeID";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        MenuItem.MenuItemID = reader.GetInt32(0);
                        MenuItem.ItemName = reader.GetString(1);
                        MenuItem.ItemImage = reader.GetString(2);
                        MenuItem.Price = reader.GetDecimal(3);
                        MenuItem.ItemTypeID = reader.GetInt32(4);
                            
                        MenuItem.MenuItemIngredients = PopulateMenuItemIngredients(reader.GetInt32(0));
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private List<string> PopulateMenuItemIngredients(int v)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    string cmdText = "SELECT n.IngredientName FROM Ingredients n " +
                   "JOIN MenuItemIngredients mn ON n.IngredientID = mn.IngredientID " +
                   "WHERE mn.MenuItemID = @MenuItemID"; 
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@id", v);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> ingredients = new List<string>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ingredients.Add(reader.GetString(0));
                        }
                    }
                    return ingredients;
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
