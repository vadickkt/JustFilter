using System.Linq.Expressions;
using MongoDB.Driver;

namespace JustFilter.infrastructure.datastore.mongo.channel;

using Filter = Expression<Func<ChannelData, bool>>;

public class ChannelRepository
{
    private readonly MongoDbContext _dbContext;

    public ChannelRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<(ChannelData ChannelData, bool Existed)> AddChannelIfNotExistAsync(ChannelData channel)
    {
        var existingChannel = await _dbContext.Channels
            .Find(c => c.ChannelId == channel.ChannelId)
            .FirstOrDefaultAsync();

        if (existingChannel != null) return (existingChannel, true);

        await _dbContext.Channels.InsertOneAsync(channel);
        return (channel, false);
    }


    public async Task UpdateChannel(ChannelData channel)
    {
        Filter filter = c => c.ChannelId == channel.ChannelId && c.ServerId == channel.ServerId;
        await _dbContext.Channels.ReplaceOneAsync(filter, channel);
    }

    public async Task DeleteConfigsInChannel(ulong serverId, ulong channelId)
    {
        var filter = Builders<ChannelData>.Filter.Where(c => c.ServerId == serverId && c.ChannelId == channelId);
        var update = Builders<ChannelData>.Update.Set(c => c.ConfigsIds, []);

        await _dbContext.Channels.UpdateOneAsync(filter, update);
    }

}