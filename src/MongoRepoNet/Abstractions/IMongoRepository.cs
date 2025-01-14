using System.Linq.Expressions;
using MongoRepoNet.Utils;

namespace MongoRepoNet.Abstractions;

public interface IMongoRepository<TDocument> where TDocument : class
{


    /// <summary>
    /// Provides an IQueryable interface for querying the documents in the repository.
    /// </summary>
    /// <returns>
    /// Returns an IQueryable of TDocument, allowing for LINQ queries to be performed on the documents.
    /// </returns>
    IQueryable<TDocument> AsQueryable();

    /// <summary>
    /// Retrieves all records.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>
    /// Returns a paginated list of documents.
    /// </returns>
    Task<PagedResult<TDocument>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a document by its ID.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="field">The field to search by.</param>
    /// <param name="id">The ID of the document.</param>
    /// <returns>
    /// Returns a document that matches the given ID.
    /// </returns>
    Task<TDocument> FindByIdAsync(string field, Guid id);

    /// <summary>
    /// Retrieves a document by its property ID.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="field">The field to search by.</param>
    /// <param name="id">The  property ID of the document.</param>
    /// <returns>
    /// Returns a document that matches the given  property ID.
    /// </returns>
    Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves documents by name.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="field">The field to search by.</param>
    /// <param name="name">The value to search for.</param>
    /// <returns>
    /// Returns a paginated list of documents that match the given name.
    /// </returns>
    Task<PagedResult<TDocument>> GetByNameAsync(string field, string name);

    /// <summary>
    /// Inserts a document into the database.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <returns>
    /// No return value.
    /// </returns>
    Task InsertAsync(TDocument obj);

    /// <summary>
    /// Inserts multiple documents into the database.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <returns>
    /// No return value.
    /// </returns>
    Task InsertManyAsync(List<TDocument> obj);

    /// <summary>
    /// Inserts a document into a specified collection.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task InsertAsync(TDocument obj, string collectionName);

    /// <summary>
    /// Updates multiple fields of a document.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="field">The field of the document.</param>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task UpdateAsync(string field, TDocument obj, string collectionName);

    /// <summary>
    /// Deletes a document by its ID from a specified collection.
    /// </summary>
    /// <param name="field">The field of the document.</param>
    /// <param name="id">The ID of the document.</param>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task DeleteAsync(string field, Guid id, string collectionName);

    /// <summary>
    /// Updates a document.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="id">The ID of the document.</param>
    /// <param name="obj">The document to update.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task UpdateAsync(string id, TDocument obj);

    /// <summary>
    /// Updates a specific field of a document.
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="whereCondition">The condition expression.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The new value for the field.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task UpdateAsync<TField>(
    Expression<Func<TDocument, bool>> whereCondition,
    Expression<Func<TDocument, TField>> field,
    TField value);

    /// <summary>
    /// Replaces the entire document or inserts a new one if it does not exist.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="id">The ID of the document.</param>
    /// <param name="obj">The document to upsert.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task UpsertAsync(string id, TDocument obj);

    /// <summary>
    /// Deletes a document by its ID.
    /// </summary>
    /// <param name="field">The field to update.</param>
    /// <param name="id">The ID of the document.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task DeleteAsync(string field, Guid id);

    /// <summary>
    /// Deletes a document by a name or text that matches a field.
    /// </summary>
    /// <param name="field">The field to update.</param>
    /// <param name="name">The value of the field.</param>
    /// <returns>
    /// No return value.
    /// </returns>
    Task DeleteByNameAsync(string field, string name);

    /// <summary>
    /// Retrieves documents based on the provided filter.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <param name="filter">The filter expression.</param>
    /// <returns>
    /// Returns a list of documents that match the provided filter.
    /// </returns>
    Task<IEnumerable<TDocument>> FilterAsync(Expression<Func<TDocument, bool>> filter);
}
