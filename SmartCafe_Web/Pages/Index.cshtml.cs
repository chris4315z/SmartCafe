using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages;

public class IndexModel : PageModel
{
    public List<ItemDisplay> ItemDisplays { get; set; } = new List<ItemDisplay>();
    public void OnGet()
    {
        PopulateItemDisplays();
    }

    private void PopulateItemDisplays()
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

                    ItemDisplay itemDisplay = new ItemDisplay
                    {
                        MenuItemID = reader.GetInt32(0),
                        ItemName = reader.GetString(1),
                        ItemImage = reader.GetString(2),
                        Price = reader.GetDecimal(3),
                    };
                    ItemDisplays.Add(itemDisplay);
                }
            }
        }
    }


}
