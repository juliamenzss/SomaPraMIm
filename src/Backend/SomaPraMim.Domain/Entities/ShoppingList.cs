using System.Text.Json.Serialization;

namespace SomaPraMim.Domain.Entities
{
    public class ShoppingList : ModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public long UserId { get; set; }
        public User? User { get; set; } 
        public List<ShoppingItem> ShoppingItems { get; set; } = [];
    }
}