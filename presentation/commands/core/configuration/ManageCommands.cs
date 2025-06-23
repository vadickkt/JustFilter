using Discord;
using Discord.Interactions;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using JustFilter.infrastructure.datastore.redis;
using JustFilter.presentation.printers.setup;

namespace JustFilter.presentation.commands.core.configuration;

public class ManageCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigRepository _configRepository;
    private readonly ChannelRepository _channelRepository;
    private readonly RedisContext _redisContext;

    public ManageCommands(ConfigRepository configRepository, ChannelRepository channelRepository,
        RedisContext redisContext)
    {
        _configRepository = configRepository;
        _channelRepository = channelRepository;
        _redisContext = redisContext;
    }

    [SlashCommand("setup", "Setup JustFilter in a channel")]
    public async Task SetupAsync()
    {
        try
        {
            var configs = await _configRepository.GetAllConfigs(Context.Guild.Id);
            if (configs != null && configs.Count != 0)
            {
                await RespondAsync(
                    embed: SetupPrinter.BuildListOfAvailableConfigs(configs),
                    components: SetupPrinter.BuildSetupComponents(configs)
                );
            }
            else
            {
                await RespondAsync(
                    embed: SetupPrinter.BuildHelpMessage(),
                    components: SetupPrinter.BuildHelpButtons()
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in /setup: {ex}");
            await RespondAsync("Ein Fehler ist aufgetreten.");
        }
    }


    [SlashCommand("terminate", "Terminal Just Filter in a channel")]
    public async Task TerminateAsync()
    {
        var guildId = Context.Guild.Id;
        var channelId = Context.Channel.Id;

        await _channelRepository.DeleteConfigsInChannel(guildId, channelId);
        await _redisContext.RemoveConfigsAsync(guildId, channelId);
        await RespondAsync("Configs have been stopped");
    }
}