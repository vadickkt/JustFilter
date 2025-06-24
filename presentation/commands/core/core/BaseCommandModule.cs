using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace JustFilter.presentation.commands.core.core;

public abstract class BaseCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string RequiredRole = "JustFilterAdmin";

    public override async Task BeforeExecuteAsync(ICommandInfo command)
    {
        var user = Context.User as SocketGuildUser;

        if (user == null || user.Roles.All(role => role.Name != RequiredRole))
        {
            var embed = new EmbedBuilder()
                .WithTitle("Failure")
                .WithColor(Color.Blue)
                .WithDescription(
                    "You do not have permission to execute this command\n" +
                    "You need **JustFilterAdmin** rule to run this bot"
                )
                .Build();
            await RespondAsync(embed: embed, ephemeral: true);
            throw new Exception("User does not have permission.");
        }

        await base.BeforeExecuteAsync(command);
    }
}
