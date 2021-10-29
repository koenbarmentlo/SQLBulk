using System;
using System.Collections.Generic;
using System.Text;

namespace SQLBulk.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
