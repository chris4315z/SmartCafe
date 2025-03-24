using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.Account
{
    public class Sign_inModel : PageModel
    {
        public SignIn User {get; set;} = new SignIn();
        public void OnGet()
        {
        }
    }
}
