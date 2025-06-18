using JustFilter.data.entities;
using JustFilter.infrastructure.database.mongo.entities;
using MongoDB.Bson;
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

    public async Task<UpdationResult> UpdateConfig(ChangeConfigData changeConfigData, ConfigData newConfig)
    {
        var result = await _dbContext.Configs.ReplaceOneAsync(x =>
                x.DiscordId == changeConfigData.DiscordId ||
                x.Name == changeConfigData.Name, newConfig
        );

        return result.IsAcknowledged ? UpdationResult.Updated : UpdationResult.NotUpdated;
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
}