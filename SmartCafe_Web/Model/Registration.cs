using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Registration
    {
        // Registration page data?
        // Declaration - tells razor pages that property named First Name, it will display the first name instead of property name?
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Display(Name = "Username:")]
        public string Username { get; set; }
        [Display(Name = "Password:")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password:")]
        public string ConfirmPassword { get; set; }
    }
}
