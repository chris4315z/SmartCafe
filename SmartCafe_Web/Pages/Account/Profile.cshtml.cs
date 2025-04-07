using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;

namespace SmartCafe_Web.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public ProfileView UserProfile { get; set; } = new ProfileView();
        public void OnGet()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            PopulateUserProfile(userId);
        }

        private void PopulateUserProfile(int userId)
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "SELECT SystemUserFirstName, SystemUserLastName, SystemUserEmailAddress, SystemUserProfilePicture, AccountTypeName, LastLoginTime" +
                    " FROM [SystemUser] INNER JOIN AccountType" +
                    " ON [SystemUser].AccountTypeID = AccountType.AccountTypeID WHERE SystemUserID = @userId";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
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
