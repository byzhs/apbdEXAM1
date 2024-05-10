using test.Models.DTOs;
using Microsoft.Data.SqlClient;
namespace test.Repositories
{
    public class Repository : IRepository
    {
        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> DoesBookExist(int id)
        {
            var query = "select 1 from books where PK = @Id";

            await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            
            var res = await command.ExecuteScalarAsync();

            return res is not null;
        }

        public async Task<BookEditionsDto> GetEditions(int id)
        {
            var query = @"
                            select be.pk as Id, 
                            b.title as BookTitle, 
                            be.edition_title as EditionTitle, 
                            p.name as Publisher, 
                            be.release_date as ReleaseDate
                            from books_editions be 
                            join books b on b.PK = be.FK_book 
                            join publishing_houses p on p.PK = be.FK_publishing_house 
                            where be.FK_book = @Id";

            await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var bookEditions = new BookEditionsDto();

            var idOrdinal = reader.GetOrdinal("Id");
            var bookTitleOrdinal = reader.GetOrdinal("BookTitle");
            var editionTitleOrdinal = reader.GetOrdinal("EditionTitle");
            var publisherOrdinal = reader.GetOrdinal("Publisher");
            var releaseDateOrdinal = reader.GetOrdinal("ReleaseDate");

            while (await reader.ReadAsync())
            {
                bookEditions.BookEditions.Add(new BookEditionDto
                {
                    Id = reader.GetInt32(idOrdinal),
                    BookTitle = reader.GetString(bookTitleOrdinal),
                    EditionTitle = reader.GetString(editionTitleOrdinal),
                    Publisher = reader.GetString(publisherOrdinal),
                    ReleaseDate = reader.GetDateTime(releaseDateOrdinal)
                });
            }

            if (bookEditions.BookEditions.Count == 0)
                throw new Exception("No book found");

            return bookEditions;
        }
        public async Task<bool> DeleteGenreByIdAsync(int id)
        {
            var checkQuery = "select 1 from genres where PK = @Id";

            var deleteQuery = "delete from genres where PK = @Id";

            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand(checkQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            if (result == null) return false;

            command.CommandText = deleteQuery;
            await command.ExecuteNonQueryAsync();

            return true;
        }
        
        public async Task<BookDetailsDto> GetBookDetails(int id)
        {
            var query = @"
                        select b.PK as BookId, b.title as Title,
                        a.first_name as AuthorFirstName, a.last_name as AuthorLastName,
                        g.name as GenreName
                        from books b
                        join books_authors ba on b.PK = ba.FK_book
                        join authors a on ba.FK_author = a.PK
                        join books_genres bg on b.PK = bg.FK_book
                        join genres g on bg.FK_genre = g.PK
                        where b.PK = @Id";
    
            await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var bookDetails = new BookDetailsDto { Authors = new List<AuthorDto>(), Genres = new List<string>() };

            while (await reader.ReadAsync())
            {
                if (bookDetails.Id == 0)
                {
                    bookDetails.Id = reader.GetInt32(reader.GetOrdinal("BookId"));
                    bookDetails.Title = reader.GetString(reader.GetOrdinal("Title"));
                }

                var authorFirstName = reader.GetString(reader.GetOrdinal("AuthorFirstName"));
                var authorLastName = reader.GetString(reader.GetOrdinal("AuthorLastName"));
                var genreName = reader.GetString(reader.GetOrdinal("GenreName"));

                bookDetails.Authors.Add(new AuthorDto { FirstName = authorFirstName, LastName = authorLastName });
                bookDetails.Genres.Add(genreName);
            }

            if (bookDetails.Id == 0)
            {
                return null;
            }

            return bookDetails;
        }

    }
}