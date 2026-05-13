using BookDiscovery.Domain.Models;

namespace BookDiscovery.Application.Interfaces
{
    public interface IOpenLibraryEnrichmentService
    {
        Task<List<BookInfo>> EnrichAsync(List<BookInfo> books);
    }
}
