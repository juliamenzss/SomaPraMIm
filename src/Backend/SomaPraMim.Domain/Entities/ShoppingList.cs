namespace SomaPraMim.Domain.Entities
{
    public class ShoppingList : ModelBase
    {
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public long UserId { get; set; }
        public User? User { get; set; } 
        public List<ShoppingItem> Items { get; set; } = new();
    }
}