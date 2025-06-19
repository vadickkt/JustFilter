using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.datastore.mongo.config;

public class ConfigData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required ulong DiscordId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}