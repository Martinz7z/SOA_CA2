using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }

        // Foreign key for Author
        public int AuthorId { get; set; }
        public int? GenreId { get; set; }

        // Navigation property
        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }
    }
}