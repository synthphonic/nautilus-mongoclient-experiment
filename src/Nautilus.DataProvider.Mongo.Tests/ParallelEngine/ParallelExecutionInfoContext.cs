namespace MongoClient.Tests.ParallelEngine;

internal record ParallelExecutionInfoContext(int RecordsPerThread, int RemainingRecords, int ActualThreadCountToSpawn)
{
};
