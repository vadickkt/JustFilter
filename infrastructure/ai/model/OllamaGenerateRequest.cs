namespace JustFilter.infrastructure.ai.model;

public class OllamaGenerateRequest
{
    public required string model { get; set; }
    public required string prompt { get; set; }
    public required bool stream { get; set; }
    public FormatRequest? format { get; set; }
}

public class FormatRequest
{
    public string type { get; set; } = "object";
    public Dictionary<string, FormatProperty>? properties { get; set; }
    public string[]? required { get; set; }
}

public class FormatProperty
{
    public string type { get; set; }
}
