using SomaPraMim.Domain.Enums;

namespace SomaPraMim.Domain.Entities
{
    public class ShoppingItem : ModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public MeasurementUnit Unit { get; set; } 
        public long ShoppingListId { get; set; }
        public ShoppingList? ShoppingList { get; set; }
    }
}