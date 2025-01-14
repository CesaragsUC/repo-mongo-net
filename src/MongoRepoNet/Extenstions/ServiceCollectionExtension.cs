using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoRepoNet.Abstractions;
using MongoRepoNet.Context;
using MongoRepoNet.Repository;
using MongoRepoNet.Settings;

namespace MongoRepoNet.Extenstions;

public static class ServiceCollectionExtension
{
    public static void AddMongoRepoNet(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettingsOptions>(options =>
        {
            configuration.GetSection(MongoDbSettingsOptions.Section).Bind(options);
        });

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IMongoDbContext, MongoDbContext>();
    }

}
