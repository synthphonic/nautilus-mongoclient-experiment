namespace MongoClient.Tests.Helpers
{
    internal record ParallelThreadInfoContext(int RecordsPerThread, int RemainingRecords, int ActualThreadCountToSpawn)
    {
    };
}