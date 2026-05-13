using BookDiscovery.Server.Models;
using System.Text.Json;

namespace BookDiscovery.Server.Services
{
    public interface IBookSearchService
    {
        Task<List<BookResultModel>> SearchAsync(string query);
    }
    public class BookSearchService : IBookSearchService
    {
        private readonly HttpClient _httpClient;

        public BookSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BookResultModel>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<BookResultModel>();
            }

            var encodedQuery = Uri.EscapeDataString(query);

            var url = $"https://openlibrary.org/search.json?q={encodedQuery}";

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
                return new List<BookResultModel>();
            }

            var results = data.Docs
                .Take(5)
                .Select(book => new BookResultModel
                {
                    Title = book.Title ?? "Unknown Title",

                    Author = book.AuthorNames?.FirstOrDefault()
                               ?? "Unknown Author",

                    PublishedDate = book.FirstPublishYear?.ToString()
                                     ?? "Unknown",

                    ShortInfo = $"Matched from Open Library search for '{query}'.",

                    CoverImage = book.CoverId != null
                        ? $"https://covers.openlibrary.org/b/id/{book.CoverId}-M.jpg"
                        : null,

                    OpenLibraryUrl = book.Key != null
                        ? $"https://openlibrary.org{book.Key}"
                        : null
                })
                .ToList();

            return results;
        }
    }
}
