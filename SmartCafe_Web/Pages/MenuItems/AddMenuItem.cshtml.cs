using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace SmartCafe_Web.Pages.MenuItems
{
    [Authorize]
    [BindProperties]
    public class AddMenuItemModel : PageModel
    {
        //private SelectListItem ingredients;

        public MenuItem NewItem { get; set; }

        public List<SelectListItem> MenuItem {  get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        public List<IngredientInfo> Ingredients { get; set; } = new List<IngredientInfo>();

        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        //public List<MenuItemInfo> MenuItems { get; set; } = new List<MenuItemInfo>();

        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();
        public void OnGet()
        {
            //PopulateMenuItemList();
            PopulateMenuItemSizesList();
            PopulateItemTypeList();
            PopulateIngredientsList();
        }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
                {
                    string cmdText = "INSERT INTO [MenuItem] (ItemName, ItemImage, Price, ItemTypeID) " +
                        "VALUES (@itemName, @itemImage, @price, @itemTypeID); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@itemName", NewItem.ItemName);
                    cmd.Parameters.AddWithValue("@itemImage", NewItem.ItemImage);
                    cmd.Parameters.AddWithValue("@price", NewItem.Price);
                    cmd.Parameters.AddWithValue("@itemTypeID", NewItem.ItemTypeID);
                    int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    conn.Open();
                    int menuItemID = int.Parse(cmd.ExecuteScalar().ToString());

                    for (int i = 0; i < SelectedMenuItemIngredientsIDs.Count; i++)
                    {
                        cmdText = "INSERT INTO [MenuItemIngredients] (MenuItemID, IngredientID) VALUES (@menuItemID, @ingredientID)";
                        cmd = new SqlCommand(cmdText, conn);
                        cmd.Parameters.AddWithValue("@menuItemID", menuItemID);
                        cmd.Parameters.AddWithValue("@ingredientID", SelectedMenuItemIngredientsIDs[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/Index");
            }
            else
            {
                //PopulateMenuItemList();
                PopulateMenuItemSizesList();
                PopulateItemTypeList();
                PopulateIngredientsList();
                return Page();
            }
            
        }
        

        private void PopulateMenuItemSizesList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT SizeID, SizeName FROM MenuItemSizes";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var menuItemSize = new SelectListItem();
                        menuItemSize.Value = reader["SizeID"].ToString();
                        menuItemSize.Text = reader["SizeName"].ToString();
                        MenuItemSizes.Add(menuItemSize);
                    }
                    var defaultMenuItemSize = new SelectListItem();
                    defaultMenuItemSize.Value = "0";
                    defaultMenuItemSize.Text = "--Select Size--";
                    MenuItemSizes.Insert(0, defaultMenuItemSize);
                }
            }
        }

        private void PopulateItemTypeList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT ItemTypeID, ItemTypeName FROM ItemType";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var itemType = new SelectListItem();
                        itemType.Value = reader["ItemTypeID"].ToString();
                        itemType.Text = reader["ItemTypeName"].ToString();
                        ItemType.Add(itemType);
                    }
                    var defaultItemType = new SelectListItem();
                    defaultItemType.Value = "0";
                    defaultItemType.Text = "--Select Item Type--";
                    ItemType.Insert(0, defaultItemType);
                }
            }
        }

        private void PopulateIngredientsList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT IngredientID, IngredientName FROM Ingredients";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var ingredient = new IngredientInfo();
                        ingredient.IngredientID = int.Parse(reader["IngredientID"].ToString());
                        ingredient.IngredientName = reader["IngredientName"].ToString();
                        ingredient.IsSelected = false;
                        Ingredients.Add(ingredient);
                    }
                }
            }
        }
    }

    public class IngredientInfo
    {
        public bool IsSelected { get; set; }

        public int IngredientID { get; set; }

        public string IngredientName { get;set; }

    }
}
