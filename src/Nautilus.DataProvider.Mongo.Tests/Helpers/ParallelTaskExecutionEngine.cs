//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Nautilus;
//using Nautilus.Diagnostics.Utilities;

//namespace MongoClient.Tests.Helpers
//{
//    internal class ParallelTaskExecutionEngine
//    {
//        private IList<Task> _tasks;

//        public ParallelTaskExecutionEngine()
//        {
//            _tasks = new List<Task>();
//        }

//        public void Add(Task task)
//        {
//            _tasks.Add(task);
//            ConsoleOutput.Write(GetType(), message: $"Task added");
//        }

//        public void Execute()
//        {
//            var sw = ProcessStopwatch.Start();

//            Task.WaitAll(_tasks.ToArray());
//            ConsoleOutput.Write(GetType(), message: $"Waiting for all task to complete...");

//            sw.Stop();

//            Elapsed = sw.Elapsed;
//        }

//        public object Elapsed { get; private set; }

//        public static ParallelThreadInfoContext GenerateParallelExecutionPlan(int threadCount, int recordCount)
//        {
//            var actualThreadCountToSpawn = threadCount;
//            var totalRecordperThread = recordCount / threadCount;
//            var remainders = recordCount % threadCount;

//            if (remainders > 0)
//                actualThreadCountToSpawn++;

//            var context = new ParallelThreadInfoContext(totalRecordperThread, remainders, actualThreadCountToSpawn);

//            return context;
//        }
//    }
//}