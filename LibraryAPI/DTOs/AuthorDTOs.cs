namespace LibraryAPI.DTOs
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<BookDTO> Books { get; set; } = new List<BookDTO>();
    }

    public class CreateAuthorDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateAuthorDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}