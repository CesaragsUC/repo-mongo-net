using MongoDB.Driver;

namespace MongoRepoNet;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase Database { get; }
}

