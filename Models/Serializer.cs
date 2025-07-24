using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace runSyncBackend.Models
{
    /// <summary>
    /// A custom serializer to handle mixed BsonType.String and BsonType.ObjectId
    /// for the Id field. This allows you to have simple string IDs (e.g., "1", "2")
    /// for seed data and standard ObjectIds for new data.
    /// </summary>
       public class StringOrObjectIdSerializer : SerializerBase<string>
    {
        /// <summary>
        /// This method is called when reading data from MongoDB.
        /// </summary>
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Null:
                    // If the ID in the DB is null, read it and return a null C# string.
                    context.Reader.ReadNull();
                    return null;
                case BsonType.ObjectId:
                    // If it's an ObjectId in the DB, read it and convert to a string.
                    return context.Reader.ReadObjectId().ToString();
                case BsonType.String:
                    // If it's already a string, just read it.
                    return context.Reader.ReadString();
                default:
                    // Throw an error for any other type.
                    throw new BsonSerializationException($"Cannot deserialize BsonType {bsonType} to String.");
            }
        }

        /// <summary>
        /// This method is called when writing data to MongoDB.
        /// </summary>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            // When writing, check if the string is a valid ObjectId.
            if (ObjectId.TryParse(value, out var objectId))
            {
                // If it is, write it as a proper ObjectId.
                context.Writer.WriteObjectId(objectId);
            }
            else
            {
                // Otherwise, write it as a simple string.
                context.Writer.WriteString(value);
            }
        }
    }
}