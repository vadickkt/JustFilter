using Discord;
using JustFilter.infrastructure.database.mongo.entities;

namespace JustFilter.presentation.printers;

public static class SetupPrinter
{
    public static Embed BuildListOfAvailableConfigs(List<ConfigData> configs)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Select a config to configure JustFilter in a channel")
            .WithColor(Color.Blue);

        for (var i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            embedBuilder.AddField(
                $"#{i + 1} • 🧾 `{config.Name}`",
                $"📄 **Description:** {config.Description}\n🔑 **ID:** `{config.Id}`",
                inline: false
            );
        }

        return embedBuilder.Build();
    }

    public static MessageComponent BuildSetupComponents(List<ConfigData> configs)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("setup_menu")
            .WithMinValues(1)
            .WithMaxValues(configs.Count);

        for (var i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            menuBuilder.AddOption($"#{i + 1} {config.Name}", config.Id.ToString());
        }

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);
            
        var buttons = BuildButtons();
        foreach (var button in buttons)
        {
            builder.WithButton(button);
        }

        return builder.Build();
    }

    private static List<ButtonBuilder> BuildButtons()
    {
        var addNewConfigButton = new ButtonBuilder()
            .WithStyle(ButtonStyle.Success);
        
        return [addNewConfigButton];
    }
}