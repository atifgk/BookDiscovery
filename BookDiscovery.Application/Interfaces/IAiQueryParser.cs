using BookDiscovery.Domain.Models;

namespace BookDiscovery.Application.Interfaces
{
    public interface IAiQueryParser
    {
        Task<BookQueryIntent?> ExtractAsync(string query);
    }
}
