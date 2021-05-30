namespace MongoClient.Tests.ParallelEngine
{
    internal class ParallelExecutionEnginePlan
    {
        public static ParallelExecutionInfoContext GenerateParallelExecutionPlan(int threadCount, int recordCount)
        {
            var actualThreadCountToSpawn = threadCount;
            var totalRecordperThread = recordCount / threadCount;
            var remainders = recordCount % threadCount;

            if (remainders > 0)
                actualThreadCountToSpawn++;

            var context = new ParallelExecutionInfoContext(totalRecordperThread, remainders, actualThreadCountToSpawn);

            return context;
        }
    }
}