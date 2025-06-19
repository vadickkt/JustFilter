using Discord.Interactions;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.ai.model;
using JustFilter.infrastructure.database.mongo.config;
using OllamaSharp;
using OllamaSharp.Models;

namespace JustFilter.infrastructure.discord.handler.setup;

public class SetupMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public SetupMenuHandler(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    [ComponentInteraction("setup_menu")]
    public async Task HandleSetupMenuAsync(string[] selectedConfigs)
    {
        var httpClient = new HttpClient();
        const string baseUrl = "http://localhost:11434/api/";
        var ollamaHttpClient = new OllamaHttpClient(httpClient, baseUrl);
        var ollamaRequest = new OllamaGenerateRequest
        {
            model = "deepseek-r1:latest",
            prompt = "Привет, твоя задача фильтровать текст я тебе пишу сообщение а ты отвечаешь на политичискую оно тему или нет. Отвечай файлом JSON. Текст: Слава Украине",
            stream = false,
            format = new FormatRequest
            {
                type = "object",
                properties = new Dictionary<string, FormatProperty>
                {
                    { "isPolicy", new FormatProperty { type = "boolean" } },
                    { "explanation", new FormatProperty { type = "string" } }
                },
                required = ["isPolicy", "explanation"]
            }
        };
        var result = await ollamaHttpClient.GenerateAsync(ollamaRequest);
        Console.WriteLine(result);
    }

}