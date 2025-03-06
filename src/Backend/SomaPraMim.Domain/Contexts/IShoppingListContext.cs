using Microsoft.EntityFrameworkCore;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Domain.Contexts
{
    public interface IShoppingListContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}