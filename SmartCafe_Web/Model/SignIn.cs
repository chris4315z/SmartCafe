using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Signin
    {
        // Sign in page code
        [Display(Name = "Email:")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}