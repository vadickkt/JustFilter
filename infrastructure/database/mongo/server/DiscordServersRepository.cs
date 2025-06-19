using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo.server;

public class DiscordServersRepository
{
    private readonly MongoDbContext _dbContext;

    public DiscordServersRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddServer(ServerData server)
    {
        await _dbContext.Servers.InsertOneAsync(server);
    }

    public async Task DeleteServerByServerId(ulong serverId)
    {
        var filter = Builders<ServerData>.Filter.Eq(x => x.ServerId, serverId);
        await _dbContext.Servers.DeleteOneAsync(filter);
    }
}