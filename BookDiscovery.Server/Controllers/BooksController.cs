using BookDiscovery.Server.Models;
using BookDiscovery.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookDiscovery.Server.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookSearchService _service;

        public BooksController(IBookSearchService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequestModel request)
        {
            var result = await _service.SearchAsync(request.Query);
            return Ok(result);
        }
    }
}
