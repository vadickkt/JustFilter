namespace JustFilter.infrastructure.datastore.mongo.deleted_messages;

public class DeletedMessageRepository
{
    private readonly MongoDbContext _dbContext;

    public DeletedMessageRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddDeletedMessage(DeletedMessageData deletedMessage)
    {
        await _dbContext.DeletedMessages.InsertOneAsync(deletedMessage);
    }
}