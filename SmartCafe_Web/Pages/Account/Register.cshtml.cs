using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public Registration NewUser { get; set; } = new Registration();
        public void OnGet()
        {
            NewUser.FirstName = "John";
        }

        // When we submit the form, we POST to the server?
        public void OnPost()
        {
            
        }
    }
}