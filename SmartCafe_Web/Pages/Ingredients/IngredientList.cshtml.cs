using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Ingredients
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class IngredientListModel : PageModel
    {
        public List<Ingredient> IngredientList { get; set; } = new List<Ingredient>();
        public void OnGet()
        {
            PopulateIngredientList();
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                /*
                if (User.IsInRole("Admin") == false)
                {
                    return RedirectToPage("/Account/AccessDenied");
                }
                */
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "DELETE FROM Ingredients WHERE IngredientID = @IngredientID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@IngredientID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("IngredientList");
            }
            catch
            {
                throw;
            }
        }

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
                throw;
            }
        }
    }
}
