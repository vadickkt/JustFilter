using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.infrastructure.ai.data;

public static class OllamaConst
{

    public static string Prompt(string message, List<ConfigData> configs)
    {
        return $"Привет, твоя задача фильтровать текст я тебе пишу сообщение а ты отвечаешь на политичискую оно тему или нет. " +
               $"Отвечай файлом JSON. Текст: ${message}";    
    }
    
}