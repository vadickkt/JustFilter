using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.presentation.printers;
using JustFilter.presentation.printers.config;
using MongoDB.Bson;

namespace JustFilter.infrastructure.discord.handler.config;

public class ConfigMenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;

    public ConfigMenuHandler(ConfigRepository configRepository)
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
        var config = await _configRepository.GetConfigById(configId);

        await RespondAsync(
            embed: EditConfigPrinter.PrintFinalUpdateMessage(config), 
            components: EditConfigPrinter.BuildFinalConfigEditComponents(config)
        );
    }
}