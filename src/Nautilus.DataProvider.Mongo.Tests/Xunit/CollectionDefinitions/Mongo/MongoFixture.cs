using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoClient.Tests.Models;
using MongoClient.Tests.Models.Schema;
using Nautilus.Configuration;
using Nautilus.DataProvider.Mongo.Tests.Xunit.Shared;
using Nautilus.Experiment.DataProvider.Mongo;
using Nautilus.Experiment.DataProvider.Mongo.Exceptions;
using Nautilus.Extensions;

namespace Nautilus.DataProvider.Mongo.Tests.Xunit.CollectionDefinitions.Mongo
{
    public class MongoFixture : IDisposable, IFixture
    {
        private bool disposedValue;

        public MongoFixture()
        {
            var host = CreateHostBuilder().Build();
            Services = host.Services;
            //host.Run(); // don't call this because we are not running in an aspnet core host domain

            var mongoServiceFactory = Services.GetService<IMongoServiceFactory>();
            var mongoService = mongoServiceFactory.GetService(TestConstants.MongoDBKey);
            var schemas = new List<Type>
            {
                typeof(PersonSchema),
                typeof(UserSchema),
                typeof(CategorySchema),
                typeof(CategoryDetailSchema),
                typeof(RawPayloadSchema),
                typeof(NoAttributeModelSchema)
            };
            mongoService.RegisterSchemas(schemas);
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                  .ConfigureAppConfiguration((webHostBuilderContext, configBuilder) =>
                  {
                      //
                      // no need to use system environment. just read the settings from appsettings.json
                      //
                      //configBuilder.ShowConsoleOutput(false);

                      //configBuilder.UseDefaultEnvironment(baseAppSettingPath);
                      configBuilder.UseSystemEnvironments(new SystemEnvironmentOptions
                      {
                          BaseAppSetting = Path.Combine(AppContext.BaseDirectory, "_config"),
                          UseDefault = true
                      });
                  })
                  .ConfigureLogging((loggingBuilder) =>
                  {
                      //loggingBuilder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.None);
                  })
                  .ConfigureServices((webHostBuilderContext, services) =>
                  {
                      var appSettings = webHostBuilderContext.Configuration.GetAppSetting<AppSettings>();
                      services.AddSingleton(appSettings);
                      services.RegisterMongoDatabases(appSettings);
                  });
        }

        public IServiceProvider Services { get; private set; }

        internal Category CreateObject(string categoryName, string userId)
        {
            var category = new Category
            {
                CategoryName = categoryName,
                UserId = userId
            };

            return category;
        }

        #region IDisposable pattern
        ~MongoFixture()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //
                    // Dispose managed state (managed objects)
                    //
                    var mongoFactory = Services.GetService<IMongoServiceFactory>();
                    var mongoService = mongoFactory.GetService(TestConstants.MongoDBKey);

                    try
                    {
                        mongoService.DropDatabase();
                    }
                    catch (NautilusMongoDbException nautilusMongoEx)
                    {
                        ConsoleOutput.WriteWarning(nautilusMongoEx.Message);
                    }
                    catch (Exception ex)
                    {
                        ConsoleOutput.WriteWarning(ex.Message);
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        #endregion
    }
}