using Discord.Interactions;

namespace JustFilter.presentation.commands.entities.config;

public abstract class EditConfigNameModal : IModal
{
    public string Title => "Update Config";
    
    [InputLabel("New Config Name")]
    [ModalTextInput("new_config_name_id", placeholder: "Example: Religion")]
    public string NewConfigName { get; set; }
}