using BookDiscovery.Application.Interfaces;
using BookDiscovery.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookDiscovery.Application.Services
{
    public class OpenLibraryEnrichmentService : IOpenLibraryEnrichmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenLibraryEnrichmentService> _logger;

        public OpenLibraryEnrichmentService(
            HttpClient httpClient,
            ILogger<OpenLibraryEnrichmentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<BookInfo>> EnrichAsync(List<BookInfo> books)
        {
            var enriched = new List<BookInfo>();

            foreach (var book in books)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(book.OpenLibraryUrl))
                    {
                        enriched.Add(book);
                        continue;
                    }

                    var workKey = ExtractWorkKey(book.OpenLibraryUrl);

                    if (string.IsNullOrWhiteSpace(workKey))
                    {
                        enriched.Add(book);
                        continue;
                    }

                    var work = await GetWorkAsync(workKey);

                    if (work != null)
                    {
                        // Primary author resolution
                        var primaryAuthor = await GetPrimaryAuthorAsync(work);

                        if (!string.IsNullOrWhiteSpace(primaryAuthor))
                        {
                            book.Author = primaryAuthor;
                        }

                        // Subtitle handling
                        if (!string.IsNullOrWhiteSpace(work.Subtitle))
                        {
                            book.ShortInfo +=
                                $". Subtitle variant: {work.Subtitle}";
                        }
                    }

                    enriched.Add(book);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to enrich book {Title}",
                        book.Title);

                    enriched.Add(book);
                }
            }

            return enriched
                .GroupBy(x => x.Title.Normalize())
                .Select(g => g.First())
                .Take(5)
                .ToList();
        }

        private async Task<OpenLibraryWork?> GetWorkAsync(string workKey)
        {
            var url = $"https://openlibrary.org{workKey}.json";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OpenLibraryWork>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private async Task<string?> GetPrimaryAuthorAsync(OpenLibraryWork work)
        {
            var authorKey = work.Authors?
                .FirstOrDefault()?
                .Author?
                .Key;

            if (string.IsNullOrWhiteSpace(authorKey))
                return null;

            var url = $"https://openlibrary.org{authorKey}.json";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var author = JsonSerializer.Deserialize<OpenLibraryAuthor>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return author?.Name;
        }

        private string ExtractWorkKey(string url)
        {
            try
            {
                var uri = new Uri(url);

                return uri.AbsolutePath;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}