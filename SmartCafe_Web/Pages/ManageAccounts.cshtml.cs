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
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class ManageAccountsModel : PageModel
    {
        public List<SystemUser> SystemUserList { get; set; } = new List<SystemUser>();

        public void OnGet()
        {
            PopulateSystemUserList();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "DELETE FROM dbo.SystemUser WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemUserID", id);
                        cmd.ExecuteNonQuery();
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

        public IActionResult OnPostPromote(int id)
        {
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot promote your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            return UpdateAccountType(id, 1); // 1 = Admin
        }

        public IActionResult OnPostDemote(int id)
        {
            if (id == GetCurrentUserID())
            {
                TempData["ErrorMessage"] = "You cannot demote your own account.";
                return RedirectToPage("/ManageAccounts");
            }

            return UpdateAccountType(id, 2); // 2 = User
        }

        private IActionResult UpdateAccountType(int userId, int newTypeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "UPDATE dbo.SystemUser SET AccountTypeID = @AccountTypeID WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountTypeID", newTypeId);
                        cmd.Parameters.AddWithValue("@SystemUserID", userId);
                        cmd.ExecuteNonQuery();
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

        private void PopulateSystemUserList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT SystemUserID, SystemUsername, SystemUserEmailAddress, AccountTypeID FROM dbo.SystemUser ORDER BY SystemUsername";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SystemUser user = new SystemUser
                                {
                                    SystemUserID = reader.GetInt32(0),
                                    SystemUsername = reader.GetString(1),
                                    SystemUserEmailAddress = reader.GetString(2),
                                    AccountTypeID = reader.GetInt32(3)
                                };
                                SystemUserList.Add(user);
                            }
                        }
                    }
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to load user list.";
            }
        }

        private int GetCurrentUserID()
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }
    }
}
