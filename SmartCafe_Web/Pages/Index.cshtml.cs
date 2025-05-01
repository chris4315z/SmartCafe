using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages;

// This PageModel handles the logic for the homepage (Index page)
public class IndexModel : PageModel
{
    // List to store the items that will be displayed on the homepage
    public List<ItemDisplay> ItemDisplays { get; set; } = new List<ItemDisplay>();

    // On GET request, populate the list of display items
    public void OnGet()
    {
        PopulateItemDisplays();
    }

    // Method to retrieve menu item data from the database
    private void PopulateItemDisplays()
    {
        // Establish a SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
        {
            // SQL command to select item data and join with ItemType table
            string cmdText = "SELECT i.MenuItemID, i.ItemName, i.ItemImage, i.Price, i.ItemTypeID" +
                             " FROM MenuItem i " +
                             "JOIN ItemType t ON i.ItemTypeID = t.ItemTypeID";

            SqlCommand cmd = new SqlCommand(cmdText, conn);
            conn.Open(); // Open the connection
            SqlDataReader reader = cmd.ExecuteReader(); // Execute the query

            // If the query returns rows, read through each row
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // Create an ItemDisplay object for each menu item
                    ItemDisplay itemDisplay = new ItemDisplay
                    {
                        MenuItemID = reader.GetInt32(0),    // Get the MenuItemID
                        ItemName = reader.GetString(1),     // Get the ItemName
                        ItemImage = reader.GetString(2),    // Get the ItemImage URL or path
                        Price = reader.GetDecimal(3),       // Get the Price
                    };

                    // Add the item to the list for display
                    ItemDisplays.Add(itemDisplay);
                }
            }
        }
    }
}
