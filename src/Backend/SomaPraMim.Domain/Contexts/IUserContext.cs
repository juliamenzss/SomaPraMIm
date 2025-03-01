using SomaPraMim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SomaPraMim.Domain.Contexts
{
    public interface IUserContext
    {
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}