using System.Text;
using System.Text.Json;
using JustFilter.infrastructure.ai.model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace JustFilter.infrastructure.ai;

public class OllamaHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaHttpClient> _logger;

    public OllamaHttpClient(HttpClient httpClient, IConfiguration config, ILogger<OllamaHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var baseUrl = config["Ollama:BaseUrl"] ?? "http://localhost:11434/api/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        _logger.LogInformation("OllamaHttpClient initialized with BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
    }

    public async Task<OllamaResponseMetaData?> GenerateAsync(OllamaGenerateRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            _logger.LogInformation("Sending request to Ollama API: {RequestJson}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/generate", content);

            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response content: {ResponseContent}", responseContent);

            var ollamaGenerateResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseContent);
            return ollamaGenerateResponse == null ? null : JsonSerializer.Deserialize<OllamaResponseMetaData>(ollamaGenerateResponse.Response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating from Ollama API");
            throw;
        }
    }
}