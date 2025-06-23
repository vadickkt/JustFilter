using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustFilter.infrastructure.datastore.mongo.deleted_messages;

public class DeletedMessageData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required string DeletedMessage { get; set; }
    public required string DeletingReason { get; set; }
}