using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
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
