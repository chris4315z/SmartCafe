using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Web.Model;
using SmartCafe_Business;

namespace SmartCafe_Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Registration NewUser { get; set; }
        public void OnGet()
        {
            //NewUser.FirstName = "John";
        }

        public IActionResult OnPost()
        {
            // Validate User Input
            if (ModelState.IsValid)
            {
                // Save to Database
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    // 2. Create a command to insert the data
                    string cmdText = "INSERT INTO [User] (UserFirstName, UserLastName, UserProfileImage, UserEmail, UserPassword, AccountTypeID) VALUES (@FirstName, @LastName, @ProfileImage, @Email, @Password, 3)";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@FirstName", NewUser.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", NewUser.LastName);
                    cmd.Parameters.AddWithValue("@ProfileImage", "default.jpg");
                    cmd.Parameters.AddWithValue("@Email", NewUser.Email);
                    cmd.Parameters.AddWithValue("@Password", AppHelper.GeneratePasswordHash(NewUser.Password));
                    // 3. Execute the command
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Redirect to Login Page
                return RedirectToPage("/Account/Signin");
            }
            else
            {
                return Page();
            }
        }
    }
}