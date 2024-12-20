using System.Linq.Expressions;

namespace MongoRepoNet;

public interface IMongoRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retorna todos os registros
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="page">page</param>
    /// <param name="pageSize">page size</param>
    /// <returns>
    ///  Retorna uma lista paginada de documento paginado
    /// </returns>
    Task<PagedResult<TEntity>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retorna dado pelo id
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="field">Propriedade da busca</param>
    /// <param name="id">Id da propriedade de busca</param>
    /// <returns>
    ///  Retorna um documento de acordo com o id
    /// </returns>
    Task<TEntity> GetByIdAsync(string field, Guid id);

    /// <summary>
    /// Retorna dado pelo nome
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="field">Propriedade da busca</param>
    /// <param name="nome">Valor da busca</param>
    /// <returns>
    ///  Retorna uma lista paginada de documento de acordo com o nome
    /// </returns>
    Task<PagedResult<TEntity>> GetByNameAsync(string field, string nome);

    /// <summary>
    /// Insere um documento na base
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task InsertAsync(TEntity obj);

    /// <summary>
    /// Insere multiplos documentos na base
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task InsertManyAsync(List<TEntity> obj);

    /// <summary>
    /// Insere um documento informando em qual colecao sera inserida
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="collectionName">Nome da colecao</param>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task InsertAsync(TEntity obj, string collectionName);

    /// <summary>
    ///  Atualiza multiplos campos de um documento.
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="field">Propriedade do documento</param>
    /// <param name="collectionName">Nome da colecao</param>
    /// <returns>
    ///  Nao possui retorno 
    /// </returns>
    Task UpdateAsync(string field, TEntity obj, string collectionName);

    /// <summary>
    /// Deleta um documento pelo id e  informando em qual colecao sera deletado
    /// </summary>
    /// <param name="field">Propriedade do documento</param>
    /// <param name="id">Id da propriedade de busca</param>
    /// <param name="collectionName">Nome da colecao</param>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task DeleteAsync(string field, Guid id, string collectionName);

    /// <summary>
    ///  Atualiza documento
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="id">Id da propriedade de busca</param>
    /// <param name="obj">Documento</param>
    /// <returns>
    /// Nao possui retorno
    /// </returns>
    Task UpdateAsync(string id, TEntity obj);

    /// <summary>
    ///  Atualiza um campo
    /// </summary>
    /// <typeparam name="TField">O tipo da propriedade.</typeparam>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="whereCondition">Expressao da condicao</param>
    /// <param name="field">Propriedade para atualizar</param>
    /// <param name="value">Novo valor para atualizar</param>
    /// <returns>
    /// Nao possui retorno
    /// </returns>
    Task UpdateAsync<TField>(
    Expression<Func<TEntity, bool>> whereCondition,
    Expression<Func<TEntity, TField>> field,
    TField value);

    /// <summary>
    /// Deleta um documento pelo id de uma propriedade
    /// </summary>
    /// <param name="field">Propriedade para atualizar</param>
    /// <param name="id">Id da propriedade </param>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task DeleteAsync(string field, Guid id);

    /// <summary>
    /// Deleta um documento pelo nome ou texto que seja igual de uma propriedade
    /// </summary>
    /// <param name="field">Propriedade para atualizar</param>
    /// <param name="nome">Valor da propriedade </param>
    /// <returns>
    ///  Nao possui retorno
    /// </returns>
    Task DeleteByNameAsync(string field, string nome);

    /// <summary>
    /// Retorna documentos com base no filtro informado
    /// </summary>
    /// <typeparam name="TEntity">O tipo do documento.</typeparam>
    /// <param name="filter">Expressao do filtro</param>
    /// <returns>
    /// Retorna uma lista de documento com base no filtro informado
    /// </returns>
    Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> filter);
}
