using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Collections.Generic;

namespace SmartCafe_Web.Pages.SystemUsers
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class SystemUserListModel : PageModel
    {
        public List<SystemUser> SystemUserList { get; set; } = new List<SystemUser>();

        public void OnGet()
        {
            PopulateSystemUserList();
        }

        public IActionResult OnPostDelete(int id)
        {
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
                return RedirectToPage("/ManageAccounts");  // Updated redirect path
            }
            catch
            {
                throw;
            }
        }

        public IActionResult OnPostPromote(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    // Assuming 1 is Admin in the AccountTypeID field
                    string sql = "UPDATE dbo.SystemUser SET AccountTypeID = 1 WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemUserID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/ManageAccounts");  // Updated redirect path
            }
            catch
            {
                throw;
            }
        }

        public IActionResult OnPostDemote(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    conn.Open();
                    // Assuming 2 is User in the AccountTypeID field
                    string sql = "UPDATE dbo.SystemUser SET AccountTypeID = 2 WHERE SystemUserID = @SystemUserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemUserID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/ManageAccounts");  // Updated redirect path
            }
            catch
            {
                throw;
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
                throw;
            }
        }
    }
}
