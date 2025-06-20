using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace JustFilter.infrastructure.datastore.redis;

public class RedisContext
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisContext> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public RedisContext(IConnectionMultiplexer redis, ILogger<RedisContext> logger)
    {
        _db = redis.GetDatabase();
        _logger = logger;
    }

    private string GetServerChannelsKey(ulong serverId) => $"server:{serverId}:channels";
    private string GetChannelConfigsKey(ulong serverId, ulong channelId) => $"server:{serverId}:channel:{channelId}:configs";

    public async Task<List<string>> GetConfigsAsync(ulong serverId, ulong channelId)
    {
        try
        {
            var key = GetChannelConfigsKey(serverId, channelId);
            var redisValue = await _db.StringGetAsync(key);
            if (!redisValue.HasValue)
            {
                _logger.LogInformation("No configs found for server {ServerId}, channel {ChannelId}", serverId, channelId);
                return new List<string>();
            }

            var configs = JsonSerializer.Deserialize<List<string>>(redisValue, _jsonOptions);
            _logger.LogInformation("Fetched {Count} configs for server {ServerId}, channel {ChannelId}", configs?.Count ?? 0, serverId, channelId);
            return configs ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configs for server {ServerId}, channel {ChannelId}", serverId, channelId);
            throw;
        }
    }

    public async Task AddConfigsAsync(ulong serverId, ulong channelId, List<string> newConfigs)
    {
        try
        {
            var existingConfigs = await GetConfigsAsync(serverId, channelId);
            var addedConfigs = newConfigs.Where(c => !existingConfigs.Contains(c)).ToList();

            if (addedConfigs.Count == 0)
            {
                _logger.LogInformation("No new configs to add for server {ServerId}, channel {ChannelId}", serverId, channelId);
                return;
            }

            existingConfigs.AddRange(addedConfigs);
            await SetConfigsAndEnsureChannelAsync(serverId, channelId, existingConfigs);
            _logger.LogInformation("Added {Count} new configs to server {ServerId}, channel {ChannelId}", addedConfigs.Count, serverId, channelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding configs for server {ServerId}, channel {ChannelId}", serverId, channelId);
            throw;
        }
    }

    public async Task UpdateConfigsAsync(ulong serverId, ulong channelId, List<string> configs)
    {
        try
        {
            await SetConfigsAndEnsureChannelAsync(serverId, channelId, configs);
            _logger.LogInformation("Updated configs for server {ServerId}, channel {ChannelId} with {Count} configs", serverId, channelId, configs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configs for server {ServerId}, channel {ChannelId}", serverId, channelId);
            throw;
        }
    }

    public async Task RemoveConfigsAsync(ulong serverId, ulong channelId)
    {
        try
        {
            var configKey = GetChannelConfigsKey(serverId, channelId);
            await _db.KeyDeleteAsync(configKey);

            var serverKey = GetServerChannelsKey(serverId);
            var channels = await GetChannelsAsync(serverId);
            channels.Remove(channelId);

            if (channels.Count == 0)
            {
                await _db.KeyDeleteAsync(serverKey);
                _logger.LogInformation("Removed channel {ChannelId} and server {ServerId} because no more channels remain", channelId, serverId);
            }
            else
            {
                var channelsJson = JsonSerializer.Serialize(channels, _jsonOptions);
                await _db.StringSetAsync(serverKey, channelsJson);
                _logger.LogInformation("Removed channel {ChannelId} from server {ServerId}", channelId, serverId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing configs for server {ServerId}, channel {ChannelId}", serverId, channelId);
            throw;
        }
    }

    private async Task<List<ulong>> GetChannelsAsync(ulong serverId)
    {
        var key = GetServerChannelsKey(serverId);
        var redisValue = await _db.StringGetAsync(key);
        return redisValue.HasValue
            ? JsonSerializer.Deserialize<List<ulong>>(redisValue, _jsonOptions)
            : new List<ulong>();
    }

    private async Task SetConfigsAndEnsureChannelAsync(ulong serverId, ulong channelId, List<string> configs)
    {
        var configKey = GetChannelConfigsKey(serverId, channelId);
        var configJson = JsonSerializer.Serialize(configs, _jsonOptions);
        await _db.StringSetAsync(configKey, configJson);

        var serverKey = GetServerChannelsKey(serverId);
        var channels = await GetChannelsAsync(serverId);

        if (!channels.Contains(channelId))
        {
            channels.Add(channelId);
            var channelsJson = JsonSerializer.Serialize(channels, _jsonOptions);
            await _db.StringSetAsync(serverKey, channelsJson);
        }
    }
}
