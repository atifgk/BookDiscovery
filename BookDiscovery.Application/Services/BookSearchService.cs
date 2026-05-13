using Microsoft.Extensions.Logging;
using BookDiscovery.Domain.Models;
using System.Text.Json;
using BookDiscovery.Application.Interfaces;

namespace BookDiscovery.Application.Services
{
    
    public class BookSearchService : IBookSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly IAiQueryParser _parser;
        private readonly IBookRankingService _rankingService;
        private readonly ILogger<BookSearchService> _logger;

        public BookSearchService(HttpClient httpClient, IAiQueryParser parser, IBookRankingService rankingService, ILogger<BookSearchService> logger)
        {
            _httpClient = httpClient;
            _parser = parser;
            _rankingService = rankingService;
            _logger = logger;
        }

        public async Task<List<BookInfo>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<BookInfo>();
            }

            var searchQuery = query;

            var intent = await _parser.ExtractAsync(query);

            if (intent != null)
            {
                searchQuery = intent.Title ?? intent.Author ?? string.Join(" ", intent.Keywords);

                _logger.LogInformation("Parsed search intent: Title='{Title}', Author='{Author}', Keywords='{Keywords}'", intent.Title, intent.Author, string.Join(", ", intent.Keywords));
            }
            else
            {
                _logger.LogWarning("No specific intent extracted from query. Using raw query for search.");
            }


            var url = $"https://openlibrary.org/search.json?q={searchQuery}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<OpenLibrarySearchResponse>(json, options);

            if (data?.Docs == null)
            {
                return new List<BookInfo>();
            }

            var filteredData = new List<BookInfo>();

            if (intent == null)
            {
                filteredData = data.Docs.Take(5)
                 .Select(book => new BookInfo
                 {
                     Title = book.Title ?? "",

                     Author = book.AuthorNames?.FirstOrDefault() ?? "",

                     PublishedYear = book.FirstPublishYear?.ToString() ?? "",

                     ShortInfo = $"Matched from Open Library search for '{query}'.",

                     CoverImage = book.CoverId != null
                         ? $"https://covers.openlibrary.org/b/id/{book.CoverId}-M.jpg"
                         : null,

                     OpenLibraryUrl = book.Key != null
                         ? $"https://openlibrary.org{book.Key}"
                         : null
                 })
                 .ToList();
            }
            else
            {
                filteredData = _rankingService.Rank(intent, data.Docs);
            }

            //Todo: add logic here /works/{work_id}.json/authors/{author_id}.json/authors/{author_id}/works.json

            return filteredData;
        }
    }
}
