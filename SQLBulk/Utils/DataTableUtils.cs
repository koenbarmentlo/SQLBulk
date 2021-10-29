using SQLBulk.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("SQLBulk.UnitTests")]
namespace SQLBulk.Utils
{
    internal static class DataTableUtils
    {
        internal static DataTable GetDataTable<T>(ICollection<T> items)
        {
            var properties = typeof(T).GetProperties();
            using (var dataTable = new DataTable())
            {
                var columns = properties
                    .Select(p => new DataColumn(p.GetColumnName(), Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType))
                    .ToArray();
                dataTable.Columns.AddRange(columns);
                var rows = new List<DataRow>();
                foreach (var item in items)
                {
                    var row = dataTable.NewRow();
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var prop = properties[j];
                        row[prop.GetColumnName()] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }
                dataTable.AcceptChanges();
                return dataTable;
            }
        }
    }
}
