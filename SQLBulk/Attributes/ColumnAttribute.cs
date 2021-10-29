using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
