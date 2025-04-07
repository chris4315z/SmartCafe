using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;

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
                    string cmdText = "SELECT UserID, UserPassword, UserFirstName, AccountTypeName" +
                        " FROM [User] INNER JOIN AccountType" +
                        " ON [User].AccountTypeID = AccountType.AccountTypeID WHERE UserEmail = @email";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@email", SigninUser.Email);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        string passwordHash = reader.GetString(1);
                        if (AppHelper.VerifyPassword(SigninUser.Password, passwordHash))
                        {
                            // create a email claim
                            Claim emailClaim = new Claim(ClaimTypes.Email, SigninUser.Email);
                            // create a user id claim
                            Claim userIdClaim = new Claim(ClaimTypes.NameIdentifier, reader.GetInt32(0).ToString());
                            // create a name claim
                            Claim nameClaim = new Claim(ClaimTypes.Name, reader.GetString(2));
                            // create a role claim
                            Claim roleClaim = new Claim(ClaimTypes.Role, reader.GetString(3));

                            // create a list of claims
                            List<Claim> claims = new List<Claim> { emailClaim, userIdClaim, nameClaim, roleClaim };

                            // create a claims identity
                            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // create a claims principal
                            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                            // sign in the user
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            // update user login time
                            UpdateUserLoginTime(reader.GetInt32(0));

                            return RedirectToPage("/Account/Profile");
                        }
                        else
                        {
                            ModelState.AddModelError("SigninError", "Invalid credentials.");
                            return Page();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SignInError", "Invalid credentials.");
                        return Page();
                    }
                }
            }
            else
            {
                return Page();
            }
        }

        private void UpdateUserLoginTime(int v)
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string cmdText = "UPDATE [User] SET LastLoginTime = @loginTime WHERE UserID = @userId";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@SignInTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@userId", v);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
