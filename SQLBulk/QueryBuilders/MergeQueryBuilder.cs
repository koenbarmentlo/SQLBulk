using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLBulk.QueryBuilders
{
    internal class MergeQueryBuilder
    {
        private MergeQueryBuilder() { }
        private readonly MergeQuery mergeQuery = new MergeQuery();

        public static MergeQueryBuilder StartBuilding()
        {
            return new MergeQueryBuilder();
        }

        public MergeQueryBuilder SetSourceTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException($"{nameof(tableName)} is null");
            }
            mergeQuery.SourceTableName = tableName;
            return this;
        }

        public MergeQueryBuilder SetDestinationTable(string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException($"{nameof(tableName)} is null");
            }
            mergeQuery.DestinationTableName = tableName;
            return this;
        }

        public MergeQueryBuilder SetCustomMatchExpression(string matchExpression)
        {
            if(mergeQuery.MatchColumns == null || !mergeQuery.MatchColumns.Any())
            {
                throw new ArgumentException("Use matchColumns or matchExpression. Not both.");
            }
            mergeQuery.CustomMatchExpression = matchExpression;
            return this;
        }

        public MergeQueryBuilder SetMatchOn(params string[] matchColumns)
        {
            if(mergeQuery.MatchIsAlwaysFalse)
            {
                throw new ArgumentException($"Can't use MatchAlwaysFalse and SetMatchOn. Choose one.");
            }
            if(!string.IsNullOrWhiteSpace(mergeQuery.CustomMatchExpression))
            {
                throw new ArgumentException("Use matchColumns or matchExpression. Not both.");
            }
            if(matchColumns == null)
            {
                throw new ArgumentNullException($"{nameof(matchColumns)} is null");
            }
            if (!matchColumns.Any())
            {
                throw new ArgumentException($"{nameof(matchColumns)} is empty");
            }
            mergeQuery.MatchColumns = matchColumns;
            return this;
        }

        public MergeQueryBuilder MatchAlwaysFalse()
        {
            if (mergeQuery.MatchColumns.Any())
            {
                throw new ArgumentException($"Can't use MatchAlwaysFalse and SetMatchOn. Choose one.");
            }
            if (!mergeQuery.UseInsert)
            {
                throw new ArgumentException($"Call UseInsert (first)");
            }
            mergeQuery.MatchIsAlwaysFalse = true;
            return this;
        }

        public MergeQueryBuilder UseInsert()
        {
            mergeQuery.UseInsert = true;
            return this;
        }

        public MergeQueryBuilder UseDelete()
        {
            mergeQuery.UseDelete = true;
            return this;
        }

        public MergeQueryBuilder SetColumnNames(string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException($"{nameof(columns)} is null");
            }
            if (!columns.Any())
            {
                throw new ArgumentException($"{nameof(columns)} is empty");
            }
            mergeQuery.ColumnNames = columns;
            return this;
        }

        public string Build()
        {
            return mergeQuery.ToString();
        }

        private class MergeQuery
        {
            public string SourceTableName { get; set; }
            public string DestinationTableName { get; set; }
            public string[] MatchColumns { get; set; }
            /// <summary>
            /// See MERGE statement documentation of SQL Server
            /// </summary>
            public string CustomMatchExpression { get; set; }
            public bool MatchIsAlwaysFalse { get; set; } = false;
            public bool UseInsert { get; set; } = false;
            public bool UseDelete { get; set; } = false;
            public string[] ColumnNames { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(SourceTableName) || string.IsNullOrWhiteSpace(DestinationTableName))
                {
                    throw new QueryBuilderException("Set SourceTableName and DestinationTableName.");
                }
                if (!MatchIsAlwaysFalse)
                {
                    if (string.IsNullOrWhiteSpace(CustomMatchExpression) && (MatchColumns == null || !MatchColumns.Any()))
                    {
                        throw new QueryBuilderException("Missing match columns or match expression.");
                    }
                }

                string query = $"MERGE {DestinationTableName} AS t USING {SourceTableName} AS s ON (";
                if (MatchIsAlwaysFalse)
                {
                    query += "1 = 2";
                }
                else if (!string.IsNullOrWhiteSpace(CustomMatchExpression))
                {
                    query += CustomMatchExpression;
                }
                else 
                {
                    query += string.Join(" AND ", MatchColumns.Select(m =>
                    {
                        return $"s.{m} = t.{m}";
                    }));
                }
                query += ") WHEN MATCHED THEN UPDATE SET ";
                query += string.Join(",", ColumnNames.Where(p => !MatchColumns.Contains(p)).Select(name =>
                {
                    return $"t.{name} = s.{name}";
                }));
                if (UseInsert)
                {
                    query += " WHEN NOT MATCHED BY TARGET THEN INSERT (";
                    query += string.Join(",", ColumnNames);
                    query += ") VALUES (";
                    query += string.Join(",", ColumnNames.Select(c => "s." + c));
                    query += ")";
                }
                if (UseDelete)
                {
                    query += " WHEN NOT MATCHED BY SOURCE THEN DELETE";
                }
                query += ";";
                return query;
            }
        }

        public class QueryBuilderException : Exception
        {
            public QueryBuilderException()
            {
            }

            public QueryBuilderException(string message) : base(message)
            {
            }

            public QueryBuilderException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected QueryBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
