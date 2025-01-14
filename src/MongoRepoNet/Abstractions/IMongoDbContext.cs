using MongoDB.Driver;

namespace MongoRepoNet.Abstractions;

public interface IMongoDbContext
{
    IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName);
    IMongoDatabase Database { get; }
}

