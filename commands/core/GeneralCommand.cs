using Discord.Interactions;

namespace JustFilter.commands.core;

public class GeneralCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check bot latency")]
    public async Task PingAsync()
    {
        await RespondAsync($"🏓 Pong! Latency: {Context.Client.Latency}ms");
    }

    [SlashCommand("help", "Show help menu")]
    public async Task HelpAsync()
    {
        await RespondAsync("Need help? Use `/ping` or `/config-add`.");
    }
}