using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.database.mongo.entities;

public class ConfigData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}