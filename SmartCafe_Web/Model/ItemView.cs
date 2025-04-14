namespace SmartCafe_Web.Model
{
    public class ItemView
    {
        public int ItemTypeID { get; set; }
        public string ItemName { get; set; }
        public string ItemImage { get; set; }
        public decimal Price { get; set; }
        public string ItemTypeName { get; set; }
        public string IngredientName { get; set; }

        public List<string> MenuItemIngredients { get; set; } = new List<string>();
    }
}
