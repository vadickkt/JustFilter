using System.Text.Json;
using JustFilter.data.converter;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using JustFilter.infrastructure.datastore.mongo.config;

namespace JustFilter.infrastructure.datastore.redis;

public class RedisContext
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisContext(IConnectionMultiplexer redis, ILogger<RedisContext> logger)
    {
        _db = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _jsonOptions.Converters.Add(new ObjectIdConverter());
    }

    private string GetServerChannelsKey(ulong serverId) => $"server:{serverId}:channels";
    private string GetChannelConfigsKey(ulong serverId, ulong channelId) => $"server:{serverId}:channel:{channelId}:configs";

    public async Task<List<ConfigData>> GetConfigsAsync(ulong serverId, ulong channelId)
    {
        var key = GetChannelConfigsKey(serverId, channelId);
        var redisValue = await _db.StringGetAsync(key);
        if (!redisValue.HasValue)
        {
            return new List<ConfigData>();
        }

        var configs = JsonSerializer.Deserialize<List<ConfigData>>(redisValue, _jsonOptions);
        return configs ?? new List<ConfigData>();
    }

    public async Task AddConfigsAsync(ulong serverId, ulong channelId, List<ConfigData> newConfigs)
    {
        var existingConfigs = await GetConfigsAsync(serverId, channelId);
        var existingIds = existingConfigs.Select(c => c.Id).ToHashSet();

        var addedConfigs = newConfigs.Where(c => !existingIds.Contains(c.Id)).ToList();
        if (addedConfigs.Count == 0)
        {
            return;
        }

        existingConfigs.AddRange(addedConfigs);
        await SetConfigsAndEnsureChannelAsync(serverId, channelId, existingConfigs);
    }

    public async Task UpdateConfigsAsync(ulong serverId, ulong channelId, List<ConfigData> configs)
    {
        await SetConfigsAndEnsureChannelAsync(serverId, channelId, configs);
    }

    public async Task RemoveConfigsAsync(ulong serverId, ulong channelId)
    {
        var configKey = GetChannelConfigsKey(serverId, channelId);
        await _db.KeyDeleteAsync(configKey);

        var serverKey = GetServerChannelsKey(serverId);
        var channels = await GetChannelsAsync(serverId);
        channels.Remove(channelId);

        if (channels.Count == 0)
        {
            await _db.KeyDeleteAsync(serverKey);
        }
        else
        {
            var channelsJson = JsonSerializer.Serialize(channels, _jsonOptions);
            await _db.StringSetAsync(serverKey, channelsJson);
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

    private async Task SetConfigsAndEnsureChannelAsync(ulong serverId, ulong channelId, List<ConfigData> configs)
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
