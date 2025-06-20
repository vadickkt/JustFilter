using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.infrastructure.ai.data;

public static class OllamaConst
{
    public static string Prompt(string message, List<ConfigData> configs)
    {
        // Формуємо опис категорій, додаючи приклади в полі "policy"
        var categoriesText = string.Join("\n", configs.Select(c =>
        {
            string description = c.Description;

            // Якщо категорія - "policy", додаємо приклади в опис
            if (c.Name.ToLower() == "policy")
            {
                description += "\n  Examples:\n" +
                               "  - \"Biden is incompetent\"\n" +
                               "  - \"Putin is a dictator\"\n" +
                               "  - \"зеленський лох\"";
            }

            return $"- Category: {c.Name}\n  Description: {description}";
        }));

        // Формуємо повний prompt з динамічними категоріями та повідомленням
        return
            "You are an AI content filter. Your task is to check if a message violates the following policies:\n\n" +
            $"{categoriesText}\n\n" +
            $"Check the following message:\n\"{message}\"\n\n" +
            "If the message belongs to any of the categories, respond with JSON:\n" +
            "{\n" +
            "  \"matches\": true,\n" +
            "  \"category\": \"category name\",\n" +
            "  \"reason\": \"explanation why it falls under the category\"\n" +
            "}\n\n" +
            "If it does not belong to any category, respond with:\n" +
            "{\n" +
            "  \"matches\": false,\n" +
            "  \"category\": null,\n" +
            "  \"reason\": null\n" +
            "}";
    }
}
