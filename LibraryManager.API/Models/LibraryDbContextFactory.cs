using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryManager.API.Models;

public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
     
        var connectionString = configuration.GetConnectionString("LibraryDb");
    
       var builder = new DbContextOptionsBuilder<LibraryDbContext>();
        builder.UseNpgsql(connectionString);
        return new LibraryDbContext(builder.Options);
    }
}