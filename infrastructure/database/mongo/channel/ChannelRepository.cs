using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo.channel;

public class ChannelRepository
{
    private readonly MongoDbContext _dbContext;

    public ChannelRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddingChanelResult> AddChannelIfNotExist(ChannelData channel)
    {
        var existingChannel = await _dbContext.Channels
            .Find(c => c.Id == channel.Id)
            .FirstOrDefaultAsync();

        if (existingChannel != null) return AddingChanelResult.AlreadyExists;
        await _dbContext.Channels.InsertOneAsync(channel);
        return AddingChanelResult.Created;
    }
}