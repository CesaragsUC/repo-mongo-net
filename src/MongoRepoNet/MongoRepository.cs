using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoRepoNet;

public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
{
    private readonly IMongoCollection<TEntity> _mongoCollection;
    private readonly IMongoDbContext _dbContext;

    public MongoRepository(IMongoDbContext dbContext)
    {
        _mongoCollection = dbContext.GetCollection<TEntity>(typeof(TEntity).Name);
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TEntity>> GetAll(int page = 1, int pageSize = 10, string sort = "asc")
    {
        var builder = Builders<TEntity>.Filter;
        var filterDefinition = builder.Empty;

        var results = await _mongoCollection
            .Find(filterDefinition)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<TEntity>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = page,
            PageSize = pageSize

        };
    }

    public async Task<TEntity> GetById(string field, Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq(field, id);
        return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<PagedResult<TEntity>> GetByName(string field, string nome)
    {
        // Usar Regex para encontrar nomes que começam com a string fornecida (case-insensitive)
        var filter = Builders<TEntity>.Filter.Regex(field, new BsonRegularExpression(nome, "i"));

        var result = await _mongoCollection.Find(filter).ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filter);

        return new PagedResult<TEntity>
        {
            Items = result,
            TotalCount = (int)totalItems,
            Page = 1,
            PageSize = 10

        };
    }

    public async Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> filter)
    {

        return await _mongoCollection.Find(filter).ToListAsync();
    }

    public async Task InsertAsync(TEntity obj)
    {
        await _mongoCollection.InsertOneAsync(obj);
    }

    public async Task InsertMany(List<TEntity> obj)
    {
        await _mongoCollection.InsertManyAsync(obj);
    }


    public async Task InsertAsync(TEntity obj, string collectionName)
    {
        var collection = _dbContext.GetCollection<TEntity>(collectionName);
        await collection.InsertOneAsync(obj);
    }

    public async Task UpdateAsync(string field, TEntity obj, string collectionName)
    {
        var filter = Builders<TEntity>.Filter.Eq(field, GetEntityId(field, obj));

        var update = GetObjectProperties(obj);

        var collection = _dbContext.GetCollection<TEntity>(collectionName);
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task Delete(string field, Guid id, string collectionName)
    {
        var filter = Builders<TEntity>.Filter.Eq(field, id);
        var collection = _dbContext.GetCollection<TEntity>(collectionName);
        await collection.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(string id, TEntity obj)
    {
        var filter = Builders<TEntity>.Filter.Eq(id, GetEntityId(id, obj));

        // Define as atualizações necessárias para cada <field,value>.
        var update = GetObjectProperties(obj);

        await _mongoCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateAsync<TField>(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, TField>> field, TField value)
    {
        var update = Builders<TEntity>.Update.Set(field, value);
        await _mongoCollection.UpdateOneAsync(whereCondition, update);
    }


    public async Task Delete(string field, Guid id)
    {
        var filter = Builders<TEntity>.Filter.Eq(field, id);
        await _mongoCollection.DeleteOneAsync(filter);
    }

    public async Task DeleteByName(string field, string nome)
    {
        var filter = Builders<TEntity>.Filter.Eq(field, nome);
        await _mongoCollection.DeleteOneAsync(filter);
    }


    private object GetEntityId(string field, TEntity entity)
    {
        var propertyInfo = entity.GetType().GetProperty(field);
        var value = propertyInfo?.GetValue(entity);

        // Verificar se o valor é um Guid ou uma string
        if (value is Guid guidValue)
        {
            return guidValue;
        }

        if (value is string stringValue && Guid.TryParse(stringValue, out var parsedGuid))
        {
            return parsedGuid;
        }

        // Se o valor não for Guid nem string, retornar como está (evitar erro de conversão)
        return value;
    }

    private object GetPropertyValue(TEntity entity, string propertyName)
    {
        // Obtenha o valor de uma propriedade da entidade
        var propertyInfo = entity.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(entity);
    }

    private UpdateDefinition<T> GetObjectProperties<T>(T obj)
    {
        // Inicializa o updateDefinition
        var updateDefinition = Builders<T>.Update.Combine();

        // Obtém o tipo do objeto
        Type tipo = obj.GetType();

        // Percorre todas as propriedades do objeto
        foreach (PropertyInfo propriedade in tipo.GetProperties())
        {
            // Pega o nome da propriedade
            string nomePropriedade = propriedade.Name;

            // Pega o valor da propriedade
            object valorPropriedade = propriedade.GetValue(obj);

            // Adiciona o valor ao updateDefinition (Se o valor não for nulo)
            if (valorPropriedade != null)
            {
                updateDefinition = updateDefinition.Set(nomePropriedade, valorPropriedade);
            }
        }

        return updateDefinition;
    }
}

