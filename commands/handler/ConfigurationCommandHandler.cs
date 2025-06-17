using Discord.Interactions;
using JustFilter.commands.entities;

namespace JustFilter.commands.handler;

public class ConfigurationCommandHandler : InteractionModuleBase<SocketInteractionContext>
{
    [ModalInteraction("new_config")]
    public async Task HandleConfigModalAsync(ConfigModal modal)
    {
        //TODO add config to table by ServerId
        await RespondAsync(
            $"**Title**: {modal.Title}\n" +
            $"**Config Name**: {modal.ConfigName}\n" +
            $"**Config Description**: {modal.ConfigDescription}"
        );
    }
}
