using System.Text.Json.Serialization;

namespace BookDiscovery.Domain.Models
{
    public class OpenLibrarySearchResponse
    {
        [JsonPropertyName("docs")]
        public List<OpenLibraryBookDoc> Docs { get; set; } = new();
    }

    public class OpenLibraryBookDoc
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("author_name")]
        public List<string>? AuthorNames { get; set; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }

        [JsonPropertyName("cover_i")]
        public int? CoverId { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }
}
