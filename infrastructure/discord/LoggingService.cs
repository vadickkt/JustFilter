using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace JustFilter.infrastructure.discord;

public class LoggingService
{
    public LoggingService(DiscordSocketClient client, CommandService command)
    {
        client.Log += LogAsync;
        command.Log += LogAsync;
    }

    private Task LogAsync(LogMessage msg)
    {
        Console.WriteLine($"[{msg.Severity}] {msg.Source}: {msg.Message}");
        if (msg.Exception != null)
            Console.WriteLine(msg.Exception);
        return Task.CompletedTask;
    }
}