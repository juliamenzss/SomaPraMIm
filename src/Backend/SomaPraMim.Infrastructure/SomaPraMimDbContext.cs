using Microsoft.EntityFrameworkCore;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Infrastructure;

public class SomaPraMimDbContext(DbContextOptions<SomaPraMimDbContext> options)
    : DbContext(options), IUserContext, IShoppingListContext, IShoppingItemContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingItem> ShoppingItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ShoppingList>()
            .HasOne(sl => sl.User)
            .WithMany(sl => sl.ShoppingLists)
            .HasForeignKey(sl => sl.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ShoppingItem>()
            .HasOne(si => si.ShoppingList)
            .WithMany(sl => sl.ShoppingItems)
            .HasForeignKey(si => si.ShoppingListId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
