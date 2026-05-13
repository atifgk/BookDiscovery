using BookDiscovery.Application.Interfaces;
using BookDiscovery.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookDiscovery.Application.Services
{
    public class BookSearchService : IBookSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly IAiQueryParser _parser;
        private readonly IBookRankingService _rankingService;
        private readonly IOpenLibraryEnrichmentService _enrichmentService;
        private readonly ILogger<BookSearchService> _logger;

        public BookSearchService(
            HttpClient httpClient,
            IAiQueryParser parser,
            IBookRankingService rankingService,
            IOpenLibraryEnrichmentService enrichmentService,
            ILogger<BookSearchService> logger)
        {
            _httpClient = httpClient;
            _parser = parser;
            _rankingService = rankingService;
            _enrichmentService = enrichmentService;
            _logger = logger;            
        }

        public async Task<List<BookInfo>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<BookInfo>();

            // ----------------------------
            // 1. AI INTENT EXTRACTION
            // ----------------------------
            var intent = await _parser.ExtractAsync(query);

            string searchQuery = BuildSearchQuery(query, intent);

            _logger.LogInformation(
                "SearchQuery: {SearchQuery}, Title: {Title}, Author: {Author}",
                searchQuery,
                intent?.Title,
                intent?.Author
            );

            // ----------------------------
            // 2. OPEN LIBRARY SEARCH
            // ----------------------------
            var url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(searchQuery)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<OpenLibrarySearchResponse>(json, options);

            if (data?.Docs == null || !data.Docs.Any())
                return new List<BookInfo>();

            
            var books = data.Docs
                .Select(book => new BookInfo
                {
                    Title = book.Title ?? "",
                    Author = book.AuthorNames?.FirstOrDefault() ?? "",
                    PublishedYear = book.FirstPublishYear?.ToString() ?? "",
                    CoverImage = book.CoverId != null
                        ? $"https://covers.openlibrary.org/b/id/{book.CoverId}-M.jpg"
                        : null,
                    OpenLibraryUrl = book.Key != null
                        ? $"https://openlibrary.org{book.Key}"
                        : null
                })
                .ToList();

            List<BookInfo> result;

            if (intent == null)
            {
                result = books.Take(5).ToList();

                foreach (var b in result)
                {
                    b.ShortInfo =
                        $"Matched from Open Library using raw query '{query}'.";
                }
            }
            else
            {
                result = _rankingService.Rank(intent, data.Docs);
            }

            
            result = await _enrichmentService.EnrichAsync(result);

            return result;
        }

        
        private static string BuildSearchQuery(string raw, BookQueryIntent? intent)
        {
            if (intent == null)
                return raw;

            if (!string.IsNullOrWhiteSpace(intent.Title))
                return intent.Title;

            if (!string.IsNullOrWhiteSpace(intent.Author))
                return intent.Author;

            if (intent.Keywords?.Any() == true)
                return string.Join(" ", intent.Keywords);

            return raw;
        }
    }
}