using Microsoft.AspNetCore.Mvc;
using test.Models.DTOs;
using test.Repositories;

namespace test.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly IRepository _repository;

        public MyController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}/editions")]
        public async Task<IActionResult> GetEditions(int id)
        {
            if (!await _repository.DoesBookExist(id))
            {
                return NotFound("Book not found");
            }

            var bookEditionsDto = await _repository.GetEditions(id);

            return Ok(bookEditionsDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var success = await _repository.DeleteGenreByIdAsync(id);
            if (!success) return NotFound("Genre not found");

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookDetails(int id)
        {
            var bookDetails = await _repository.GetBookDetails(id);
            if (bookDetails == null) return NotFound("Book not found");

            return Ok(bookDetails);
        }

    }
}
