using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Collections.Generic;
using System.Security.Claims;

namespace SmartCafe_Web.Pages
{
    // Only users with the "Admin" role are authorized to access this page
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class ManageAccountsModel : PageModel
    {
        // List to hold the system user data for display in the page
        public List<SystemUser> SystemUserList { get; set; } = new List<SystemUser>();

        // On GET request, populate the list of system users
        public void OnGet()
        {
            PopulateSystemUserList();
        }

        // Handles deleting a user account
        public IActionResult OnPostDelete(int id)
        {
            // Prevent deletion of the current admin's own account
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            try
            {
                // Open a database connection to delete the user by ID
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "DELETE FROM dbo.SystemUser WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemUserID", id);
                        cmd.ExecuteNonQuery(); // Execute the delete command
                    }
                }
                return RedirectToPage("/ManageAccounts");
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while trying to delete the account.";
                return RedirectToPage("/ManageAccounts");
            }
        }

        // Handles promoting a user to admin role
        public IActionResult OnPostPromote(int id)
        {
            // Prevent promotion of the current admin's own account
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot promote your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            return UpdateAccountType(id, 1); // 1 = Admin
        }

        // Handles demoting a user to a regular user role
        public IActionResult OnPostDemote(int id)
        {
            // Prevent demotion of the current admin's own account
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot demote your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            return UpdateAccountType(id, 2); // 2 = User
        }

        // Helper method to update the account type (role) of a user
        private IActionResult UpdateAccountType(int userId, int newTypeId)
        {
            try
            {
                // Open a database connection to update the account type
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "UPDATE dbo.SystemUser SET AccountTypeID = @AccountTypeID WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountTypeID", newTypeId);
                        cmd.Parameters.AddWithValue("@SystemUserID", userId);
                        cmd.ExecuteNonQuery(); // Execute the update command
                    }
                }
                return RedirectToPage("/ManageAccounts");
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while updating the account.";
                return RedirectToPage("/ManageAccounts");
            }
        }

        // Helper method to populate the system user list by fetching data from the database
        private void PopulateSystemUserList()
        {
            try
            {
                // Open a database connection and retrieve the list of users
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT SystemUserID, SystemUsername, SystemUserEmailAddress, AccountTypeID FROM dbo.SystemUser ORDER BY SystemUsername";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Read each user and add it to the list
                            while (reader.Read())
                            {
                                SystemUser user = new SystemUser
                                {
                                    SystemUserID = reader.GetInt32(0),
                                    SystemUsername = reader.GetString(1),
                                    SystemUserEmailAddress = reader.GetString(2),
                                    AccountTypeID = reader.GetInt32(3)
                                };
                                SystemUserList.Add(user); // Add the user to the list
                            }
                        }
                    }
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to load user list."; // Error if user list loading fails
            }
        }

        // Helper method to get the current logged-in user's ID
        private int GetCurrentUserID()
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0; // Return user ID or 0 if not found
        }
    }
}
