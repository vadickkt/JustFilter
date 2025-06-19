using JustFilter.infrastructure.database.mongo.server;
using JustFilter.infrastructure.datastore.mongo.channel;
using JustFilter.infrastructure.datastore.mongo.config;
using MongoDB.Driver;

namespace JustFilter.infrastructure.datastore.mongo;

public class MongoDbContext(IMongoClient mongoClient)
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("just-filter");

    public IMongoCollection<ServerData> Servers => _database.GetCollection<ServerData>("discord-servers");
    public IMongoCollection<ConfigData> Configs => _database.GetCollection<ConfigData>("configs");
    public IMongoCollection<ChannelData> Channels => _database.GetCollection<ChannelData>("channels");
}
