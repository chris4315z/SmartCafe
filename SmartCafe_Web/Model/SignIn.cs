using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class SignIn
    {
        [Display(Name = "Username:")]
        public string Username { get; set; }
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}