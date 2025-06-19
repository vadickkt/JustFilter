using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.database.mongo.channel;

public class ChannelData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required ulong ChannelId { get; set; }
    public required string Name { get; set; } 
}