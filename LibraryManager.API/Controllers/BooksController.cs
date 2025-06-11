using Microsoft.AspNetCore.Mvc;
using LibraryManager.API.Models;
using LibraryManager.API.Repositories;

namespace LibraryManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _repository.GetAllBooks();
            return Ok(books ?? new List<Book>()); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _repository.GetBookById(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] Book book)
        {
            if (!ModelState.IsValid) // Checks data annotations automatically
            {
                return BadRequest(ModelState);
            }
    
            await _repository.AddBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id) return BadRequest();
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await _repository.UpdateBook(book);
            return NoContent();
        }
        
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _repository.GetBookById(id);
            if (book == null) return NotFound();
            
            await _repository.DeleteBook(id);
            return NoContent();
        }
    }
}