namespace SomaPraMim.Communication.Requests.ShoppingListRequests
{
    public class ShoppingListCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
    }
}