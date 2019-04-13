using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataSentinel.DataLayer.Models{
    public class Setting{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
        [BsonElement("WrongPasswordLimit")]
        public int WrongPasswordLimit;
    }
}