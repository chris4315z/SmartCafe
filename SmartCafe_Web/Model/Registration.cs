using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class Registration
    {
        // Registration page data?
        // Declaration - tells razor pages that property named First Name, it will display the first name instead of property name?
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
