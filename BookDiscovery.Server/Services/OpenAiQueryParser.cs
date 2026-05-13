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

        const int maxRetries = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            _logger.LogInformation("OpenAI attempt {Attempt} for query: {Query}", attempt, query);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return ParseResponse(json);
            }

            var errorBody = await response.Content.ReadAsStringAsync();

            _logger.LogWarning(
                "OpenAI failed (Attempt {Attempt}) Status: {Status} Body: {Body}",
                attempt,
                response.StatusCode,
                errorBody
            );

            // 429 = rate limit → retry with backoff
            if ((int)response.StatusCode == 429)
            {
                var delay = attempt * 1000; // 1s, 2s, 3s
                await Task.Delay(delay);
                continue;
            }

            // 401/403 = stop immediately (bad key)
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError("OpenAI auth failed. Check API key/billing.");
            }

            // other errors
            _logger.LogError($"OpenAI error: {response.StatusCode} - {errorBody}");
        }

        _logger.LogError("OpenAI failed after multiple retries (rate limit or network issue).");

        return null;
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

        return JsonSerializer.Deserialize<BookQueryIntentModel>(
                   content,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? new BookQueryIntentModel();
    }
}