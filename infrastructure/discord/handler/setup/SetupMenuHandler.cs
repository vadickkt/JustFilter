using Discord.Interactions;
using JustFilter.infrastructure.ai;
using JustFilter.infrastructure.ai.data;
using JustFilter.infrastructure.ai.model;
using JustFilter.infrastructure.database.mongo.channel;
using JustFilter.infrastructure.database.mongo.config;
using MongoDB.Bson;
using OllamaSharp;
using OllamaSharp.Models;

namespace JustFilter.infrastructure.discord.handler.setup;

public class SetupMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly ChannelRepository _channelRepository;

    public SetupMenuHandler(ConfigRepository configRepository, ChannelRepository channelRepository)
    {
        _configRepository = configRepository;
        _channelRepository = channelRepository;
    }

    [ComponentInteraction("setup_menu")]
    public async Task HandleSetupMenuAsync(string[] selectedConfigs)
    {
        var guild = Context.Guild;
        var channel = Context.Channel;

        var channelData = new ChannelData
        {
            ChannelId = channel.Id,
            Name = channel.Name,
        };
        
        var ids = selectedConfigs.ToList().ConvertAll(ObjectId.Parse);
        var configs = await _configRepository.GetManyConfigsByIds(ids);

        var addingResult = await _channelRepository.AddChannelIfNotExist(channelData);



    }
    /*
     * var httpClient = new HttpClient();
       const string baseUrl = "http://localhost:11434/api/";
       var ollamaHttpClient = new OllamaHttpClient(httpClient, baseUrl);
       var ollamaRequest = new OllamaGenerateRequest
       {
           model = "deepseek-r1:latest",
           prompt = OllamaConst.Prompt("Украина лучше России", configs),
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
       await RespondAsync(result);
     */
}