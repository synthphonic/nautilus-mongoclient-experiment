/*
 * References:
 *  Search collection given specific criteria or filter
 *      - https://stackoverflow.com/questions/9314886/getting-an-item-count-with-mongodb-c-sharp-driver-query-builder
 *  
 *  Bulk CRUD operations (BulkWriteAsync and etc...)
 *      - https://dev.to/mpetrinidev/a-guide-to-bulk-write-operations-in-mongodb-with-c-51fk
 *      
 */

namespace MongoClient.Tests;

using NUnit.Framework;

[Ignore("dont run")]
public class BulkCRUDTest : BaseTest
{
    [OneTimeSetUp]
    public async Task Setup()
    {
        DatabaseName = "bulk-test-db";

        await SetupMongoDb();
        await TearDown();
    }

    [OneTimeTearDown]
    public async Task TearDownOneTime()
    {
        await TearDown();
    }

    [Test]
    [TestCase(50000)]
    [TestCase(100000)]
    [TestCase(200000)]
    [TestCase(300000)]
    [TestCase(400000)]
    [TestCase(500000)]
    public async Task BulkWriteAsync_Write_IsOrdered(int insertCount)
    {
        Console.WriteLine($"\nBulkWriteAsync (is ordered): TestCase = {insertCount} records");

        //
        // Arrange
        var persons = new List<WriteModel<Person>>();
        for (var i = 1; i <= insertCount; i++)
        {
            var p = new Person
            {
                Active = true,
                FirstName = $"First Name {i}",
                LastName = $"Last Name {i}",
                Age = GetRandomAge(),
            };

            var inserModel = new InsertOneModel<Person>(p);
            persons.Add(inserModel);
        }

        //
        // Act
        var sw = ProcessStopwatch.Start();

        var schema = MongoService.GetSchema<Person>();
        await schema.BulkWriteAsync(persons);

        sw.Stop();
        Console.WriteLine($"Total Records inserted {insertCount} : [{sw.Elapsed}] secs");

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<Person>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(insertCount, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<Person>();
    }

    [Test]
    [TestCase(25000)]
    [TestCase(50000)]
    [TestCase(70000)]
    [TestCase(100000)]
    [TestCase(200000)]
    [TestCase(300000)]
    [TestCase(400000)]
    [TestCase(500000)]
    public async Task BulkWriteAsync_Write_RawData_SingleThread_IsOrdered(int insertCount)
    {
        Console.WriteLine($"\nBulkWriteAsync_Write_RawData_SingleThread_IsOrdered (is ordered): TestCase = {insertCount} records");

        //
        // Arrange
        var persons = new List<WriteModel<RawPayloadModel>>();
        for (var i = 1; i <= insertCount; i++)
        {
            var model = await ReadJsonData<RawPayloadModel>("payload1.json");
            var inserModel = new InsertOneModel<RawPayloadModel>(model);
            persons.Add(inserModel);
        }

        //
        // Act
        var sw = ProcessStopwatch.Start();

        var schema = MongoService.GetSchema<RawPayloadModel>();
        await schema.BulkWriteAsync(persons);

        sw.Stop();
        Console.WriteLine($"Total Records inserted {insertCount} : [{sw.Elapsed}] secs");

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<RawPayloadModel>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(insertCount, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<RawPayloadModel>();
    }

    [Test]
    [TestCase(1239, 2)]
    [TestCase(10000, 2)]
    [TestCase(50000, 2)]
    [TestCase(100000, 2)]
    [Category("Multi Threadings")]
    public async Task BulkWriteAsync_Write_RawData_Multi_Threads_IsOrdered(int recordsToProcess, int spawnThreadCount)
    {
        Console.WriteLine($"\nBulkWriteAsync_Write_RawData_Multi_Threads_IsOrdered (is ordered): TestCase = {recordsToProcess} records");
        Console.WriteLine("==== INPUT PARAMETERS ====");
        Console.WriteLine($"Expected Thread Count To Spawn {spawnThreadCount}");
        Console.WriteLine($"Records To Process {recordsToProcess}");
        Console.WriteLine();

        #region Arrange
        var engine = new ParallelThreadExecutionEngine<RawPayloadModel>();
        var parallelExecutionPlan = ParallelExecutionEnginePlan.GenerateParallelExecutionPlan(spawnThreadCount, recordsToProcess);
        Console.WriteLine($"Total records per loop is {parallelExecutionPlan.RecordsPerThread}");
        Console.WriteLine($"Total records remaining {parallelExecutionPlan.RemainingRecords}");
        Console.WriteLine($"Total expected threads to spawn {spawnThreadCount}");
        Console.WriteLine($"Total actual threads to spawn {parallelExecutionPlan.ActualThreadCountToSpawn}");

        for (var threadCount = 0; threadCount < parallelExecutionPlan.ActualThreadCountToSpawn; threadCount++)
        {
            IList<RawPayloadModel> models;
            if (parallelExecutionPlan.ActualThreadCountToSpawn == threadCount + 1 && parallelExecutionPlan.RemainingRecords > 0)
            {
                models = new List<RawPayloadModel>();

                // processing records for the last thread
                for (var i = 0; i < parallelExecutionPlan.RemainingRecords; i++)
                {
                    var model = await ReadJsonData<RawPayloadModel>("payload1.json");
                    models.Add(model);
                }

                engine.Add(models);

                continue;
            }

            models = new List<RawPayloadModel>();
            for (var i = 0; i < parallelExecutionPlan.RecordsPerThread; i++)
            {
                var model = await ReadJsonData<RawPayloadModel>("payload1.json");
                models.Add(model);
            }

            engine.Add(models);
        }
        #endregion

        #region Act
        var schema = MongoService.GetSchema<RawPayloadModel>();
        engine.SetSchema(schema);
        engine.Execute();
        Console.WriteLine($"Total Records inserted {recordsToProcess} : [{engine.Elapsed}] secs");
        #endregion

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<RawPayloadModel>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(recordsToProcess, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<RawPayloadModel>();
        await Task.Delay(3000); // release a bit of pressure to the code
    }

    [Test]
    [TestCase(1239, 2)]
    [TestCase(10000, 2)]
    [TestCase(50000, 2)]
    [TestCase(100000, 2)]
    [Category("Multi Tasks")]
    public async Task BulkWriteAsync_Write_RawData_Multi_Tasks_IsOrdered(int recordsToProcess, int spawnThreadCount)
    {
        Console.WriteLine($"\nBulkWriteAsync_Write_RawData_Multi_Tasks_IsOrdered (is ordered): TestCase = {recordsToProcess} records");
        Console.WriteLine("==== INPUT PARAMETERS ====");
        Console.WriteLine($"Expected Thread Count To Spawn {spawnThreadCount}");
        Console.WriteLine($"Records To Process {recordsToProcess}");
        Console.WriteLine();

        #region Arrange
        var engine = new ParallelTaskExecutionEngine<RawPayloadModel>();
        var parallelExecutionPlan = ParallelExecutionEnginePlan.GenerateParallelExecutionPlan(spawnThreadCount, recordsToProcess);
        Console.WriteLine($"Total records per loop is {parallelExecutionPlan.RecordsPerThread}");
        Console.WriteLine($"Total records remaining {parallelExecutionPlan.RemainingRecords}");
        Console.WriteLine($"Total expected threads to spawn {spawnThreadCount}");
        Console.WriteLine($"Total actual threads to spawn {parallelExecutionPlan.ActualThreadCountToSpawn}");
        var schema = MongoService.GetSchema<RawPayloadModel>();
        IEnumerable<WriteModel<RawPayloadModel>> writeModels = null;

        for (var threadCount = 0; threadCount < parallelExecutionPlan.ActualThreadCountToSpawn; threadCount++)
        {
            IList<RawPayloadModel> models;
            if (parallelExecutionPlan.ActualThreadCountToSpawn == threadCount + 1 && parallelExecutionPlan.RemainingRecords > 0)
            {
                models = new List<RawPayloadModel>();

                // processing records for the last thread
                for (var i = 0; i < parallelExecutionPlan.RemainingRecords; i++)
                {
                    var model = await ReadJsonData<RawPayloadModel>("payload1.json");
                    models.Add(model);
                }

                writeModels = ParallelTaskExecutionEngine<RawPayloadModel>.ToWriteModelList(models);
                engine.Add(schema.BulkWriteAsync(writeModels));

                continue;
            }

            models = new List<RawPayloadModel>();
            for (var i = 0; i < parallelExecutionPlan.RecordsPerThread; i++)
            {
                var model = await ReadJsonData<RawPayloadModel>("payload1.json");
                models.Add(model);
            }

            writeModels = ParallelTaskExecutionEngine<RawPayloadModel>.ToWriteModelList(models);
            engine.Add(schema.BulkWriteAsync(writeModels));
        }
        #endregion

        #region Act
        engine.Execute();
        Console.WriteLine($"Total Records inserted {recordsToProcess} : [{engine.Elapsed}] secs");
        #endregion

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<RawPayloadModel>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(recordsToProcess, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<RawPayloadModel>();
        await Task.Delay(1500); // release a bit of pressure to the code
    }

    [TestCase(200000)]
    [TestCase(300000)]
    [TestCase(400000)]
    [TestCase(500000)]
    public async Task BulkWriteAsync_Write_Unordered(int insertCount)
    {
        Console.WriteLine($"\nBulkWriteAsync (unordered): TestCase = {insertCount} records");

        //
        // Arrange
        var persons = new List<WriteModel<Person>>();
        for (var i = 1; i <= insertCount; i++)
        {
            var p = new Person
            {
                Active = true,
                FirstName = $"First Name {i}",
                LastName = $"Last Name {i}",
                Age = GetRandomAge(),
            };

            var inserModel = new InsertOneModel<Person>(p);
            persons.Add(inserModel);
        }

        //
        // Act
        var sw = ProcessStopwatch.Start();

        var schema = MongoService.GetSchema<Person>();
        var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = true };
        await schema.BulkWriteAsync(persons, options);

        sw.Stop();
        Console.WriteLine($"Total Records inserted {insertCount} : [{sw.Elapsed}] secs");

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<Person>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(insertCount, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<Person>();
    }

    [Test]
    [TestCase(50000)]
    [TestCase(100000)]
    [TestCase(200000)]
    [TestCase(300000)]
    [TestCase(400000)]
    [TestCase(500000)]
    public async Task BulkInsertAsync(int insertCount)
    {
        Console.WriteLine($"\nBulkInsertAsync : TestCase = {insertCount} records");

        //
        // Arrange
        var persons = new List<Person>();
        for (var i = 1; i <= insertCount; i++)
        {
            persons.Add(new Person
            {
                Active = true,
                FirstName = $"First Name {i}",
                LastName = $"Last Name {i}",
                Age = GetRandomAge(),
            });
        }

        //
        // Act
        var sw = ProcessStopwatch.Start();

        var schema = MongoService.GetSchema<Person>();
        await schema.BulkInsertAsync(persons);

        sw.Stop();
        Console.WriteLine($"Total Records inserted {insertCount} : [{sw.Elapsed}] secs");

        //
        // Assert
        var emptyFilter = CreateEmptyFilter<Person>();
        var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
        Assert.AreEqual(insertCount, actualTotalDocuments);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<Person>();
    }

    [Test]
    [TestCase(250000, new[] { 5, 14, 25, 39, 43, 63 }, 39)]
    [TestCase(628000, new[] { 25, 29, 44, 43, 63 }, 29)]
    public async Task BulkDeleteAsync(int insertDocumentCount, int[] ages, int lookupAge)
    {
        //
        // NOTE:
        // Delete all documents that has Age = 39
        //

        Console.WriteLine($"\nBulkDeleteAsync");

        // Arrange
        Console.WriteLine("Setup test data...\n");
        var persons = new List<Person>();
        for (var i = 1; i <= insertDocumentCount; i++)
        {
            persons.Add(new Person
            {
                Active = true,
                FirstName = $"First Name {i}",
                LastName = $"Last Name {i}",
                Age = GetRandomAge(ages),
            });
        }
        var schema = MongoService.GetSchema<Person>();
        await schema.BulkInsertAsync(persons);
        var dbTotalDocsAfterBulkInsert = await schema.Collection.CountDocumentsAsync(CreateEmptyFilter<Person>());
        Console.WriteLine($"Total documents found is {dbTotalDocsAfterBulkInsert}");

        var filter = Builders<Person>.Filter.Where(x => x.Age.Equals(lookupAge));
        var dbTotalAgeFilteredDocs = await schema.Collection.CountDocumentsAsync(filter);
        Console.WriteLine($"Total documents found for age {lookupAge} is {dbTotalAgeFilteredDocs}");

        //
        // Act
        Console.WriteLine("Executing...\n");
        var deleteFilter = Builders<Person>.Filter.Where(x => x.Age == lookupAge);

        var sw = ProcessStopwatch.Start();
        var deleteResult = await schema.BulkDeleteAsync(filter);
        sw.Stop();
        Console.WriteLine($"Delete targeted records took {sw.Elapsed} secs]");

        var dbTotalAgeFilteredDocsAfterBulkDelete = await schema.Collection.CountDocumentsAsync(filter);
        Console.WriteLine($"Total documents (bulk after delete) found for age {lookupAge} is {dbTotalAgeFilteredDocs}");

        var dbTotalRemainingDocs = await schema.Collection.CountDocumentsAsync(CreateEmptyFilter<Person>());
        Console.WriteLine($"Total documents found is {dbTotalAgeFilteredDocs}");

        //
        // Assert
        Assert.AreEqual(insertDocumentCount, dbTotalDocsAfterBulkInsert);

        Assert.True(dbTotalAgeFilteredDocsAfterBulkDelete == 0);

        var expecteddocsAfterDelete = dbTotalDocsAfterBulkInsert - dbTotalAgeFilteredDocs;
        Assert.AreEqual(expecteddocsAfterDelete, dbTotalRemainingDocs);

        //
        // Post db cleanup
        await ExecutePostTestCleanupAsync<Person>();
    }

    [Test]
    public async Task BulkUpdateAsync()
    {
        await Task.CompletedTask;
    }

    [Test]
    public async Task BulkUpdateAsync2()
    {
        await Task.CompletedTask;
    }

    //[Ignore("This test is for illustration only. Use other test methods")]
    //[TestCase(25000, 2)]
    ////[TestCase(50000)]
    ////[TestCase(70000)]
    ////[TestCase(100000)]
    ////[TestCase(200000)]
    ////[TestCase(300000)]
    ////[TestCase(400000)]
    ////[TestCase(500000)]
    //public async Task BulkWriteAsync_Write_RawData_MultiThread_IsOrdered_Archive(int insertingRecordCount, int spawnThreadCount)
    //{
    //    Console.WriteLine($"\nBulkWriteAsync_Write_RawData_MultiThread_IsOrdered (is ordered): TestCase = {insertingRecordCount} records");

    //    //
    //    // Arrange
    //    var groupRecordTotal = insertingRecordCount / spawnThreadCount;
    //    var groups = new List<List<WriteModel<RawPayloadModel>>>();

    //    for (var a = 0; a < spawnThreadCount; a++)
    //    {
    //        var rawPayloadModels = new List<WriteModel<RawPayloadModel>>();
    //        for (var i = 1; i <= groupRecordTotal; i++)
    //        {
    //            var model = await ReadJsonData<RawPayloadModel>("payload1.json");
    //            var inserModel = new InsertOneModel<RawPayloadModel>(model);
    //            rawPayloadModels.Add(inserModel);
    //        }

    //        groups.Add(rawPayloadModels);
    //    }

    //    Console.WriteLine($"list[0] [{groups[0].Count()}] records...");
    //    Console.WriteLine($"list[1] [{groups[1].Count()}] records...");
    //    Console.WriteLine($"Data preparation completed...");

    //    //
    //    // Act
    //    var schema = _mongoService.GetSchema<RawPayloadModel>();

    //    #region 1st way
    //    var task1 = schema.BulkWriteAsync(groups[0]);
    //    Console.WriteLine($"Task1 created...");

    //    Console.WriteLine("Task2: Bulk inserting begins...");
    //    var task2 = schema.BulkWriteAsync(groups[1]);
    //    #endregion

    //    #region 2nd way
    //    //var task1 = new Task(() =>
    //    //{
    //    //    var schema = _mongoService.GetSchema<RawPayloadModel>();

    //    //    Console.WriteLine("Task1: Bulk inserting begins...");
    //    //    schema.BulkWrite(groups[0]);
    //    //});
    //    //Console.WriteLine($"Task1 created...");

    //    //var task2 = new Task(() =>
    //    //{
    //    //    var schema = _mongoService.GetSchema<RawPayloadModel>();

    //    //    Console.WriteLine("Task2: Bulk inserting begins...");
    //    //    schema.BulkWrite(groups[1]);
    //    //});
    //    //Console.WriteLine($"Task2 created...");
    //    #endregion

    //    var sw = ProcessStopwatch.Start();

    //    Task.WaitAll(task1, task2);
    //    Console.WriteLine($"Waiting for all task to complete...");
    //    sw.Stop();

    //    Console.WriteLine($"Total Records inserted {insertingRecordCount} : [{sw.Elapsed}] secs");

    //    //
    //    // Assert
    //    var emptyFilter = CreateEmptyFilter<RawPayloadModel>();
    //    var actualTotalDocuments = await schema.Collection.CountDocumentsAsync(emptyFilter);
    //    Assert.AreEqual(insertingRecordCount, actualTotalDocuments);

    //    //
    //    // Post db cleanup
    //    //await ExecutePostTestCleanupAsync<RawPayloadModel>();
    //}
}
