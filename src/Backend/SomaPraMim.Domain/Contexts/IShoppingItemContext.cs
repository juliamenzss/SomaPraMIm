using Microsoft.EntityFrameworkCore;
using SomaPraMim.Domain.Entities;


namespace SomaPraMim.Domain.Contexts
{
    public interface IShoppingItemContext
    {
        DbSet<ShoppingItem> ShoppingItems { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    }
}