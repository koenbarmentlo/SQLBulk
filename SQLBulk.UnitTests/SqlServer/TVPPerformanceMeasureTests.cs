using Dapper;
using NUnit.Framework;
using SQLBulk.UnitTests.Entities;
using SQLBulk.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLBulk.Extensions;

namespace SQLBulk.UnitTests.SqlServer
{
    /// <summary>
    /// Only used for performance comparison.
    /// </summary>
    public class TVPPerformanceMeasureTests
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
        //[TestCase(10_000_000)]
        public void BulkInsertPerformanceMeasureTest(int numberOfRecords)
        {
            var customers = Customer.GetCustomers(numberOfRecords);

            using var connection = _testDatabase.GetNewConnection();
            connection.Open();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var dataTable = DataTableUtils.GetDataTable(customers.ToArray()))
            {
                var parameters = new { tvp = dataTable.AsTableValuedParameter("CustomerTVP") };
                var result = connection.Query($@"
MERGE Customer AS Destination USING @tvp AS Source
ON Destination.FirstName = Source.FirstName
WHEN Matched THEN
UPDATE SET BirthDate = Source.BirthDate,
           Gender = Source.Gender,
           LastName = Source.LastName,
           MoneySpent = Source.MoneySpent,
           NumberOfComplaints = Source.NumberOfComplaints,
           Type = Source.Type
WHEN NOT MATCHED BY TARGET THEN
INSERT (FirstName, BirthDate, Gender, LastName, MoneySpent, NumberOfComplaints, Type) 
VALUES (Source.FirstName, Source.BirthDate, Source.Gender, Source.LastName, Source.MoneySpent, Source.NumberOfComplaints, Source.Type)
OUTPUT inserted.*;
", parameters);
            }

            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
        }

        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        [TestCase(1_000_000)]
        [TestCase(2_000_000)]
        //[TestCase(5_000_000)]
        //[TestCase(10_000_000)]
        public void BulkUpdatePerformanceMeasureTest(int numberOfRecords)
        {
            var customers = Customer.GetCustomers(numberOfRecords);

            using var connection = _testDatabase.GetNewConnection();
            connection.Open();

            connection.BulkInsert(customers.ToArray(), bulkOptions: new Options.BulkOptions()
            {
                SqlBulkCopyTimeoutInSeconds = int.MaxValue
            });

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var dataTable = DataTableUtils.GetDataTable(customers.ToArray()))
            {
                var parameters = new { tvp = dataTable.AsTableValuedParameter("CustomerTVP") };
                var result = connection.Query($@"
MERGE Customer AS Destination USING @tvp AS Source
ON Destination.FirstName = Source.FirstName
WHEN Matched THEN
UPDATE SET BirthDate = Source.BirthDate,
           Gender = Source.Gender,
           LastName = Source.LastName,
           MoneySpent = Source.MoneySpent,
           NumberOfComplaints = Source.NumberOfComplaints,
           Type = Source.Type
WHEN NOT MATCHED BY TARGET THEN
INSERT (FirstName, BirthDate, Gender, LastName, MoneySpent, NumberOfComplaints, Type) 
VALUES (Source.FirstName, Source.BirthDate, Source.Gender, Source.LastName, Source.MoneySpent, Source.NumberOfComplaints, Source.Type)
OUTPUT inserted.*;
", parameters);
            }

            stopwatch.Stop();
            TestContext.WriteLine($"Elapsed seconds with {numberOfRecords} records: {stopwatch.ElapsedMilliseconds * 0.001}.");
        }

        [TearDown]
        public void Teardown()
        {
            _testDatabase.DropDatabase();
        }
    }
}
