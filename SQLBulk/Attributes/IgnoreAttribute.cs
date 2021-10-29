using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
    /// <summary>
    /// Ignore property with inserting or updating.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
