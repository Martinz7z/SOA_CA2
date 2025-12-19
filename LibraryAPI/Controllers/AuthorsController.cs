using LibraryAPI.DTOs;
using LibraryAPI.Models;
using LibraryAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        // GET: api/authors
        [HttpGet]
        [AllowAnonymous]  // Public access
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            var authors = await _authorRepository.GetAllAsync();
            var authorDTOs = authors.Select(a => new AuthorDTO
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                BookCount = a.Books?.Count ?? 0
            }).ToList();

            return Ok(authorDTOs);
        }

        // GET: api/authors/5
        [HttpGet("{id}")]
        [AllowAnonymous]  // Public access
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found" });
            }

            var authorDTO = new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                BookCount = author.Books?.Count ?? 0
            };

            return Ok(authorDTO);
        }

        // POST: api/authors
        [HttpPost]
        [Authorize]  // Requires authentication
        public async Task<ActionResult<AuthorDTO>> PostAuthor(CreateAuthorDTO createAuthorDTO)
        {
            var author = new Author
            {
                Name = createAuthorDTO.Name,
                Email = createAuthorDTO.Email
            };

            var createdAuthor = await _authorRepository.CreateAsync(author);

            var authorDTO = new AuthorDTO
            {
                Id = createdAuthor.Id,
                Name = createdAuthor.Name,
                Email = createdAuthor.Email,
                BookCount = 0  // New author has no books yet
            };

            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, authorDTO);
        }

        // PUT: api/authors/5
        [HttpPut("{id}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> PutAuthor(int id, UpdateAuthorDTO updateAuthorDTO)
        {
            var author = new Author
            {
                Name = updateAuthorDTO.Name,
                Email = updateAuthorDTO.Email
            };

            var updatedAuthor = await _authorRepository.UpdateAsync(id, author);
            if (updatedAuthor == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found" });
            }

            return NoContent();
        }

        // DELETE: api/authors/5
        [HttpDelete("{id}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var deleted = await _authorRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Author with ID {id} not found" });
            }

            return NoContent();
        }
    }
}