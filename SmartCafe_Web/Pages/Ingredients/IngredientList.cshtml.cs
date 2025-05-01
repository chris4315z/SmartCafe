using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Ingredients
{
    // This page requires the user to be in the "Admin" role
    [Authorize(Roles = "Admin")]
    // Automatically binds public properties for POST requests
    [BindProperties]
    public class IngredientListModel : PageModel
    {
        // This property holds the list of ingredients to be displayed
        public List<Ingredient> IngredientList { get; set; } = new List<Ingredient>();

        // Called on GET request to load the page
        public void OnGet()
        {
            PopulateIngredientList(); // Load the list of ingredients from the database
        }

        // Handles the deletion of an ingredient when a POST request with the id is made
        public IActionResult OnPostDelete(int id)
        {
            try
            {
                /*
                // Optional: Uncomment this to do an extra check in case the [Authorize] attribute is not sufficient
                if (User.IsInRole("Admin") == false)
                {
                    return RedirectToPage("/Account/AccessDenied");
                }
                */

                // Connect to the database and delete the ingredient with the specified ID
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "DELETE FROM Ingredients WHERE IngredientID = @IngredientID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Prevent SQL injection by using a parameterized query
                        cmd.Parameters.AddWithValue("@IngredientID", id);
                        cmd.ExecuteNonQuery(); // Execute the delete command
                    }
                }

                // After deletion, reload the IngredientList page
                return RedirectToPage("IngredientList");
            }
            catch
            {
                // If an error occurs, rethrow it to be handled by global error handling
                throw;
            }
        }

        // Loads the ingredient list from the database into the IngredientList property
        private void PopulateIngredientList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT IngredientID, IngredientName FROM Ingredients ORDER BY IngredientName";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Loop through each record and add to the IngredientList
                            while (reader.Read())
                            {
                                Ingredient ingredient = new Ingredient
                                {
                                    IngredientID = reader.GetInt32(0),
                                    IngredientName = reader.GetString(1)
                                };
                                IngredientList.Add(ingredient);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Re-throw any caught exceptions
                throw;
            }
        }
    }
}
