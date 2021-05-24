/*
 * References:
 *  Search collection given specific criteria or filter
 *      - https://stackoverflow.com/questions/9314886/getting-an-item-count-with-mongodb-c-sharp-driver-query-builder
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoClient.Tests.Base;
using MongoClient.Tests.Models;
using MongoDB.Driver;
using Nautilus.Diagnostics.Utilities;
using NUnit.Framework;

namespace MongoClient.Tests
{
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
        [TestCase(500000)]
        public async Task BulkInsertAsync(int insertCount)
        {
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

            var schema = _mongoService.GetSchema<Person>();
            await schema.BulkInsertAsync(persons);

            sw.Stop();
            Console.WriteLine($"Total Records inserted {insertCount} : [{sw.Elapsed}]");

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
            var schema = _mongoService.GetSchema<Person>();
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
            Console.WriteLine($"BulkDeleteAsync : [{sw.Elapsed}]");

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
    }
}