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

        public MenuItem NewItem { get; set; }

        public List<SelectListItem> MenuItem {  get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MenuItemSizes { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> OrderItems { get; set; } = new List<SelectListItem>();

        public List<MenuItemInfo> MenuItems { get; set; } = new List<MenuItemInfo>();

        public List<int> SelectedMenuItemIngredientsIDs { get; set; } = new List<int>();
        public void OnGet()
        {
            PopulateMenuItemList();
            PopulateMenuItemSizesList();
            PopulateItemTypeList();
            PopulateIngredientsList();
            PopulateOrderItemsList();
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
                PopulateMenuItemList();
                PopulateMenuItemSizesList();
                PopulateItemTypeList();
                PopulateIngredientsList();
                PopulateOrderItemsList();
                return Page();
            }
            
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
                        var ingredient = new SelectListItem();
                        ingredient.Value = reader["IngredientID"].ToString();
                        ingredient.Text = reader["IngredientName"].ToString();
                        Ingredients.Add(ingredient);
                    }
                    var defaultIngredient = new SelectListItem();
                    defaultIngredient.Value = "0";
                    defaultIngredient.Text = "--Select Ingredient--";
                    Ingredients.Insert(0, defaultIngredient);
                }
            }
        }

        private void PopulateOrderItemsList()
        {
            using (SqlConnection conn = new SqlConnection(AppHelper.GetDBConnectionString()))
            {
                string query = "SELECT OrderItems, OrderID, MenuItemID, SizeID, Quantity, Subtotal FROM OrderItems";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var orderItems = new SelectListItem();
                        orderItems.Value = reader["OrderItems"].ToString();
                        orderItems.Value = reader["OrderID"].ToString();
                        orderItems.Value = reader["MenuItemID"].ToString();
                        orderItems.Value = reader["SizeID"].ToString();
                        orderItems.Value = reader["Quantity"].ToString();
                        orderItems.Value = reader["Subtotal"].ToString();
                        OrderItems.Add(orderItems);

                    }
                    var defaultOrderItem = new SelectListItem();
                    defaultOrderItem.Value = "0";
                    defaultOrderItem.Text = "--Select Order Item--";
                    OrderItems.Insert(0, defaultOrderItem);
                }
            }
        }
    }

    public class MenuItemInfo
    {
        public int MenuItemID { get; set; }

        public string ItemName { get; set; }

        public bool IsSelected { get; set; }

    }
}
