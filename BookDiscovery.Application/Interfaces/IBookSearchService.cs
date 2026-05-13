using BookDiscovery.Domain.Models;

namespace BookDiscovery.Application.Interfaces
{
    public interface IBookSearchService
    {
        Task<List<BookInfo>> SearchAsync(string query);
    }
}
