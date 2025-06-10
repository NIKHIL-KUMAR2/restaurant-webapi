namespace Restaurant_WebAPI.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MenuItemModel
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
    }
}
