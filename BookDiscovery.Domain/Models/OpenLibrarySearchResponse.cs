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

        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        [JsonPropertyName("description")]
        public OpenLibraryDescription? Description { get; set; }

        public List<OpenLibraryWorkAuthor>? Authors { get; set; }

        [JsonPropertyName("first_publish_date")]
        public string? FirstPublishDate { get; set; }

        public List<string>? Subjects { get; set; }
    }

    public class OpenLibraryDescription
    {
        public string? Value { get; set; }
    }

    public class OpenLibraryWorkAuthor
    {
        public OpenLibraryAuthorReference? Author { get; set; }

        public OpenLibraryType? Type { get; set; }
    }

    public class OpenLibraryAuthorReference
    {
        public string? Key { get; set; }
    }

    public class OpenLibraryType
    {
        public string? Key { get; set; }
    }

    public class OpenLibraryAuthor
    {
        public string? Name { get; set; }

        public string? PersonalName { get; set; }

        public OpenLibraryDescription? Bio { get; set; }
    }
}
