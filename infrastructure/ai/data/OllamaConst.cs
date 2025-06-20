using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.infrastructure.ai.data;

public static class OllamaConst
{
    public static string Prompt(string message, List<ConfigData> configs)
    {
        var categories = string.Join("\n", configs.Select(c =>
            $"- Category: {c.Name}\n  Description: {c.Description}"));

        return
            "You are an AI that filters messages according to predefined categories.\n" +
            "Here is the list of categories to determine whether the message violates the rules:\n\n" +
            $"{categories}\n\n" +
            "Check the following message:\n" +
            $"\"{message}\"\n\n" +
            "If the message belongs to at least one of the specified categories, indicate the category and the reason. " +
            "If not, say that everything is fine.\n\n" +
            "Respond strictly in JSON format:\n" +
            "{\n" +
            "  \"matches\": true/false,\n" +
            "  \"category\": \"category name or null\",\n" +
            "  \"reason\": \"explanation why it falls under the category or null\"\n" +
            "}";
    }
}