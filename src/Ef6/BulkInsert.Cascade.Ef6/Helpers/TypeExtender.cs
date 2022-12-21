using System;

namespace BulkInsert.Cascade.Ef6.Helpers
{
    internal static class TypeExtender
    {
        internal static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}