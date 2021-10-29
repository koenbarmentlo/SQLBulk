using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulk.UnitTests.SqlServer
{
    public class TestDatabase
    {
        private const string DefaultConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={0};Integrated Security=True;";
        private const string LocalDbEnvironmentVariableKey = "LocalDbConnectionString";
        private static readonly string LocalDbMaster = (Environment.GetEnvironmentVariable(LocalDbEnvironmentVariableKey) ?? DefaultConnectionString).Replace("{0}", "master");
        private static readonly string TestConnectionStringTemplate = (Environment.GetEnvironmentVariable(LocalDbEnvironmentVariableKey) ?? DefaultConnectionString) + @"MultipleActiveResultSets=True;AttachDBFilename={1}.mdf";

        private readonly string TestConnectionString;

        private readonly string _databaseName;

        private readonly string DatabaseFileName;

        private bool _dbCreated = false;

        public TestDatabase(string databaseName)
        {
            if(string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Pass a valid database name");
            }
            _databaseName = databaseName;
            DatabaseFileName = DatabaseFilePath();
            TestConnectionString = string.Format(TestConnectionStringTemplate, databaseName, DatabaseFileName);
        }

        internal void DropDatabase()
        {
            if(DatabaseExists())
            {
                using (var connection = new SqlConnection(LocalDbMaster))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format(@"USE master;
ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [{0}];
",
                        _databaseName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateDatabase()
        {
            if (DatabaseExists())
            {
                var isDetached = DetachDatabase();
                if (!isDetached) return; //reuse database
            }
            CleanupDatabase();

            using (var connection = new SqlConnection(LocalDbMaster))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}.mdf')",
                    _databaseName,
                    DatabaseFileName);
                cmd.ExecuteNonQuery();
            }
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
CREATE TABLE customer (
	[FirstName] VARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[BirthDate] DATETIME2 NOT NULL,
	[NumberOfComplaints] INT NOT NULL,
	[MoneySpent] DECIMAL NULL,
	[Type] CHAR NULL DEFAULT 'G',
	[Gender] NCHAR NULL,
	CONSTRAINT PK_Customer PRIMARY KEY ([FirstName], [LastName], [BirthDate]),
	CONSTRAINT CHK_Gender CHECK ([Gender] = 'M' OR [Gender] = 'F')
);
--GO;
--CREATE INDEX IDX_Type ON customer ([Type])";
                cmd.ExecuteNonQuery();

                var tvpCmd = connection.CreateCommand();
                tvpCmd.CommandTimeout = 1000;
                tvpCmd.CommandText = @"
IF TYPE_ID(N'CustomerTVP') IS NULL
BEGIN
CREATE TYPE CustomerTVP AS TABLE ( 
    [FirstName] VARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[BirthDate] DATETIME2 NOT NULL,
	[NumberOfComplaints] INT NOT NULL,
	[MoneySpent] DECIMAL NULL,
	[Type] CHAR NULL DEFAULT 'G',
	[Gender] NCHAR NULL
)
END";
                tvpCmd.ExecuteNonQuery();
            }
            _dbCreated = true;
        }

        public SqlConnection GetNewConnection()
        {
            if(!_dbCreated)
            {
                throw new Exception("Call create database first");
            }
            return new SqlConnection(TestConnectionString);
        }

        private bool DatabaseExists()
        {
            using (var connection = new SqlConnection(LocalDbMaster))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {

                    cmd.CommandText = String.Format(@"
IF DB_ID('{0}') IS NULL
SELECT 0 AS DbExists
ELSE
SELECT 1 AS DbExists", _databaseName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        return reader.GetInt32(0) == 1;
                    }
                }
            }
        }

        private void CleanupDatabase()
        {
            try
            {
                if (File.Exists(DatabaseFileName + ".mdf")) File.Delete(DatabaseFileName + ".mdf");
                if (File.Exists(DatabaseFileName + "_log.ldf")) File.Delete(DatabaseFileName + "_log.ldf");
            }
            catch
            {
                Console.WriteLine("Could not delete the files (open in Visual Studio?)");
            }
        }
        private bool DetachDatabase()
        {
            using (var connection = new SqlConnection(LocalDbMaster))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {

                    cmd.CommandText = string.Format("exec sp_detach_db '{0}'", _databaseName);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch
                    {
                        Console.WriteLine("Could not detach");
                        return false;
                    }
                }
            }
        }
        private string DatabaseFilePath()
        {
            return Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                _databaseName);
        }
    }
}
