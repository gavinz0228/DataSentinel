using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataSentinel.Infrastructure;
namespace DataSentinel.DataLayer{
    public class MongoDbDataRepository: IDataRepository
    {
        protected object collectionLock = new object();
        protected IOptions<AppConfig> _options;
        protected MongoClient _client = null;
        protected Dictionary<string, IMongoCollection<BsonDocument>> _collectionCache = new Dictionary<string, IMongoCollection<BsonDocument>>();
        public MongoDbDataRepository(IOptions<AppConfig> options){
            this._options = options;
            _client = new MongoDB.Driver.MongoClient(options.Value.ConnectionString);
        }
        public async Task Add(string collection, Stream stream){
            await GetCollection(collection).InsertOneAsync(await ReadStreamToBsonDocument(stream));
        }
        public async Task Save(string collection, Stream stream, string filter){
            await GetCollection(collection).ReplaceOneAsync(BsonDocument.Parse(filter), await ReadStreamToBsonDocument(stream));
        }
        public async Task<long> Delete(string collection, string filter){
            var deleteResult = await GetCollection(collection).DeleteManyAsync(BsonDocument.Parse(filter).ToBsonDocument());
            return deleteResult.DeletedCount;
        }
        public async Task<IList<Object>> Get(string collection, string filter){
            var cursor = await GetCollection(collection).FindAsync(BsonDocument.Parse(filter));
            var result = new List<object>();
            foreach( var doc in await cursor.ToListAsync())
                result.Add(doc.ToJson());
            return result;
        }
        protected IMongoCollection<BsonDocument> GetCollection(string collection)
        {
            if(!_collectionCache.ContainsKey(collection)){ 
                lock(collectionLock)
                {
                    if(!_collectionCache.ContainsKey(collection)){
                        _collectionCache.Add(
                            collection,
                             _client.GetDatabase(_options.Value.DatabaseName).GetCollection<BsonDocument>(collection)
                        );
                    }
                }
            }
            return _collectionCache[collection];
        }
        protected async Task<BsonDocument> ReadStreamToBsonDocument(Stream stream)
        {
            using(var streamReader = new StreamReader(stream))
            {
                return BsonDocument.Parse(await streamReader.ReadToEndAsync());
            }
        }
    }
}