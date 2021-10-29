using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLBulk.Extensions;
using SQLBulk.UnitTests.Entities;
using System.Diagnostics;
using System.Data;
using Dapper;

namespace SQLBulk.UnitTests.SqlServer
{
    public class BulkTests
    {
        private TestDatabase _testDatabase;

        [SetUp]
        public void Setup()
        {
            _testDatabase = new TestDatabase("temp_test_db");
            _testDatabase.CreateDatabase();
        }

        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        [TestCase(1_000_000)]
        [TestCase(2_000_000)]
        [TestCase(5_000_000)]
        [TestCase(10_000_000)]
        public void BulkInsertTest(int numberOfRecords)
        {
            var customers = Customer.GetCustomers(numberOfRecords);
            using var connection = _testDatabase.GetNewConnection();
            connection.Open();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            connection.BulkInsert(customers.ToArray(), bulkOptions: new Options.BulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue
            });
            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
        }

        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        //[TestCase(500_000)]
        //[TestCase(1_000_000)]
        //[TestCase(2_000_000)]
        //[TestCase(5_000_000)]
        //[TestCase(10_000_000)]
        public void BulkUpdateTest(int numberOfRecords)
        {
            var customers = Customer.GetCustomers(numberOfRecords);
            using var connection = _testDatabase.GetNewConnection();
            connection.Open();

            connection.BulkInsert(customers.ToArray(), bulkOptions: new Options.BulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue
            });
            connection.BulkInsert(new Customer[]
                {
                    new Customer()
                    {
                        FirstName = "Japie",
                        BirthDate = DateTime.Now,
                        Gender = 'M',
                        SecondName = "Jansie",
                        MoneySpent = 10M,
                        NumberOfComplaints = 1,
                        Type = 'A',
                    }
                });

            customers.ForEach(x => x.MoneySpent = 14M);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            connection.BulkUpdate(customers.ToArray(), bulkOptions: new Options.UpdateBulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue,
                MatchOnColumnNames = new string[] { nameof(Customer.FirstName) }
            });
            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
            var updated = connection.Query<Customer>("SELECT * FROM customer");
            Assert.IsTrue(updated.Take(100).ToList().All(x => x.MoneySpent == 14M));
            Assert.AreEqual(10M, updated.Last().MoneySpent);
        }

        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        //[TestCase(1_000_000)]
        //[TestCase(2_000_000)]
        public void BulkInsertOrUpdateTest(int numberOfRecords)
        {
            int numberOfRecordsHalf = numberOfRecords / 2;
            var customers = Customer.GetCustomers(numberOfRecords);
            using var connection = _testDatabase.GetNewConnection();
            connection.Open();

            connection.BulkInsert(customers.Take(numberOfRecordsHalf).ToArray(), bulkOptions: new Options.BulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue
            });

            customers.Take(numberOfRecordsHalf).ToList().ForEach(x => x.MoneySpent = 14M);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            connection.BulkInsertOrUpdate(customers.ToArray(), bulkOptions: new Options.UpdateBulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue,
                MatchOnColumnNames = new string[] { nameof(Customer.FirstName) }
            });
            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
            var updated = connection.Query<Customer>("SELECT * FROM customer");
            Assert.AreEqual(updated.Count(), numberOfRecords);
            Assert.IsTrue(updated.Take(numberOfRecordsHalf).All(x => x.MoneySpent == 14M));
            Assert.IsTrue(updated.Skip(numberOfRecordsHalf).All(x => x.MoneySpent == 12M));
        }

        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        //[TestCase(1_000_000)]
        //[TestCase(2_000_000)]
        public void BulkInsertOrUpdateOrDeleteTest(int numberOfRecords)
        {
            int numberOfRecordsHalf = numberOfRecords / 2;
            var customers = Customer.GetCustomers(numberOfRecords);
            using var connection = _testDatabase.GetNewConnection();
            connection.Open();

            connection.BulkInsert(customers.Take(numberOfRecordsHalf).ToArray(), bulkOptions: new Options.BulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue
            });

            var halfOfCustomers = customers.Take(numberOfRecordsHalf);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            connection.BulkInsertOrUpdateOrDelete(halfOfCustomers.ToArray(), bulkOptions: new Options.UpdateBulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue,
                MatchOnColumnNames = new string[] { nameof(Customer.FirstName) }
            });
            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
            var updated = connection.Query<Customer>("SELECT * FROM customer");
            Assert.AreEqual(updated.Count(), numberOfRecordsHalf);
        }

        [TearDown]
        public void Teardown()
        {
            _testDatabase.DropDatabase();
        }
    }
}
