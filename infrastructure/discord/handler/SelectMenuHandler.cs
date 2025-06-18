using Discord.Interactions;
using JustFilter.infrastructure.database.mongo.repository;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler;

public class SelectMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public SelectMenuHandler(ConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    [ComponentInteraction("select_config_menu")]
    public async Task HandleConfigMenuAsync(string[] selectedConfig)
    {
        var configId = ObjectId.Parse(selectedConfig[0]);
        var oldConfig = _configRepository.GetConfigById(configId);
        
        await RespondAsync($"selectedConfig: {selectedConfig[0]}");
    }
}