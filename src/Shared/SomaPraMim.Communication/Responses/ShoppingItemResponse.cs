using SomaPraMim.Domain.Enums;

namespace SomaPraMim.Communication.Responses
{
    public class ShoppingItemResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public MeasurementUnit Unit { get; set; }
        public decimal Price { get; set; }
        public long ShoppingListId { get; set; }
    }
}