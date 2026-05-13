using BookDiscovery.Domain.Models;

namespace BookDiscovery.Application.Interfaces
{
    public interface IBookRankingService
    {
        List<BookInfo> Rank(BookQueryIntent query, List<OpenLibraryBookDoc> candidates);
    }
}
