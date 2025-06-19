using MongoDB.Bson;
using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo.config;

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

    public async Task UpdateConfig(ObjectId id, ConfigData newConfig)
    {
        await _dbContext.Configs.ReplaceOneAsync(x => x.Id == id, newConfig);
    }

    public async Task DeleteConfigByObjectId(ObjectId id)
    {
        await _dbContext.Configs.DeleteOneAsync(c => c.Id == id);
    }

    public async Task<ConfigData> GetConfigById(ObjectId id)
    {
        return await _dbContext.Configs.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<ConfigData>?> GetAllConfigs(ulong discordId)
    {
        return await _dbContext.Configs.Find(c => c.DiscordId == discordId).ToListAsync();
    }

    public async Task<List<ConfigData>> GetManyConfigsByIds(List<ObjectId> ids)
    {
        var configs = new List<ConfigData>();
        foreach (var id in ids)
        {
            var config = await _dbContext.Configs.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (config != null) configs.Add(config);
        }
        
        return configs;
    }
}