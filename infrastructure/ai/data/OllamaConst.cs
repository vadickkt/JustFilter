using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.infrastructure.ai.data;

public static class OllamaConst
{

    public static string Prompt(string message, List<ConfigData> configs)
    {
        var categories = string.Join("\n", configs.Select(c =>
            $"- Категория: {c.Name}\n  Описание: {c.Description}"));

        return
            "Ты — ИИ, фильтрующий сообщения по заранее заданным категориям.\n" +
            "Вот список категорий, по которым нужно определять, нарушает ли сообщение правила:\n\n" +
            $"{categories}\n\n" +
            "Проверь следующее сообщение:\n" +
            $"\"{message}\"\n\n" +
            "Если сообщение относится хотя бы к одной из указанных категорий — укажи категорию и причину. " +
            "Если нет — скажи, что всё в порядке.\n\n" +
            "Ответь строго в формате JSON:\n" +
            "{\n" +
            "  \"matches\": true/false,\n" +
            "  \"category\": \"название категории или null\",\n" +
            "  \"reason\": \"описание, почему отнесено к категории или null\"\n" +
            "}";
    }


    
}