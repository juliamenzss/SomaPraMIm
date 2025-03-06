namespace SomaPraMim.Communication.Responses
{
    public class ShoppingListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public long UserId { get; set; }
    }
}