using JustFilter.infrastructure.database.mongo.entities;
using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo.repository;

public class DiscordServersRepository
{
    private readonly MongoDbContext _dbContext;

    public DiscordServersRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddServer(DiscordServer server)
    {
        try
        {
            await _dbContext.DiscordServers.InsertOneAsync(server);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteServerByServerId(ulong serverId)
    {
        try
        {
            var filter = Builders<DiscordServer>.Filter.Eq(x => x.ServerId, serverId);
            await _dbContext.DiscordServers.DeleteOneAsync(filter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}