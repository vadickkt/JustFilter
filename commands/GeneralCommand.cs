using Discord.Interactions;

namespace JustFilter.commands;

public class GeneralCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "Shows the help menu")]
    public async Task Help()
    {
        await RespondAsync("Do u need help?");
    }
    
    [SlashCommand("ping", "Get the bot's ping")]
    public async Task Ping()
    {
        var latency = Context.Client.Latency;
        await RespondAsync($"🏓 Pong! Latency: {latency}ms");
    }
}