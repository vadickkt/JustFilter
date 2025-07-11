using System.Text.Json;
using JustFilter.data.converter;
using JustFilter.infrastructure.datastore.mongo.config;
using MongoDB.Bson;
using StackExchange.Redis;

namespace JustFilter.infrastructure.datastore.redis;

public class RedisContext
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisContext(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _jsonOptions.Converters.Add(new ObjectIdConverter());
    }

    private static string GetServerChannelsKey(ulong serverId) => $"server:{serverId}:channels";
    private static string GetChannelConfigsKey(ulong serverId, ulong channelId) => $"server:{serverId}:channel:{channelId}:configs";
    
    public async Task RemoveConfigAsync(ulong serverId, ulong channelId, ObjectId configId)
    {
        var configs = await GetConfigsAsync(serverId, channelId);
        var updatedConfigs = configs.Where(c => c.Id != configId).ToList();

        if (updatedConfigs.Count == configs.Count) return;

        await SetConfigsAndEnsureChannelAsync(serverId, channelId, updatedConfigs);
    }

    public async Task UpdateConfigAsync(ulong serverId, ulong channelId, ConfigData updatedConfig)
    {
        var configs = await GetConfigsAsync(serverId, channelId);
        var index = configs.FindIndex(c => c.Id == updatedConfig.Id);

        if (index == -1) return;

        configs[index] = updatedConfig;
        await SetConfigsAndEnsureChannelAsync(serverId, channelId, configs);
    }


    public async Task<List<ConfigData>> GetConfigsAsync(ulong serverId, ulong channelId)
    {
        var key = GetChannelConfigsKey(serverId, channelId);
        var redisValue = await _db.StringGetAsync(key);
        if (!redisValue.HasValue)
        {
            return new List<ConfigData>();
        }

        var configs = JsonSerializer.Deserialize<List<ConfigData>>(redisValue, _jsonOptions);
        return configs ?? [];
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
            : [];
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
