using System.Text.Json;
using MongoDB.Bson;
using StackExchange.Redis;

namespace JustFilter.infrastructure.datastore.redis;

public class RedisContext
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public RedisContext(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private string GetServerChannelsKey(ulong serverId) => $"server:{serverId}:channels";
    private string GetChannelConfigsKey(ulong serverId, ulong channelId) => $"server:{serverId}:channel:{channelId}:configs";

    public async Task<List<ulong>> GetChannelsAsync(ulong serverId)
    {
        var key = GetServerChannelsKey(serverId);
        var redisValue = await _db.StringGetAsync(key);
        return redisValue.HasValue
            ? JsonSerializer.Deserialize<List<ulong>>(redisValue, _jsonOptions)
            : new List<ulong>();
    }

    public async Task SetChannelsAsync(ulong serverId, List<ulong> channels)
    {
        var key = GetServerChannelsKey(serverId);
        var json = JsonSerializer.Serialize(channels, _jsonOptions);
        await _db.StringSetAsync(key, json);
    }

    public async Task<List<ObjectId>> GetConfigsAsync(ulong serverId, ulong channelId)
    {
        var key = GetChannelConfigsKey(serverId, channelId);
        var redisValue = await _db.StringGetAsync(key);
        return redisValue.HasValue
            ? JsonSerializer.Deserialize<List<ObjectId>>(redisValue, _jsonOptions)
            : [];
    }

    public async Task SetConfigsAsync(ulong serverId, ulong channelId, List<ObjectId> configs)
    {
        var key = GetChannelConfigsKey(serverId, channelId);
        var json = JsonSerializer.Serialize(configs, _jsonOptions);
        await _db.StringSetAsync(key, json);
    }

    public async Task RemoveConfigsAsync(ulong serverId, ulong channelId)
    {
        var key = GetChannelConfigsKey(serverId, channelId);
        await _db.KeyDeleteAsync(key);
    }
}

/*
var redis = ConnectionMultiplexer.Connect("localhost");
   var context = new RedisContext(redis);
   
   // Добавление каналов к серверу
   await context.SetChannelsAsync("123", new List<string> { "general", "news" });
   
   // Добавление конфигов к каналу
   await context.SetConfigsAsync("123", "general", new List<Config>
   {
       new Config { Name = "FilterBadWords", Enabled = true },
       new Config { Name = "AutoDelete", Enabled = false }
   });
   
   // Получение
   var channels = await context.GetChannelsAsync("123");
   var configs = await context.GetConfigsAsync("123", "general");
   */