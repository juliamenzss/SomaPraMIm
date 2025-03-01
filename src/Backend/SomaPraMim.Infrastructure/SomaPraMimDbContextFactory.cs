using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SomaPraMim.Infrastructure;

public class SomaPraMimDbContextFactory : IDesignTimeDbContextFactory<SomaPraMimDbContext>
{
    public SomaPraMimDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<SomaPraMimDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new SomaPraMimDbContext(optionsBuilder.Options);
    }
}