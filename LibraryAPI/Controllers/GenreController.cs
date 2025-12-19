using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly LibraryContext _context;

    public GenresController(LibraryContext context)
    {
        _context = context;
    }

    // GET: api/genres
    [HttpGet]
    public IActionResult GetGenres()
    {
        return Ok(_context.Genres.ToList());
    }

    // GET: api/genres/5
    [HttpGet("{id}")]
    public IActionResult GetGenre(int id)
    {
        var genre = _context.Genres.Find(id);
        if (genre == null) return NotFound();
        return Ok(genre);
    }

    // POST: api/genres
    [HttpPost]
    public IActionResult CreateGenre(Genre genre)
    {
        _context.Genres.Add(genre);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    // PUT: api/genres/5
    [HttpPut("{id}")]
    public IActionResult UpdateGenre(int id, Genre genre)
    {
        if (id != genre.Id) return BadRequest();
        _context.Entry(genre).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }

    // DELETE: api/genres/5
    [HttpDelete("{id}")]
    public IActionResult DeleteGenre(int id)
    {
        var genre = _context.Genres.Find(id);
        if (genre == null) return NotFound();
        _context.Genres.Remove(genre);
        _context.SaveChanges();
        return NoContent();
    }
}