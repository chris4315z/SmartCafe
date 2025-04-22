using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Ingredients
{
    [Authorize(Roles = "Admin")]
    public class EditIngredientModel : PageModel
    {
        [BindProperty]
        public Ingredient CurrentIngredient { get; set; } = new Ingredient();
        public void OnGet(int id)
        {
            PopulateIngredientInfo(id);
        }

        public IActionResult OnPost(int id)
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                conn.Open();
                string sql = "UPDATE Ingredient SET IngredientName = @IngredientName WHERE IngredientID = @IngredientID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IngredientName", CurrentIngredient.IngredientName);
                    cmd.Parameters.AddWithValue("@IngredientID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToPage("IngredientList");
        }

        private void PopulateIngredientInfo(int id)
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                conn.Open();
                                    // EITHER INGREDIENTS OR INGREDIENT
                string sql = "SELECT IngredientID, IngredientName FROM Ingredients WHERE IngredientID = @IngredientID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IngredientID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentIngredient.IngredientID = reader.GetInt32(0);
                            CurrentIngredient.IngredientName = reader.GetString(1);
                        }
                    }
                }
            }
        }
    }
}
