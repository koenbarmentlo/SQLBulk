using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
    /// <summary>
    /// Map class to table. If omitted, classname is used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }
        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
