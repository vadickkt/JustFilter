using System.Text.Json.Serialization;

namespace JustFilter.infrastructure.ai.model;

public class OllamaGenerateResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("response")]
    public string Response { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("done_reason")]
    public string DoneReason { get; set; }

    [JsonPropertyName("context")]
    public List<int> Context { get; set; }

    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }

    [JsonPropertyName("load_duration")]
    public long LoadDuration { get; set; }

    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; set; }

    [JsonPropertyName("prompt_eval_duration")]
    public long PromptEvalDuration { get; set; }

    [JsonPropertyName("eval_count")]
    public int EvalCount { get; set; }

    [JsonPropertyName("eval_duration")]
    public long EvalDuration { get; set; }
}
public class OllamaResponseMetaData
{
    [JsonPropertyName("matches")]
    public bool Matches { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }  // nullable by default

    [JsonPropertyName("reason")]
    public string Reason { get; set; }    // nullable by default
}
