using JustFilter.infrastructure.database.mongo.entities;
using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo.repository;

public class ConfigRepository
{
    private readonly MongoDbContext _dbContext;

    public ConfigRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddConfig(ConfigData config)
    {
        await _dbContext.Configs.InsertOneAsync(config);
    }

    public async Task<List<ConfigData>> GetAllConfigs()
    {
        return await _dbContext.Configs.Find(_ => true).ToListAsync();
    }
}