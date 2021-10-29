using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
    /// <summary>
    /// Maps property to column. If omitted, property name is used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
