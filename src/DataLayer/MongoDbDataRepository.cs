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
using DataSentinel.DataLayer.Models;
namespace DataSentinel.DataLayer{
    public class MongoDbDataRepository: IDataRepository
    {
        protected const string COLLECTION_SETTING = "setting";
        protected const string COLLECTION_BLACKLIST = "blacklist";
        protected IMongoCollection<Setting> _settingCollection;
        protected IMongoCollection<Blacklist> _blacklistCollection;
        protected object collectionLock = new object();
        protected IOptions<AppConfig> _options;
        protected MongoClient _client = null;
        protected IMongoDatabase _database = null;
        protected Setting _setting = null;
        protected Dictionary<string, IMongoCollection<BsonDocument>> _collectionCache = new Dictionary<string, IMongoCollection<BsonDocument>>();
        protected HashSet<string> _reservedCollections = new HashSet<string>(new string[] {COLLECTION_SETTING, COLLECTION_BLACKLIST}, StringComparer.InvariantCultureIgnoreCase);
        public MongoDbDataRepository(IOptions<AppConfig> options){
            this._options = options;
            this._client = new MongoDB.Driver.MongoClient(options.Value.ConnectionString);
            this._database = _client.GetDatabase(_options.Value.DatabaseName);            
            this._blacklistCollection = this._database.GetCollection<Blacklist>(COLLECTION_BLACKLIST);
            this._settingCollection = this._database.GetCollection<Setting>(COLLECTION_SETTING);

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
            var cursor = await this._blacklistCollection.FindAsync(b => b.IPAddress == ip && b.Status == BlacklistStatus.Active);
            var item = await cursor.SingleOrDefaultAsync();
            return item != null;
        }
        public async Task LogWrongPassword(string ip)
        {
            if(this._setting==null)
            {
                var settingCur = await this._settingCollection.FindAsync(_ => true);
                this._setting = await settingCur.SingleOrDefaultAsync();
            }
            var cursor = await _blacklistCollection.FindAsync(b => b.IPAddress == ip && b.Status == BlacklistStatus.Inactive);
            var existing = await cursor.SingleOrDefaultAsync();
            if(existing==null){
                await this._blacklistCollection.InsertOneAsync(new Blacklist(){IPAddress = ip, Status= BlacklistStatus.Inactive, WrongPasswordTry = 1, LastTryTime = DateTime.Now });
            }
            else{
                if(existing.WrongPasswordTry < this._setting.WrongPasswordLimit){
                    var update = Builders<Blacklist>.Update.Set(s => s.WrongPasswordTry, existing.WrongPasswordTry + 1)
                        .Set(s => s.LastTryTime, DateTime.Now);
                    await this._blacklistCollection.UpdateOneAsync(b => b.Id == existing.Id, update);
                }
                else{
                    var update = Builders<Blacklist>.Update.Set(s => s.Status, BlacklistStatus.Active)
                        .Set(s => s.LastTryTime, DateTime.Now);
                    await this._blacklistCollection.UpdateOneAsync(b => b.Id == existing.Id, update);
                }
            }
        }
        public async Task RemoveWrongPassword(string ip)
        {
            var cursor = await _blacklistCollection.FindAsync(b => b.IPAddress == ip && b.Status == BlacklistStatus.Inactive);
            var existing = await cursor.SingleOrDefaultAsync();
            if(existing!=null){
                var update = Builders<Blacklist>.Update.Set(s => s.WrongPasswordTry, 0)
                    .Set(s => s.LastTryTime, DateTime.Now);
                await this._blacklistCollection.UpdateOneAsync(b => b.Id == existing.Id, update);
            }
        }
    }
}