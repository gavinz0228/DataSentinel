using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Core.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DataSentinel.Infrastructure;
namespace DataSentinel.DataLayer{
    public class MongoDbDataRepository: IDataRepository
    {
        protected const string FUNCTION_IS_IP_BLACKLISTED = "isIPBlacklisted";
        protected const string FUNCTION_LOG_WRONG_PASSWORD = "logWrongPassword";
        protected const string FUNCTION_REMOVE_WRONG_PASSWORD = "removeWrongPassword";
        protected object collectionLock = new object();
        protected IOptions<AppConfig> _options;
        protected MongoClient _client = null;
        protected IMongoDatabase _database;
        protected Dictionary<string, IMongoCollection<BsonDocument>> _collectionCache = new Dictionary<string, IMongoCollection<BsonDocument>>();
        protected HashSet<string> _reservedCollections = new HashSet<string>(new string[] {"settings", "blacklist"}, StringComparer.InvariantCultureIgnoreCase);
        public MongoDbDataRepository(IOptions<AppConfig> options){
            this._options = options;
            _client = new MongoDB.Driver.MongoClient(options.Value.ConnectionString);
            _database = _client.GetDatabase(_options.Value.DatabaseName);
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
                        if(!_reservedCollections.Contains(collection))
                            _collectionCache.Add(
                                collection,
                                _client.GetDatabase(_options.Value.DatabaseName).GetCollection<BsonDocument>(collection)
                            );
                        else
                            throw new Exception($"Reserved collection name {collection} is not allowed to access.");
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
        protected async Task<BsonValue> EvalAsync( string javascript)
        {
            var function = new BsonJavaScript(javascript);
            var op = new EvalOperation(this._database.DatabaseNamespace, function, null);

            using (var writeBinding = new WritableServerBinding(this._client.Cluster, new CoreSessionHandle(NoCoreSession.Instance)))
            {
                return await op.ExecuteAsync(writeBinding, CancellationToken.None);
            }
        }
        protected async Task<BsonValue> EvalFunctionAsync(string functionName, string [] parameters)
        {
            var script =functionName + "("+ string.Join(',', parameters.Select(e=> $"'{e}'" )) +")";
            return await this.EvalAsync(script);

        }
        public async Task<bool> IsBlacklisted(string ip){
            var result = await this.EvalFunctionAsync(FUNCTION_IS_IP_BLACKLISTED, new string[]{ip});
            return result.AsBoolean;
        }
        public async Task LogWrongPassword(string ip)
        {
            await this.EvalFunctionAsync(FUNCTION_LOG_WRONG_PASSWORD, new string[]{ip});
        }
        public async Task RemoveWrongPassword(string ip)
        {
            await this.EvalFunctionAsync(FUNCTION_REMOVE_WRONG_PASSWORD, new string[]{ip});
        }
    }
}