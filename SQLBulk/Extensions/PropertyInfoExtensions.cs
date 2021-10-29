using SQLBulk.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SQLBulk.Extensions
{
    public static class PropertyInfoExtensions
    {
        internal static bool Ignore(this PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(IgnoreAttribute));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Null if not a primary key or no attributes otherwise columnname.</returns>
        internal static string GetPrimaryKeyName(this PropertyInfo property)
        {
            if (Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute != null)
            {
                return (Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute)?.Name;
            }
            return null;
        }

        internal static string GetColumnName(this PropertyInfo property)
        {
            if (Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) is ColumnAttribute attribute)
            {
                return attribute.Name;
            }
            return property.Name;
        }
    }
}
