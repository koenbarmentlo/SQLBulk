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
        internal static DataTable GetDataTable<T>(T[] items)
        {
            var properties = typeof(T).GetProperties();
            using (var dataTable = new DataTable())
            {
                var columns = properties
                    .Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType))
                    .ToArray();
                dataTable.Columns.AddRange(columns);
                var rows = new List<DataRow>();
                for (int i = 0; i < items.Length; i++)
                {
                    var row = dataTable.NewRow();
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var prop = properties[j];
                        row[prop.Name] = prop.GetValue(items[i]) ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }
                dataTable.AcceptChanges();
                return dataTable;
            }
        }
    }
}
