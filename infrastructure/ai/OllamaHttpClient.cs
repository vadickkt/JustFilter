using System.Text;
using System.Text.Json;
using JustFilter.infrastructure.ai.model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            var content = new StringContent(json, Encoding.UTF8, "application/json");
        
            var response = await _httpClient.PostAsync("/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var ollamaGenerateResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseContent);

            _logger.LogInformation("{Response}", ollamaGenerateResponse?.Response);

            return ollamaGenerateResponse == null ? null : 
                JsonSerializer.Deserialize<OllamaResponseMetaData>(ollamaGenerateResponse.Response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating from Ollama API");
            throw;
        }
    }
}