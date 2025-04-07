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
                // 1. Create a connection to the database
                // string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=SmartCafe;Trusted_Connection=True;";
                // SqlConnection conn = new SqlConnection(connectionString);


                // 2. Create a command to insert the data
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    string cmdText = "INSERT INTO [SystemUser] (SystemUserFirstName, SystemUserLastName, SystemUserProfilePicture, SystemUsername, SystemUserEmailAddress, SystemUserPassword, AccountTypeID) VALUES (@FirstName, @LastName, @ProfilePicture, @Username, @Email, @Password, 2)";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@FirstName", NewUser.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", NewUser.LastName);
                    cmd.Parameters.AddWithValue("@ProfilePicture", "default.jpg");
                    cmd.Parameters.AddWithValue("@Username", NewUser.Username);
                    cmd.Parameters.AddWithValue("@Email", NewUser.Email);
                    cmd.Parameters.AddWithValue("@Password", AppHelper.GeneratePasswordHash(NewUser.Password));

                    // 3. Open the connection and execute the command
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