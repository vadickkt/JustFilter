namespace JustFilter.presentation.printers.general;

using Discord;

public static class HelpPrinter
{
    public static Embed BuildHelpMessage()
    {
        var embed = new EmbedBuilder()
            .WithTitle("JustFilter: Help")
            .WithColor(Color.Blue)
            .AddField("Configuration",
                "> **/setup**: Setup JustFilter in a channel\n" +
                "> **/terminate**: Stop all configs in a channel")
            .AddField("Management",
                "> **/config-add**: Add a new config\n" +
                "> **/config-delete**: Delete config\n" +
                "> **/config-update**: Update config\n" +
                "> **/config-list**: Get a list of configs\n")
            .AddField("General",
                "> **/help**: Show the help menu\n" +
                "> **/ping**: Get the bot's ping\n");

        return embed.Build();
    }
}
