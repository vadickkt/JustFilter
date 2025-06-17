using JustFilter.infrastructure.database.mongo.entities;
using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo;

public class MongoDbContext(IMongoClient mongoClient)
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("just-filter");

    public IMongoCollection<DiscordServer> Servers => _database.GetCollection<DiscordServer>("servers");
}
