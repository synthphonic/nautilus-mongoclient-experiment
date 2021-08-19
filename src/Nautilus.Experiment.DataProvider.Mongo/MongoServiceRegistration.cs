using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Nautilus.Configuration;

namespace Nautilus.Experiment.DataProvider.Mongo
{
    public static class MongoServiceRegistration
    {
        public static void RegisterMongoDatabases(this IServiceCollection services, AppSettings appSettings, IEnumerable<Type> schemas)
        {
            MongoServiceFactory mongoFactory = new();

            foreach (var providerItem in appSettings.MongoDatabaseProviders)
            {
                ConsoleOutput.Write(typeof(MongoServiceRegistration), ConsoleMessage.Create("Mongo Database Settings..."));
                Console.WriteLine($"KEY: {providerItem.Key}");

                var providerSetting = providerItem.Value;
                Console.WriteLine($"Host: {providerSetting.Host}");
                Console.WriteLine($"Port: {providerSetting.Port}");
                Console.WriteLine($"UserName: {providerSetting.UserName}");
                Console.WriteLine($"Password: {providerSetting.Password}");
                Console.WriteLine($"Database: {providerSetting.Database}");
                Console.WriteLine($"SslProtocol: {providerSetting.SslProtocol}");
                Console.WriteLine($"MongoCredentialMechanism: {providerSetting.MongoCredentialMechanism}");
                Console.WriteLine($"UseTls: {providerSetting.UseTls}");
                Console.WriteLine($"ApplyMongoSecurity:  {providerSetting.UseMongoAuthentication}");

                var mongoClientSettings = ApplyMongoConnectionSettings(providerSetting);
                var mongoService = new MongoService(providerItem.Key, mongoClientSettings, providerSetting.Database);
                mongoService.UseCamelCase();
                mongoService.RegisterSchemas(schemas);

                mongoFactory.Add(providerItem.Key, mongoService);
            }

            services.AddSingleton<IMongoServiceFactory>(mongoFactory);
        }

        public static void RegisterMongoDatabases(this IServiceCollection services, AppSettings appSettings)
        {
            MongoServiceFactory mongoFactory = new();

            foreach (var providerItem in appSettings.MongoDatabaseProviders)
            {
                ConsoleOutput.Write(typeof(MongoServiceRegistration), ConsoleMessage.Create("Mongo Database Settings..."));
                Console.WriteLine($"KEY: {providerItem.Key}");

                var providerSetting = providerItem.Value;
                Console.WriteLine($"Host: {providerSetting.Host}");
                Console.WriteLine($"Port: {providerSetting.Port}");
                Console.WriteLine($"UserName: {providerSetting.UserName}");
                Console.WriteLine($"Password: {providerSetting.Password}");
                Console.WriteLine($"Database: {providerSetting.Database}");
                Console.WriteLine($"SslProtocol: {providerSetting.SslProtocol}");
                Console.WriteLine($"MongoCredentialMechanism: {providerSetting.MongoCredentialMechanism}");
                Console.WriteLine($"UseTls: {providerSetting.UseTls}");
                Console.WriteLine($"ApplyMongoSecurity:  {providerSetting.UseMongoAuthentication}");

                var mongoClientSettings = ApplyMongoConnectionSettings(providerSetting);
                var mongoService = new MongoService(providerItem.Key, mongoClientSettings, providerSetting.Database);
                mongoService.UseCamelCase();

                mongoFactory.Add(providerItem.Key, mongoService);
            }

            services.AddSingleton<IMongoServiceFactory>(mongoFactory);
        }

        private static MongoClientSettings ApplyMongoConnectionSettings(NautilusMongoDatabaseSetting mongoDbSetting)
        {
            var mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(mongoDbSetting.Host, mongoDbSetting.Port),
            };

            if (mongoDbSetting.UseMongoAuthentication)
            {
                mongoClientSettings.UseTls = mongoDbSetting.UseTls;

                if (!mongoDbSetting.SslProtocol.ToLower().Equals("none"))
                {
                    var sslProtocol = (SslProtocols)Enum.Parse(typeof(SslProtocols), mongoDbSetting.SslProtocol, true);
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = sslProtocol
                    };
                }

                var identity = new MongoInternalIdentity(mongoDbSetting.Database, mongoDbSetting.UserName);
                var evidence = new PasswordEvidence(mongoDbSetting.Password);
                mongoClientSettings.Credential = new MongoCredential(mongoDbSetting.MongoCredentialMechanism, identity, evidence);

            }

            return mongoClientSettings;
        }

        //[Obsolete("Use the extension method RegisterMongoDatabases() instead", true)]
        //public static void Register(IServiceCollection services, AppSettings appSettings)
        //{
        //    var callerAndMemberName = CallerMemberHelper.GetClassAndMemberName(typeof(MongoServiceRegistration));

        //    MongoServiceFactory mongoFactory = new();

        //    foreach (var providerItem in appSettings.MongoDatabaseProviders)
        //    {
        //        ConsoleOutput.Write($"{callerAndMemberName}, message: Mongo Database Settings...");
        //        Console.WriteLine($"KEY: {providerItem.Key}");
        //        var providerSetting = providerItem.Value;
        //        Console.WriteLine($"Host: {providerSetting.Host}");
        //        Console.WriteLine($"Port: {providerSetting.Port}");
        //        Console.WriteLine($"UserName: {providerSetting.UserName}");
        //        Console.WriteLine($"Password: {providerSetting.Password}");
        //        Console.WriteLine($"Database: {providerSetting.Database}");
        //        Console.WriteLine($"SslProtocol: {providerSetting.SslProtocol}");
        //        Console.WriteLine($"MongoCredentialMechanism: {providerSetting.MongoCredentialMechanism}");
        //        Console.WriteLine($"UseTls: {providerSetting.UseTls}");
        //        Console.WriteLine($"ApplyMongoSecurity:  {providerSetting.UseMongoAuthentication}");

        //        var mongoClientSettings = ApplyMongoConnectionSettings(providerSetting);
        //        var mongoService = new MongoService(providerItem.Key, mongoClientSettings, providerSetting.Database);
        //        mongoService.UseCamelCase();

        //        mongoFactory.Add(providerItem.Key, mongoService);
        //    }

        //    services.AddSingleton(mongoFactory);
        //}

        //[Obsolete("Use the extension method RegisterMongoDatabases() instead", true)]
        //public static void Register(IServiceCollection services, AppSettings appSettings, IEnumerable<Type> schemas)
        //{
        //    var callerAndMemberName = CallerMemberHelper.GetClassAndMemberName(typeof(MongoServiceRegistration));

        //    MongoServiceFactory mongoFactory = new();

        //    foreach (var providerItem in appSettings.MongoDatabaseProviders)
        //    {
        //        ConsoleOutput.Write($"{callerAndMemberName}, message: Mongo Database Settings...");
        //        Console.WriteLine($"KEY: {providerItem.Key}");
        //        var providerSetting = providerItem.Value;
        //        Console.WriteLine($"Host: {providerSetting.Host}");
        //        Console.WriteLine($"Port: {providerSetting.Port}");
        //        Console.WriteLine($"UserName: {providerSetting.UserName}");
        //        Console.WriteLine($"Password: {providerSetting.Password}");
        //        Console.WriteLine($"Database: {providerSetting.Database}");
        //        Console.WriteLine($"SslProtocol: {providerSetting.SslProtocol}");
        //        Console.WriteLine($"MongoCredentialMechanism: {providerSetting.MongoCredentialMechanism}");
        //        Console.WriteLine($"UseTls: {providerSetting.UseTls}");
        //        Console.WriteLine($"ApplyMongoSecurity:  {providerSetting.UseMongoAuthentication}");

        //        var mongoClientSettings = ApplyMongoConnectionSettings(providerSetting);
        //        var mongoService = new MongoService(providerItem.Key, mongoClientSettings, providerSetting.Database);
        //        mongoService.RegisterSchemas(schemas);
        //        mongoService.UseCamelCase();

        //        mongoFactory.Add(providerItem.Key, mongoService);
        //    }

        //    services.AddSingleton(mongoFactory);
        //}
    }
}