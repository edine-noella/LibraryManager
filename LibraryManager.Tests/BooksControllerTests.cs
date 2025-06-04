using LibraryManager.API.Controllers;
using LibraryManager.API.Models;
using LibraryManager.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibraryManager.Tests;

public class BooksControllerTests : ControllerTestBase
{
    private BooksController _controller;
    private LibraryDbContext _context;
    private IBookRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = GetDbContext();
        _repository = new BookRepository(_context);
        _controller = new BooksController(_repository);
    }

    [TearDown]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    // GET /api/books Tests -------------------------------------------

    [Test]
    public async Task GetBooks_ReturnsAllBooks()
    {
        // Arrange
        await _context.Books.AddRangeAsync(new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true },
            new Book { Id = 2, Title = "Test Book 2", Author = "whiteRose", ISBN = "9780132350987", IsAvailable = true }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBooks();

        // Assert 
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    
        var okResult = result.Result as OkObjectResult;
        var books = okResult.Value as IEnumerable<Book>;
    
        Assert.That(books, Is.Not.Null);
        Assert.That(books.Count(), Is.EqualTo(2));
        Assert.That(books.Any(b => b.Title == "Test Book 1"), Is.True);
    }

   
    // GET /api/books/{id} Tests ------------------------------------
    
    [Test]
    public async Task GetBook_ReturnsBook_WhenExists()
    {
        // Arrange
        var testBook = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true };
        await _context.Books.AddAsync(testBook);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBook(1);

        // Assert - Check ActionResult wrapper first
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    
        var okResult = result.Result as OkObjectResult;
        var book = okResult.Value as Book;
    
        Assert.That(book, Is.Not.Null);
        Assert.That(book.Title, Is.EqualTo("Test Book 1"));
    }
    
    [Test]
    public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int invalidId = 999;

        // Act
        var result = await _controller.GetBook(invalidId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }
    
    // POST /api/books Tests ------------------------------------
    
    [Test]
    public async Task PostBook_CreatesNewBook()
    {
        // Arrange
        var newBook = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true};

        // Act
        var result = await _controller.PostBook(newBook);

        // Assert
        var createdBook = (result.Result as CreatedAtActionResult)?.Value as Book;
        Assert.That(createdBook.Id, Is.GreaterThan(0));
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }
    
    [Test]
    public async Task PostBook_ReturnsBadRequest_WhenModelInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Title is required");
        var invalidBook = new Book { Author = "Author" }; // Missing title

        // Act
        var result = await _controller.PostBook(invalidBook);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    // PUT /api/books/{id} Tests-------------------------------------------
    
    [Test]
    public async Task PutBook_UpdatesExistingBook()
    {
        // Arrange
        var existingBook = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true };
        await _context.Books.AddAsync(existingBook);
        await _context.SaveChangesAsync();

        // Clear the tracking to avoid conflict
        _context.Entry(existingBook).State = EntityState.Detached;

        var updatedBook = new Book { Id = 1, Title = "Updated", Author = "suzume", ISBN = "9780132350675", IsAvailable = true };

        // Act
        var result = await _controller.PutBook(1, updatedBook);

        // Assert
        var bookInDb = await _context.Books.FindAsync(1);
        Assert.That(bookInDb.Title, Is.EqualTo("Updated"));
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
    
    [Test]
    public async Task PutBook_ReturnsBadRequest_WhenIdsDontMatch()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true};

        // Act
        var result = await _controller.PutBook(2, book);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }
    
    // DELETE /api/books/{id} Tests --------------------------
    
    [Test]
    public async Task DeleteBook_RemovesBook()
    {
        // Arrange
        var bookToDelete = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true };
        await _context.Books.AddAsync(bookToDelete);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteBook(1);

        // Assert
        var bookInDb = await _context.Books.FindAsync(1);
        Assert.That(bookInDb, Is.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
    
    [Test]
    public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int invalidId = 999;

        // Act
        var result = await _controller.DeleteBook(invalidId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}