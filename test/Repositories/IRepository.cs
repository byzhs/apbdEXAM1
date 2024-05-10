using test.Models.DTOs;

namespace test.Repositories;

public interface IRepository
{
    Task<bool> DoesBookExist(int id);
    Task<BookEditionsDto> GetEditions(int id);
    Task<bool> DeleteGenreByIdAsync(int id);
    Task<BookDetailsDto> GetBookDetails(int id);
}