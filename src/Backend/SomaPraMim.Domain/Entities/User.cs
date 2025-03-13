namespace SomaPraMim.Domain.Entities
{
    public class User : ModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<ShoppingList> ShoppingLists { get; set; }
    }
}