using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;

namespace SmartCafe_Web.Pages.Account
{
    // This class handles the logic for the Profile Razor Page
    [Authorize] // Ensures that only authenticated users can access this page
    public class ProfileModel : PageModel
    {
        // Binds the ProfileView model to the Razor Page so it can be used in the UI
        [BindProperty]
        public ProfileView UserProfile { get; set; } = new ProfileView();

        // Called when the page is first accessed (GET request)
        public void OnGet()
        {
            // Retrieve the user's ID from their authentication claims
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Populate the UserProfile object with data from the database
            PopulateUserProfile(userId);
        }

        // Loads user profile information from the database using the user ID
        private void PopulateUserProfile(int userId)
        {
            // Create a new SQL connection using a helper method to get the connection string
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                // SQL query to get user info by joining SystemUser and AccountType tables
                string cmdText = "SELECT SystemUserFirstName, SystemUserLastName, SystemUserEmailAddress, SystemUserProfilePicture, AccountTypeName, LastLoginTime" +
                    " FROM [SystemUser] INNER JOIN AccountType" +
                    " ON [SystemUser].AccountTypeID = AccountType.AccountTypeID WHERE SystemUserID = @userId";

                // Create SQL command and add parameter to prevent SQL injection
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                // Open the database connection
                conn.Open();

                // Execute the command and retrieve the results
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read(); // Move to the first record

                    // Map the database values to the UserProfile model
                    UserProfile.FirstName = reader.GetString(0);
                    UserProfile.LastName = reader.GetString(1);
                    UserProfile.Email = reader.GetString(2);
                    UserProfile.ProfileImageURL = reader.GetString(3);
                    UserProfile.AccountType = reader.GetString(4);
                    UserProfile.LastLoginTime = reader.GetDateTime(5);
                }
            }
        }
    }
}
