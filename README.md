# 📦 RepoMongoNet a MongoDb Repository for .NET

Um repositório genérico com implementações completas para MongoDb usando .NET

✨ Descrição

Este pacote oferece uma implementação completa de um repositório genérico para aplicações .NET com MongoDb, facilitando a criação, leitura, atualização e remoção (CRUD) de entidades no banco de dados.

Com ele, você pode simplificar o acesso a dados usando boas práticas, abstraindo a camada de repositório e deixando sua aplicação mais limpa e desacoplada.

🚀 Instalação
Você pode instalar o pacote através do NuGet Package Manager ou da CLI:

Usando o NuGet Package Manager:
<pre> Install-Package RepoMongoNet </pre>

🛠️ Configuração

Crie uma classe MongoDbSettings.cs:

```csharp
public class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}

```
appsetings.json:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://userdb:password@localhost:27017/?authMechanism=SCRAM-SHA-256"
    "DatabaseName": "MyStore"
  }
}
```
Crie uma  ServiceCollectionExtensions.cs:

```csharp

    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        return services;
    }
```

No seu Program.cs:

```csharp

using Microsoft.EntityFrameworkCore;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Registrando o repositório
builder.Services.MongoDbService(builder.Configuration)
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();

var app = builder.Build();

```

🎯 Uso

Criando uma Entidade

Defina uma entidade no seu projeto:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

```
Usando o Repositório

Exemplo de uso do repositório genérico no Controller:

```csharp
public class ProductsController : ControllerBase
{
    private readonly IMongoRepository<Products> _repository;

    public ProductsController(IMongoRepository<Product> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _repository.InsertAsync(product);
        return Ok("Produto criado com sucesso!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }
}

```


⚙️ Funcionalidades

CRUD Completo:

* InsertAsync(TEntity obj) - Adiciona uma nova entidade.
* GetById(string field, Guid id) - Retorna uma entidade pelo ID.
* GetAllAsync(int page = 1, int pageSize = 10, string sort = "asc") - Retorna todas as entidades om paginação.
* UpdateAsync(string field, TEntity obj, string collectionName) - Atualiza uma entidade existente.
* Delete(string field, Guid id, string collectionName) - Remove uma entidade pelo ID.
* Performance:

Uso eficiente de conexões com o banco de dados PostgreSQL.
Genérico:

Pode ser usado com qualquer classe de entidade que tenha um identificador.

🧩 Requisitos

* .NET 6 ou superior
* MongoDB.Driver 2.29.0+

🗂️ Estrutura do Pacote

Interfaces:

``` IMongoRepository<T>: Interface do repositório genérico. ```
  
Implementações:

``` MongoRepository<T>: Implementação concreta.```

🤝 Contribuição
Contribuições são bem-vindas!

* Faça um fork do repositório.
* Crie uma branch para sua feature (git checkout -b feature/NovaFeature).
* Commit suas mudanças (git commit -m "Adicionei uma nova feature X").
* Faça um push para a branch (git push origin feature/NovaFeature).
* Abra um Pull Request.

⭐ Dê uma estrela!

Se você achou este pacote útil, não se esqueça de dar uma ⭐ no GitHub!
