using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SmartCafe_Business;
using SmartCafe_Web.Model;
using System.Security.Claims;

namespace SmartCafe_Web.Pages.MenuItems
{
    [Authorize]
    [BindProperties]
    public class EditMenuItemModel : PageModel
    {

        public MenuItem CurrentItem { get; set; }

        public List<SelectListItem> MenuItem { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        public List<MenuItemInfo> MenuItems { get; set; } = new List<MenuItemInfo>();

        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();
        public void OnGet(int id)
        {
            PopulateMenuItemList();
            PopulateMenuItemSizesList();
            PopulateItemTypeList();
            PopulateIngredientsList();
            PopulateOrderItemsList();
            SelectedMenuItemIngredientsIDs = PopulateSelectedMenuItemIngredientsIDs(id);

        }
        private List<int> PopulateSelectedMenuItemIngredientsIDs(int id)
        {
            List<int> sSelectedMenuItemIngredientsIDs = new List<int>();
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SelectedMenuItemIngredientsIDs.Add(reader.GetInt32(0));
                    }
                }
            }
            return SelectedMenuItemIngredientsIDs;
        }

        private void PopulateMenuItemList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT MenuItemID, ItemName, ItemImage, Price, ItemTypeID FROM MenuItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var menuItem = new MenuItemInfo();
                        menuItem.MenuItemID = int.Parse(reader["MenuItemID"].ToString());
                        menuItem.ItemName = reader["ItemName"].ToString();
                        menuItem.IsSelected = false;
                        MenuItems.Add(menuItem);
                    }
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
                        var ingredients = new MenuItemInfo();
                        ingredients.IngredientID = reader["IngredientID"].ToString();
                        ingredients.IngredientName = reader["IngredientName"].ToString();
                        Ingredients.Add(ingredients);
                        if (SelectedMenuItemIngredientsIDs.Contains(ingredients.IngredientID))
                        {
                            ingredients.IsSelected = true;
                        }
                        else
                        {
                            ingredients.IsSelected = false;
                        }
                    }
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
    }
}
