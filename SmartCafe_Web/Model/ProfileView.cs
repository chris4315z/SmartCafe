using System.ComponentModel.DataAnnotations;

namespace SmartCafe_Web.Model
{
    public class ProfileView
    {
        // This is the profile view model
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        [Display(Name = "Email:")]
        public string Email { get; set; }
        public string ProfileImageURL { get; set; }

        [Display(Name = "Account Type: ")]
        public string AccountType { get; set; }

        [Display(Name = "Last Login:")]
        public DateTime LastLoginTime { get; set; }
    }
        
}
