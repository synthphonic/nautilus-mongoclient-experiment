namespace Nautilus.DataProvider.Mongo.Tests.Xunit.Shared
{
    public class BaseTestContext<TFixture> where TFixture : IFixture, new()
    {
        protected BaseTestContext(TFixture fixture)
        {
            Fixture = fixture;
        }

        public TFixture Fixture { get; private set; }
    }
}