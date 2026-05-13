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

    public class OpenLibraryWork
    {
        public string? Title { get; set; }

        public string? Subtitle { get; set; }

        public string? Description { get; set; }

        public List<OpenLibraryWorkAuthor>? Authors { get; set; }
    }

    public class OpenLibraryWorkAuthor
    {
        public OpenLibraryAuthorReference? Author { get; set; }

        public string? Type { get; set; }
    }

    public class OpenLibraryAuthorReference
    {
        public string? Key { get; set; }
    }

    public class OpenLibraryAuthor
    {
        public string? Name { get; set; }

        public string? PersonalName { get; set; }

        public string? Bio { get; set; }
    }
}
