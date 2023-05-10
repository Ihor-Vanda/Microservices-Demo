using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Demo.Common.Settings;
using Microsoft.Extensions.Configuration;

namespace Demo.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                if (configuration != null)
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                    if (mongoDbSettings != null && serviceSettings != null)
                    {
                        var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                        return mongoClient.GetDatabase(serviceSettings.ServiceName);
                    }
                }
                throw new MongoConfigurationException("Can`t connect to database");
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
            where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(ServiceProvider =>
            {
                var database = ServiceProvider.GetService<IMongoDatabase>() ?? throw new MongoConfigurationException("Can`t connect to database"); ;
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}