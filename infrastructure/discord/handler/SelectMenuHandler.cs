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
    
    [ComponentInteraction("delete_config_menu")]
    public async Task HandleDeleteConfigMenuAsync(string[] selectedConfig)
    {
        foreach (var configId in selectedConfig)
        {
            var objectId = ObjectId.Parse(configId);
            await _configRepository.DeleteConfigByObjectId(objectId);
        }
        await RespondAsync("These configs have been successfully deleted");
    }
    
    [ComponentInteraction("edit_config_menu")]
    public async Task HandleUpdateConfigMenuAsync(string[] selectedConfig)
    {
        var configId = ObjectId.Parse(selectedConfig[0]);
        var oldConfig = _configRepository.GetConfigById(configId);

        await RespondAsync($"selectedConfig: {selectedConfig[0]}");
    }

    [ComponentInteraction("setup_menu")]
    public async Task HandleSetupMenuAsync(string[] selectedConfigs)
    {
        // TODO send it to ai and handle then
        await RespondAsync($"selectedConfigs: {string.Join(",", selectedConfigs)}");
    }

}