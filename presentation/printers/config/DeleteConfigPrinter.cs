using Discord;
using JustFilter.data.utils;
using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.presentation.printers.config;

public static class DeleteConfigPrinter
{
    public static Embed CreateFirstConfig()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Failure")
            .WithColor(Color.Red)
            .WithDescription("First, you need to create a config");

        return embedBuilder.Build();
    }
    
    public static Embed PrintDeleteMessage()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Delete Config")
            .WithColor(Color.Red)
            .WithDescription("Select the config you want to delete");

        return embedBuilder.Build();
    }

    public static Embed PrintConfigWasDeleted()
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("These configs have been successfully deleted")
            .WithColor(Color.Green)
            .Build();
        
        return embedBuilder;
    }

    public static MessageComponent BuildConfigDeleteComponents(List<ConfigData> configs)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a config")
            .WithCustomId("delete_config_menu")
            .WithMinValues(1)
            .WithMaxValues(configs.Count);

        foreach (var config in configs)
        {
            menuBuilder.AddOption(config.Name, config.Id.ToString(), config.Description.Truncate(50));
        }

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        return builder.Build();
    }
}