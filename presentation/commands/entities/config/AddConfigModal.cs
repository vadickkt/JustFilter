using Discord;
using Discord.Interactions;

namespace JustFilter.presentation.commands.entities.config;

public abstract class AddConfigModal : IModal
{
    public string Title => "Add Config";

    [InputLabel("Config Name")]
    [ModalTextInput("config_name", placeholder: "Example: Religion")]
    public string ConfigName { get; set; }

    [InputLabel("Config Description")]
    [ModalTextInput("config_description", TextInputStyle.Paragraph, placeholder: "Filter messages about religion")]
    public string ConfigDescription { get; set; }
}
