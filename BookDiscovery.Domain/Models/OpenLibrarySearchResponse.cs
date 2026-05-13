using System.Text.Json;
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

        [JsonConverter(typeof(OpenLibraryDescriptionConverter))]
        public string? Description { get; set; }

        public List<OpenLibraryWorkAuthor>? Authors { get; set; }

        [JsonPropertyName("first_publish_date")]
        public string? FirstPublishDate { get; set; }

        public List<string>? Subjects { get; set; }
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

        [JsonConverter(typeof(OpenLibraryDescriptionConverter))]
        public string? Description { get; set; }
    }

    public class OpenLibraryDescriptionConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Case 1: "description": "text"
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            // Case 2: "description": { "value": "text" }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);

                if (doc.RootElement.TryGetProperty("value", out var value))
                {
                    return value.GetString();
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
