using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.presentation.printers.setup;

namespace JustFilter.presentation.commands.core.configuration;

public class ManageCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly ChannelRepository _channelRepository;

    public ManageCommands(ConfigRepository configRepository, ChannelRepository channelRepository)
    {
        _configRepository = configRepository;
        _channelRepository = channelRepository;
    }
    
    [SlashCommand("setup", "Setup JustFilter in a channel")]
    public async Task SetupAsync()
    {
        // Баг, приложение не может получить список конфигов из за этого не отвечает вначале
        var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
        if (configs != null)
        {
            await RespondAsync(
                embed: SetupPrinter.BuildListOfAvailableConfigs(configs), 
                components: SetupPrinter.BuildSetupComponents(configs)
            );
        }
        else // TODO if else do something
        {
            await RespondAsync("dasdsadas");
        }
    }

    [SlashCommand("terminate", "Terminal Just Filter in a channel")]
    public async Task TerminateAsync()
    {
        var guildId = Context.Guild.Id;
        var channelId = Context.Channel.Id;
        
        await _channelRepository.DeleteConfigsInChannel(guildId, channelId);
        await RespondAsync("Configs have been stopped");
    }
}