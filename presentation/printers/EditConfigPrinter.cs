using Discord;
using JustFilter.data.utils;
using JustFilter.infrastructure.database.mongo.entities;

namespace JustFilter.presentation.printers;

public static class EditConfigPrinter
{
    

    public static Embed PrintUpdateMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Update Config")
            .WithColor(Color.Gold)
            .WithDescription("Select the config you want to update");

        return embedBuilder.Build();
    }

    public static MessageComponent BuildConfigEditComponents(List<ConfigData> configs)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("select_config_menu")
            .WithMinValues(1)
            .WithMaxValues(1);

        foreach (var config in configs)
        {
            menuBuilder.AddOption(config.Name, config.Id.ToString(), config.Description.Truncate(50));
        }

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        return builder.Build();
    }
}