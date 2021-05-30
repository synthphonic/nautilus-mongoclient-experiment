using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Driver;
using Nautilus;
using Nautilus.Diagnostics.Utilities;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.ParallelEngine
{
    internal class ParallelThreadExecutionEngine<TModel> where TModel : class, new()
    {
        //private IList<IList<TModel>> _groups;
        private IList<Thread> _workerThreads;
        private IList<IList<WriteModel<TModel>>> _groupWriteModels;
        private MongoBaseSchema<TModel> _schema;

        internal ParallelThreadExecutionEngine()
        {
            _workerThreads = new List<Thread>();
            _groupWriteModels = new List<IList<WriteModel<TModel>>>();
        }

        public void Execute()
        {
            var sw = ProcessStopwatch.Start();

            var threads = new Thread[_groupWriteModels.Count()];
            for (var i = 0; i < _groupWriteModels.Count(); i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart(ExecuteInternal));
                threads[i].Priority = ThreadPriority.Normal;

                threads[i].Start(_groupWriteModels[i]);
            }

            for (var i = 0; i < _groupWriteModels.Count(); i++)
            {
                threads[i].Join();
            }

            sw.Stop();
            Elapsed = sw.Elapsed;

            ConsoleOutput.Write(GetType(), message: $"All threads completed...");
        }

        internal void Add(IEnumerable<TModel> models)
        {
            var writeModel = new List<WriteModel<TModel>>();
            foreach (var item in models)
            {
                writeModel.Add(new InsertOneModel<TModel>(item));
            }

            _groupWriteModels.Add(writeModel);
        }

        internal void SetSchema(MongoBaseSchema<TModel> schema)
        {
            _schema = schema;
        }

        private void ExecuteInternal(object objectState)
        {
            var a = objectState as IEnumerable<WriteModel<TModel>>;
            _schema.BulkWrite(a);
        }

        public object Elapsed { get; private set; }
    }
}