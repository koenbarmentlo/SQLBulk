using SQLBulk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLBulk.Validators
{
    internal static class InputValidator
    {
        internal static void ValidateItems<T>(T[] items)
        {
            if (items.Any(e => e == null))
            {
                throw new ArgumentException($"item in {nameof(items)} cannot be null");
            }
            var columnNames = typeof(T).GetColumnNames();
            if (columnNames.Length != columnNames.Distinct().Count())
            {
                throw new ArgumentException($"{typeof(T).Name} has duplicate column name");
            }
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.String:
                    throw new ArgumentException($"{typeof(T).Name} cannot be numeric or string.");
                default:
                    break;
            }
        }
    }
}
