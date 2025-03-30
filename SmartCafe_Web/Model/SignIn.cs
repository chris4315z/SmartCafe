using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Signin
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Username:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}