using StackExchange.Redis;

namespace JustFilter.infrastructure.datastore.redis;

public class RedisContext(IConnectionMultiplexer redis) {
    private readonly IDatabase _redisDatabase = redis.GetDatabase();
    
    // Our redix cache: ServerId => List of ChannelId => List of Config
}