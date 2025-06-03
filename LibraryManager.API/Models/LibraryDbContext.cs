using Microsoft.EntityFrameworkCore;

namespace LibraryManager.API.Models;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
    
    public LibraryDbContext() { }
    public DbSet<Book> Books { get; set; }
   
    
    // Optional: Seed initial data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Clean Code", Author = "Robert Martin", ISBN = "9780132350884", IsAvailable= true },
            new Book { Id = 2, Title = "Design Patterns", Author = "Erich Gamma", ISBN = "9780201633610", IsAvailable= true}
        );
    }
}