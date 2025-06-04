using LibraryManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Tests;

public class ControllerTestBase
{
    protected LibraryDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test
            .Options;
        return new LibraryDbContext(options);
    }
    
}