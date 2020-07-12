using System;
using System.Data.Entity;
using System.Linq;
using EntityFramework.Metadata;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade.Helpers
{
    internal static class MetadataExtender
    {
        internal static Action<T, TValue> GetPkSetter<T,TValue>(this DbContext context)
        {
            return ExpressHelper.GetPropSetter<T, TValue>(context.GetPkName<T>()).Compile();
        }

        internal static string GetPkName<T>(this DbContext context)
        {
            return context.GetPk<T>().PropertyName;
        }

        internal static IPropertyMap GetPk<T>(this DbContext context)
        {
            return context.Db<T>().Pks.Single();
        }
    }
}