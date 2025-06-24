namespace JustFilter.infrastructure.ai.model;

public class OllamaGenerateRequest
{
    public required string Model { get; set; }
    public required string prompt { get; set; }
    public bool? stream { get; set; }
    public FormatRequest? format { get; set; }

    public static OllamaGenerateRequest Default(string prompt)
    {
        var ollamaRequest = new OllamaGenerateRequest
        {
            Model = "deepseek-r1:latest",
            prompt = prompt,
            stream = false,
            format = new FormatRequest
            {
                type = "object",
                properties = new Dictionary<string, FormatProperty>
                {
                    { "matches", new FormatProperty { type = "boolean" } },
                    { "category", new FormatProperty { type = new[] { "string", "null" } } },
                    { "reason", new FormatProperty { type = new[] { "string", "null" } } }
                },
                required = ["matches", "category", "reason"]
            }
        };
        return ollamaRequest;
    }
}

public class FormatRequest
{
    public string type { get; set; } = "object";
    public Dictionary<string, FormatProperty>? properties { get; set; }
    public string[]? required { get; set; }
}

public class FormatProperty
{
    public object type { get; set; }
}
