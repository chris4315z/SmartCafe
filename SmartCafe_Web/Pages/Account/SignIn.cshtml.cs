using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Web.Model;
using SmartCafe_Business;

namespace SmartCafe_Web.Pages.Account
{
    public class SigninModel : PageModel
    {
        [BindProperty]
        public Signin SigninUser { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Validate User Input
                // Check if the user exists in the database
                // If the user exists, redirect to the profile page
                // If the user does not exist, display an error message
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    // Try to connect to the database and see if select statement returns anything
                    string cmdText = "SELECT SystemUserID, SystemUserPassword FROM [SystemUser] WHERE SystemUserEmail = @email";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@email", SigninUser.Email);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    // if user exists in the database, aka database returns it
                    if (reader.HasRows)
                    {
                        // Grab the value
                        reader.Read();
                        // GetString(1) gets the first value in a row, it's index so it starts with a 0, which would be SystemUserID
                        string passwordHash = reader.GetString(1);
                        // Verify if the password they put is the right password
                        if (AppHelper.VerifyPassword(SigninUser.Password, passwordHash))
                        {
                            return RedirectToPage("/Account/Profile");
                        }
                        else // User entered the wrong password.
                        {
                            ModelState.AddModelError("SigninError", "Invalid credentials.");
                            return Page();
                        }
                    }
                    else
                    {
                        // User doesn't exist in the database.
                        ModelState.AddModelError("SigninError", "Invalid credentials.");
                        return Page();
                    }
                }
            }
            else
            {
                return Page();
            }
        }
    }
}
