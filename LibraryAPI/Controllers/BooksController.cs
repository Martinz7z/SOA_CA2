using LibraryAPI.DTOs;
using LibraryAPI.Models;
using LibraryAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/books - PUBLIC for testing
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _bookRepository.GetAllAsync();
            var bookDTOs = books.Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.Name ?? "Unknown"
            }).ToList();

            return Ok(bookDTOs);
        }

        // GET: api/books/5 - PUBLIC for testing
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDTO = new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                PublicationYear = book.PublicationYear,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name ?? "Unknown"
            };

            return Ok(bookDTO);
        }

        // POST: api/books - PROTECTED
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookDTO>> PostBook(CreateBookDTO createBookDTO)
        {
            var book = new Book
            {
                Title = createBookDTO.Title,
                ISBN = createBookDTO.ISBN,
                PublicationYear = createBookDTO.PublicationYear,
                AuthorId = createBookDTO.AuthorId
            };

            var createdBook = await _bookRepository.CreateAsync(book);

            var bookDTO = new BookDTO
            {
                Id = createdBook.Id,
                Title = createdBook.Title,
                ISBN = createdBook.ISBN,
                PublicationYear = createdBook.PublicationYear,
                AuthorId = createdBook.AuthorId,
                AuthorName = createdBook.Author?.Name ?? "Unknown"
            };

            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, bookDTO);
        }

        // PUT: api/books/5 - PROTECTED
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBook(int id, UpdateBookDTO updateBookDTO)
        {
            var book = new Book
            {
                Title = updateBookDTO.Title,
                ISBN = updateBookDTO.ISBN,
                PublicationYear = updateBookDTO.PublicationYear,
                AuthorId = updateBookDTO.AuthorId
            };

            var updatedBook = await _bookRepository.UpdateAsync(id, book);

            if (updatedBook == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/books/5 - PROTECTED
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}