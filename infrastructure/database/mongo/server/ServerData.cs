using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.database.mongo.server;

public class ServerData
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public required ulong ServerId { get; set; }
    public required string Name { get; set; } 
}