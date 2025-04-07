using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;

namespace SmartCafe_Web.Pages.MenuItems
{
    [BindProperties]
    public class AddMenuItemModel : PageModel
    {
        public MenuItem NewMenuItem{get; set;}

        public List<SelectListItem> ItemTypeID { get; set; }

        public List<ItemTypeInfo> ItemType {  get; set; }


        public void OnGet()
        {
            PopulateItemTypeList();
        }

        private void PopulateItemTypeList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                
            }
        }

        public class ItemTypeInfo
        {

        }
    }
}
