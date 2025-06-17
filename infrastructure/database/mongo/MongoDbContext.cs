using JustFilter.infrastructure.database.mongo.entities;
using MongoDB.Driver;

namespace JustFilter.infrastructure.database.mongo;

public class MongoDbContext(IMongoClient mongoClient)
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("just-filter");

    public IMongoCollection<ServerData> Servers => _database.GetCollection<ServerData>("discord-servers");
    public IMongoCollection<ConfigData> Configs => _database.GetCollection<ConfigData>("configs");
}
