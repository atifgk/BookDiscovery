using BookDiscovery.Application;
using BookDiscovery.Application.Interfaces;
using BookDiscovery.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

public class GeminiQueryParser : IAiQueryParser
{
    private readonly HttpClient _httpClient;
    private readonly AppConfiguration _config;
    private readonly ILogger<GeminiQueryParser> _logger;

    public GeminiQueryParser(
        HttpClient httpClient,
        AppConfiguration config,
        ILogger<GeminiQueryParser> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<BookQueryIntent?> ExtractAsync(string query)
    {
        try
        {
            var apiKey = _config.GeminiApiKey;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("Gemini API key is missing.");
                return null;
            }

            var prompt = """
You extract structured book search data from messy user queries.

Return ONLY valid JSON:

{
  "title": null,
  "author": null,
  "keywords": []
}

Rules:
- Normalize titles and authors
- Fix obvious typos
- If unsure, use null
- Keep keywords short and meaningful

User query:
""";

            var requestBody = new
            {
                contents = new[]
                {
        new
        {
            role = "user",
            parts = new[]
            {
                new
                {
                    text = prompt + query
                }
            }
        }
    },
                generationConfig = new
                {
                    temperature = 0
                }
            };

            var url =
$"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var requestJson = JsonSerializer.Serialize(requestBody);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Gemini error: {response.StatusCode} - {error}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            return ParseResponse(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
    private BookQueryIntent ParseResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
            return new BookQueryIntent();

        
        content = content.Trim();

        if (content.StartsWith("```"))
        {
            var firstNewLine = content.IndexOf('\n');
            var lastBackticks = content.LastIndexOf("```");

            if (firstNewLine > -1 && lastBackticks > firstNewLine)
            {
                content = content.Substring(firstNewLine, lastBackticks - firstNewLine).Trim();
            }
        }

        var result = JsonSerializer.Deserialize<BookQueryIntent>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new BookQueryIntent();

        result.IsAIExtracted = true;

        return result;
    }
}