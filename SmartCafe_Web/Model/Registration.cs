using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Registration
    {
        // Registration page code.
       
        [Display(Name = "First Name:")]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Display(Name = "Email:")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Username:")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Password is required")]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{10,}$",
            ErrorMessage = "Password must be at least 10 characters long, contain at least one number, one uppercase letter, and one lowercase letter.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password:")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match ")]
        public string ConfirmPassword { get; set; }
    }
}