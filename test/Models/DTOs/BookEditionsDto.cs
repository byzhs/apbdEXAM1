namespace test.Models.DTOs
{
    public class BookEditionsDto
    {
        public List<BookEditionDto> BookEditions { get; set; } = new();
    }

    public class BookEditionDto
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string EditionTitle { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }
    
    public class BookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public List<string> Genres { get; set; }
    }

    public class AuthorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}