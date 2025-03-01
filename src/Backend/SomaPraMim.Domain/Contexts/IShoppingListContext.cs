using Microsoft.EntityFrameworkCore;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Domain.Contexts
{
    public interface IShoppingListContext
    {
        DbSet<ShoppingList> ShoppingLists { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}