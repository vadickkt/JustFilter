using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.datastore.mongo.channel;

public class ChannelData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required ulong ChannelId { get; set; }
    public required ulong ServerId { get; set; }
    public required string Name { get; set; } 
    public List<ObjectId> ConfigsIds { get; set; }
}