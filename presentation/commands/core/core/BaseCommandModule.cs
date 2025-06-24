using Discord.Interactions;
using Discord.WebSocket;
using JustFilter.presentation.printers.common;

namespace JustFilter.presentation.commands.core.core;

public abstract class BaseCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string RequiredRole = "JustFilterAdmin";

    public override async Task BeforeExecuteAsync(ICommandInfo command)
    {
        var user = Context.User as SocketGuildUser;

        if (user == null || user.Roles.All(role => role.Name != RequiredRole))
        {
            await RespondAsync(embed: BaseCommandPrinter.CreateMessageNoPermission(), ephemeral: true);
            throw new Exception("User does not have permission.");
        }

        await base.BeforeExecuteAsync(command);
    }
}
