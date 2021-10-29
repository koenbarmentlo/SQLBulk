﻿using SQLBulk.Attributes;
using SQLBulk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLBulk.Extensions
{
    internal static class TypeExtensions
    {
        internal static string GetTableName(this Type type)
        {
            if (Attribute.GetCustomAttribute(type, typeof(TableAttribute)) is TableAttribute attribute)
            {
                return attribute.TableName;
            }
            return type.Name;
        }

        internal static PrimaryKey[] GetPrimaryKeys(this Type type)
        {
            return type
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)))
                .Select(prop =>
                {
                    var pkAttribute = Attribute.GetCustomAttribute(prop, typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
                    if (Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) is ColumnAttribute columnAttribute)
                    {
                        return new PrimaryKey() { ColumnName = columnAttribute.Name, AutoGenerated = pkAttribute.AutoGenerated };
                    }
                    return new PrimaryKey() { ColumnName = prop.Name, AutoGenerated = pkAttribute.AutoGenerated };
                })
                .ToArray();
        }

        internal static string[] GetColumnNames(this Type type)
        {
            return type.GetProperties()
                .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreAttribute)))
                .Select(prop =>
                {
                    if (Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) is ColumnAttribute attribute)
                    {
                        return attribute.Name;
                    }
                    return prop.Name;
                })
                .ToArray();
        }
    }
}