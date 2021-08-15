using Xunit;

namespace Nautilus.DataProvider.Mongo.Tests.Xunit.CollectionDefinitions.Mongo
{
    [CollectionDefinition(CollectionDefinitionKeys.MongoDb)]
    public class MongoCollectionDefinition : ICollectionFixture<MongoFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.            
    }
}