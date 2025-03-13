using SomaPraMim.Domain.Enums; 

namespace SomaPraMim.Communication.Requests.ShoppingItemRequests
{
    public class ShoppingItemCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public MeasurementUnit Unit { get; set; } 
        public long ShoppingListId { get; set; }
    }
}