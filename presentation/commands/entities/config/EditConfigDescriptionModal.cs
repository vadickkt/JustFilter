using Discord.Interactions;

namespace JustFilter.presentation.commands.entities.config;

public class EditConfigDescriptionModal : IModal
{
    public string Title => "Update Config";
    
    [InputLabel("New Config Description")]
    [ModalTextInput("new_config_description_id", placeholder: "Example: Filter messages about religion")]
    public string NewConfigDescription { get; set; }
}