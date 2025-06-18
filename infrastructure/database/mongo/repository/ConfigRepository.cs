using JustFilter.data.entities;
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

    public async Task<UpdationResult> UpdateConfig(ChangeConfigData changeConfigData, ConfigData newConfig)
    {
        var result = await _dbContext.Configs.ReplaceOneAsync(x =>
                x.DiscordId == changeConfigData.DiscordId ||
                x.Name == changeConfigData.Name, newConfig
        );

        return result.IsAcknowledged ? UpdationResult.Updated : UpdationResult.NotUpdated;
    }

    public async Task<DeletionResult> DeleteConfig(ChangeConfigData changeConfigData)
    {
        var result = await _dbContext.Configs.DeleteOneAsync(c =>
            c.DiscordId == changeConfigData.DiscordId ||
            c.Name == changeConfigData.Name
        );

        return result.IsAcknowledged ? DeletionResult.Deleted : DeletionResult.NotDeleted;
    }

    public async Task<ConfigData> GetConfig(ChangeConfigData changeConfigData)
    {
        return await _dbContext.Configs.Find(x => 
            x.Name == changeConfigData.Name 
            || x.DiscordId == changeConfigData.DiscordId
        ).FirstOrDefaultAsync();
    }

public async Task<List<ConfigData>?> GetAllConfigs(ulong discordId)
    {
        return await _dbContext.Configs.Find(c => c.DiscordId == discordId).ToListAsync();
    }
}