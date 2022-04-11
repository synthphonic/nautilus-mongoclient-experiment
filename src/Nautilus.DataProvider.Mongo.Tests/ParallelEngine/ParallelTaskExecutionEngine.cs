namespace MongoClient.Tests.ParallelEngine;

internal class ParallelTaskExecutionEngine<TModel> where TModel : class, new()
{
    private IList<Task> _tasks;

    public ParallelTaskExecutionEngine()
    {
        _tasks = new List<Task>();
    }

    public void Add(Task task)
    {
        _tasks.Add(task);
        ConsoleOutput.Write(GetType(), message: $"Task added");
    }

    public void Execute()
    {
        var sw = ProcessStopwatch.Start();

        Task.WaitAll(_tasks.ToArray());

        sw.Stop();

        Elapsed = sw.Elapsed;

        ConsoleOutput.Write(GetType(), message: $"All tasks completed...");
    }

    public object Elapsed { get; private set; }

    internal static IEnumerable<WriteModel<TModel>> ToWriteModelList(IEnumerable<TModel> models)
    {
        var rawPayloadModels = new List<WriteModel<TModel>>();
        foreach (var model in models)
        {
            var inserModel = new InsertOneModel<TModel>(model);
            rawPayloadModels.Add(inserModel);
        }

        return rawPayloadModels.AsEnumerable();
    }
}
