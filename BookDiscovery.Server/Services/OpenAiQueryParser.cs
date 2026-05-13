using BookDiscovery.Server.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public interface IAiQueryParser
{
    Task<BookQueryIntentModel?> ExtractAsync(string query);
}

public class OpenAiQueryParser : IAiQueryParser
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<OpenAiQueryParser> _logger;

    public OpenAiQueryParser(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<OpenAiQueryParser> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<BookQueryIntentModel?> ExtractAsync(string query)
    {
        try
        {
            var apiKey = _config["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("OpenAI API key is missing.");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new
                {
                    role = "system",
                    content =
                    """
                    You extract structured book search data from messy user queries.

                    Return ONLY valid JSON:
                    {
                      "title": string|null,
                      "author": string|null,
                      "keywords": string[]
                    }

                    Rules:
                    - Normalize titles and authors
                    - Fix typos when obvious
                    - If unsure, leave null
                    """
                },
                new
                {
                    role = "user",
                    content = query
                }
            },
                temperature = 0
            };

            var requestJson = JsonSerializer.Serialize(requestBody);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return ParseResponse(json);
            }

            var errorBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError("OpenAI auth failed. Check API key/billing.");
            }

            // other errors
            _logger.LogError($"OpenAI error: {response.StatusCode} - {errorBody}");

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return null;
        }
    }

    private BookQueryIntentModel ParseResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
            return new BookQueryIntentModel();

        var result = JsonSerializer.Deserialize<BookQueryIntentModel>(
                   content,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? new BookQueryIntentModel();

        result.IsAIExtracted = true;

        return result;
    }
}