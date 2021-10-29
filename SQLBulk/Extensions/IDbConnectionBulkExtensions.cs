using Microsoft.Data.SqlClient;
using MySqlConnector;
using SQLBulk.Options;
using SQLBulk.QueryBuilders;
using SQLBulk.Validators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBulk.Extensions
{
    public static class IDbConnectionBulkExtensions
    {
        public static void BulkInsert<T>(this IDbConnection connection, T[] items, BulkOptions bulkOptions = null)
        {
            if(connection is SqlConnection sqlConnection)
            {
                sqlConnection.BulkInsertSqlServer(items, bulkOptions);
                return;
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static Task BulkInsertAsync<T>(this IDbConnection connection, T[] items, BulkOptions bulkOptions = null)
        {
            if (connection is SqlConnection sqlConnection)
            {
                return sqlConnection.BulkInsertSqlServerAsync(items, bulkOptions);
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static void BulkUpdate<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                sqlConnection.BulkUpdateSqlServer(items, bulkOptions);
                return;
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static Task BulkUpdateAsync<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                return sqlConnection.BulkUpdateSqlServerAsync(items, bulkOptions);
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static void BulkInsertOrUpdate<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                sqlConnection.BulkInsertOrUpdateSqlServer(items, bulkOptions);
                return;
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static Task BulkInsertOrUpdateAsync<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                return sqlConnection.BulkInsertOrUpdateSqlServerAsync(items, bulkOptions);
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static void BulkInsertOrUpdateOrDelete<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                sqlConnection.BulkInsertOrUpdateOrDeleteSqlServer(items, bulkOptions);
                return;
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }

        public static Task BulkInsertOrUpdateOrDeleteAsync<T>(this IDbConnection connection, T[] items, UpdateBulkOptions bulkOptions)
        {
            if (connection is SqlConnection sqlConnection)
            {
                return sqlConnection.BulkInsertOrUpdateOrDeleteSqlServerAsync(items, bulkOptions);
            }
            throw new NotImplementedException($"Only SqlConnection is currently supported.");
        }
    }
}
