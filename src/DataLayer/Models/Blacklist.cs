using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataSentinel.DataLayer.Models{
    public enum BlacklistStatus{Inactive, Active, Removed}
    public class Blacklist{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}
        [BsonElement("IPAddress")]
        public string IPAddress {get; set;}
        [BsonElement("WrongPasswordTry")]
        public int WrongPasswordTry {get;set;}
        [BsonElement("LastTryTime")]
        public DateTime LastTryTime {get;set;}
        [BsonElement("BlacklistStatus")]
        public BlacklistStatus Status {get;set;}

    }
}