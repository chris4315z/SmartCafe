using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Ingredients
{
    [Authorize(Roles = "Admin")]
    public class AddIngredientModel : PageModel
    {
        [BindProperty]
        public Ingredient NewIngredient { get; set; } = new Ingredient();

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Here you would typically save the new ingredient to the database
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                    {
                        conn.Open();
                        string sql = "INSERT INTO Ingredient (IngredientName) VALUES (@IngredientName)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@IngredientName", NewIngredient.IngredientName);
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
            return Page();
        }
    }
}
