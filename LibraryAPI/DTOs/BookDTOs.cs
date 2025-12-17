namespace LibraryAPI.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }

    public class CreateBookDTO
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int AuthorId { get; set; }
    }

    public class UpdateBookDTO
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int AuthorId { get; set; }
    }
}