using Discord;
using JustFilter.data.utils;
using JustFilter.infrastructure.database.mongo.entities;

namespace JustFilter.presentation.printers;

public static class EditConfigPrinter 
{
    public static Embed PrintStartUpdateMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Update Config")
            .WithColor(Color.Gold)
            .WithDescription("Select the config you want to update");

        return embedBuilder.Build();
    }

    public static MessageComponent BuildStartConfigEditComponents(List<ConfigData> configs)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("edit_config_menu")
            .WithMinValues(1)
            .WithMaxValues(1);

        foreach (var config in configs)
        {
            menuBuilder.AddOption(config.Name, config.Id.ToString(), config.Description.Truncate(50));
        }

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        return builder.Build();
    }

    public static Embed PrintFinalUpdateMessage(ConfigData config)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Selected Config")
            .WithColor(Color.Gold);

        embedBuilder.AddField(
            name: $"**Name:** `{config.Name}`",
            value: $"**Description:** `{config.Description}`\n" +
                   $"**ConfigId:** ${config.Id}",
            inline: false
        );

        return embedBuilder.Build();
    }

    public static MessageComponent BuildFinalConfigEditComponents(ConfigData config)
    {
        var editNameButton = new ButtonBuilder()
            .WithCustomId($"edit_config_name_button:{config.Id}")
            .WithLabel("Edit Name")
            .WithStyle(ButtonStyle.Secondary);

        var editDescriptionButton = new ButtonBuilder()
            .WithCustomId($"edit_config_description_button:{config.Id}")
            .WithLabel("Edit Description")
            .WithStyle(ButtonStyle.Secondary);

        var builder = new ComponentBuilder()
            .WithButton(editNameButton)
            .WithButton(editDescriptionButton);

        return builder.Build();
    }

    public static Modal PrintEditConfigNameMessage(string configId)
    {
        var modal = new ModalBuilder()
            .WithTitle("Update Config")
            .WithCustomId($"new_config_name_modal:{configId}")
            .AddTextInput("New Config Name", "new_config_name_id", placeholder: "Example: Religion");

        return modal.Build();
    }
    
    public static Modal PrintEditConfigDescriptionMessage(string configId)
    {
        var modal = new ModalBuilder()
            .WithTitle("Update Config")
            .WithCustomId($"new_config_description_modal:{configId}")
            .AddTextInput("New Config Description", "new_config_description_id", TextInputStyle.Paragraph, 
                placeholder: "Filter messages about religion");

        return modal.Build();
    }
}