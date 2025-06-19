using System.Text;
using System.Text.Json;
using JustFilter.infrastructure.ai.model;
using Microsoft.Extensions.Configuration;

namespace JustFilter.infrastructure.ai;

public class OllamaHttpClient
{
    private readonly HttpClient _httpClient;

    public OllamaHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        var baseUrl = config["Ollama:BaseUrl"] ?? "http://localhost:11434/api/";
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<string> GenerateAsync(OllamaGenerateRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/generate", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}