using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepoNet.Abstractions;
using MongoRepoNet.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoRepoNet.Repository;

public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
{
    private readonly IMongoCollection<TDocument> _mongoCollection;
    private readonly IMongoDbContext _dbContext;

    public MongoRepository(IMongoDbContext dbContext)
    {
        _mongoCollection = dbContext.GetCollection<TDocument>(typeof(TDocument).Name);
        _dbContext = dbContext;
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
        return _mongoCollection.AsQueryable();
    }


    public virtual async Task<PagedResult<TDocument>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var builder = Builders<TDocument>.Filter;
        var filterDefinition = builder.Empty;

        var results = await _mongoCollection
            .Find(filterDefinition)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<TDocument>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = page,
            PageSize = pageSize

        };
    }

    public virtual async Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq("_id", objectId);
        return await (await _mongoCollection.FindAsync(filter, null, cancellationToken)).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TDocument> FindByIdAsync(string field, Guid id)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<PagedResult<TDocument>> GetByNameAsync(string field, string name)
    {
        // Use Regex to find names that start with the given string (case-insensitive)
        var filter = Builders<TDocument>.Filter.Regex(field, new BsonRegularExpression(name, "i"));

        var result = await _mongoCollection.Find(filter).ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filter);

        return new PagedResult<TDocument>
        {
            Items = result,
            TotalCount = (int)totalItems,
            Page = 1,
            PageSize = 10

        };
    }

    public virtual async Task<IEnumerable<TDocument>> FilterAsync(Expression<Func<TDocument, bool>> filter)
    {

        return await _mongoCollection.Find(filter).ToListAsync();
    }

    public virtual async Task InsertAsync(TDocument obj)
    {
        await _mongoCollection.InsertOneAsync(obj);
    }

    public virtual async Task InsertAsync(TDocument obj, string collectionName)
    {
        var collection = _dbContext.GetCollection<TDocument>(collectionName);
        await collection.InsertOneAsync(obj);
    }

    public virtual async Task InsertManyAsync(List<TDocument> obj)
    {
        await _mongoCollection.InsertManyAsync(obj);
    }

    public virtual async Task UpsertAsync(string id, TDocument obj)
    {
        var filter = Builders<TDocument>.Filter.Eq(id, GetEntityId(id, obj));

        await _mongoCollection.ReplaceOneAsync(filter, obj, options: new ReplaceOptions { IsUpsert = true });
    }

    public virtual async Task DeleteAsync(string field, Guid id, string collectionName)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        var collection = _dbContext.GetCollection<TDocument>(collectionName);
        await collection.DeleteOneAsync(filter);
    }

    public virtual async Task DeleteAsync(string field, Guid id)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        await _mongoCollection.DeleteOneAsync(filter);
    }

    public virtual async Task DeleteByNameAsync(string field, string name)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, name);
        await _mongoCollection.DeleteOneAsync(filter);
    }

    public virtual async Task UpdateAsync(string id, TDocument obj)
    {
        var filter = Builders<TDocument>.Filter.Eq(id, GetEntityId(id, obj));

        // Define the updates needed for each <field,value>.
        var update = GetObjectProperties(obj);

        await _mongoCollection.UpdateOneAsync(filter, update);
    }
    public virtual async Task UpdateAsync(string field, TDocument obj, string collectionName)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, GetEntityId(field, obj));

        var update = GetObjectProperties(obj);

        var collection = _dbContext.GetCollection<TDocument>(collectionName);
        await collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateAsync<TField>(Expression<Func<TDocument, bool>> whereCondition, Expression<Func<TDocument, TField>> field, TField value)
    {
        var update = Builders<TDocument>.Update.Set(field, value);
        await _mongoCollection.UpdateOneAsync(whereCondition, update);
    }


    private object GetEntityId(string field, TDocument entity)
    {
        var propertyInfo = entity.GetType().GetProperty(field);
        var value = propertyInfo?.GetValue(entity);

        // Check if the value is a Guid or a string
        if (value is Guid guidValue)
        {
            return guidValue;
        }

        if (value is string stringValue && Guid.TryParse(stringValue, out var parsedGuid))
        {
            return parsedGuid;
        }

        // If the value is neither Guid nor string, return as is (avoid conversion error)
        return value!;
    }

    private object GetPropertyValue(TDocument entity, string propertyName)
    {
        var propertyInfo = entity.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(entity);
    }

    private UpdateDefinition<T> GetObjectProperties<T>(T obj)
    {
        // Initialize the updateDefinition
        var updateDefinition = Builders<T>.Update.Combine();

        // Get the type of the object
        Type tipo = obj.GetType();

        // Loop through all the properties of the object
        foreach (PropertyInfo propriedade in tipo.GetProperties())
        {
            // Get the property name
            string nomePropriedade = propriedade.Name;

            // Get the property value
            object valorPropriedade = propriedade.GetValue(obj);

            // Add the value to the updateDefinition (If the value is not null)
            if (valorPropriedade != null)
            {
                updateDefinition = updateDefinition.Set(nomePropriedade, valorPropriedade);
            }
        }

        return updateDefinition;
    }

}

