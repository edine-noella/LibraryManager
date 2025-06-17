using FluentAssertions;
using LibraryManager.API.Controllers;
using LibraryManager.API.Models;
using LibraryManager.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace LibraryManager.Tests;

public class BooksControllerTests
{
    private BooksController _controller;
    // private LibraryDbContext _context;
    // private IBookRepository _repository;
    private  Mock<IBookRepository> _bookRepositoryMock;


    [SetUp]
    public void Setup()
    {
        // _context = GetDbContext();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _controller = new BooksController(_bookRepositoryMock.Object);
    }

    // GET /api/books Tests -------------------------------------------
    [Test]
    public async Task GetBooks_ReturnsAllBooks()
    {
        // Arrange
        var testBooks = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true },
            new Book { Id = 2, Title = "Test Book 2", Author = "whiteRose", ISBN = "9780132350987", IsAvailable = true }
        };
        
        _bookRepositoryMock.Setup(x => x.GetAllBooks())
            .ReturnsAsync(testBooks);
        
        // Act
        var result = await _controller.GetBooks();

        // Assert 
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        var books = okResult.Value as IEnumerable<Book>;
        
        books.Should()
            .NotBeNull()
            .And.HaveCount(2)
            .And.Contain(b => b.Title == "Test Book 1");
    }

    // GET /api/books/{id} Tests ------------------------------------
    [Test]
    public async Task GetBook_ReturnsBook_WhenExists()
    {
        // Arrange
        var testBook = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true 
        };
        
        _bookRepositoryMock.Setup(x => x.GetBookById(1))
            .ReturnsAsync(testBook);

        // Act
        var result = await _controller.GetBook(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    
        var okResult = result.Result as OkObjectResult;
        var book = okResult.Value as Book;
    
        book.Should().NotBeNull();
        book.Title.Should().Be("Test Book 1");
        
        _bookRepositoryMock.Verify(x => x.GetBookById(1), Times.Once);
    }
    
    [Test]
    public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int invalidId = 999;

        // Act
        var result = await _controller.GetBook(invalidId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
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
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdBook = (result.Result as CreatedAtActionResult)?.Value as Book;
        createdBook.Id.Should().BePositive();
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
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    // PUT /api/books/{id} Tests-------------------------------------------
    [Test]
    public async Task PutBook_UpdatesExistingBook()
    {
        // Arrange
        var existingBook = new Book { Id = 1, Title = "Test Book 1", Author = "keke", ISBN = "9780132350675", IsAvailable = true };
        var updatedBook = new Book { Id = 1, Title = "Updated", Author = "keke", ISBN = "9780132350687", IsAvailable = true };

        _bookRepositoryMock.Setup(x => x.GetBookById(1))
            .ReturnsAsync(existingBook);
        
        _bookRepositoryMock.Setup(x => x.UpdateBook(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PutBook(1, updatedBook);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _bookRepositoryMock.Verify(x => x.UpdateBook(It.Is<Book>(b => b.Title == "Updated")), Times.Once);
    }
    
    [Test]
    public async Task PutBook_ReturnsBadRequest_WhenIdsDontMatch()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book 1", Author = "suzume", ISBN = "9780132350675", IsAvailable = true};

        // Act
        var result = await _controller.PutBook(2, book);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    // DELETE /api/books/{id} Tests --------------------------
    [Test]
    public async Task DeleteBook_RemovesBook()
    {
        // Arrange
        var bookToDelete = new Book { Id = 1, Title = "Test Book 1" };
    
        // Properly mock repository methods
        _bookRepositoryMock.Setup(x => x.GetBookById(1))
            .ReturnsAsync(bookToDelete);
    
        _bookRepositoryMock.Setup(x => x.DeleteBook(1))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _controller.DeleteBook(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        _bookRepositoryMock.Verify(x => x.DeleteBook(1), Times.Once);
    
        _bookRepositoryMock.Setup(x => x.GetBookById(1))
            .ReturnsAsync((Book)null);
    
        var bookInDb = await _bookRepositoryMock.Object.GetBookById(1);
        bookInDb.Should().BeNull();
    }
    
    [Test]
    public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int invalidId = 999;

        // Act
        var result = await _controller.DeleteBook(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}