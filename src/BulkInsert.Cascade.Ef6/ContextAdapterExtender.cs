using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Ef6.Helpers;
using BulkInsert.Cascade.Ef6.HiLo;

namespace BulkInsert.Cascade.Ef6
{
    internal static class ContextAdapterExtender
    {
        private const int BulkCopyTimeout = 10 * 60;

        public static async Task BulkInsert<T>(this IContextAdapter contextAdapter, IEnumerable<T> entities,
            bool keepIdentity = false)
        {
            var sqlTransaction = contextAdapter.GeTransaction();
            using var sqlBulkCopy = new SqlBulkCopy(sqlTransaction.Connection,
                keepIdentity ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default, sqlTransaction)
            {
                BulkCopyTimeout = BulkCopyTimeout,
                DestinationTableName = contextAdapter.GetTableName<T>()
            };
            var table = contextAdapter.GetDataReader(entities);
            foreach (var column in table.Columns.OfType<DataColumn>().Select(o => o.ColumnName))
            {
                sqlBulkCopy.ColumnMappings.Add(column, column);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }

        public static DataTable GetDataReader<T>(this IContextAdapter contextAdapter, IEnumerable<T> entities)
        {
            var propertyMaps = contextAdapter.GetProperties<T>().ToArray();
            var table = propertyMaps.CreateTable();
            var columns = propertyMaps.Select(o => new
            {
                o.ColumnName,
                GetValue = o.IsDiscriminator
                    ? x => contextAdapter.GetDiscriminatorValue(x.GetType())
                    : ExpressHelper.GetPropGetter<T>(o.PropertyName).Compile(),
            }).ToArray();
            foreach (var entity in entities)
            {
                var dataRow = table.Rows.Add();
                foreach (var column in columns)
                {
                    dataRow[column.ColumnName] = column.GetValue(entity) ?? DBNull.Value;
                }
            }

            return table;
        }

        private static DataTable CreateTable(this IEnumerable<PropertyDescription> columns)
        {
            var result = new DataTable();
            foreach (var column in columns)
            {
                result.Columns.Add(column.ColumnName, GetType(column));
            }

            return result;
        }

        private static Type GetType(PropertyDescription column)
        {
            if (column.IsDiscriminator)
            {
                return typeof(string);
            }
            var propertyType = column.Type;
            return propertyType.IsNullable() ? propertyType.GetGenericArguments().Single() : propertyType;
        }

        public static async Task BulkInsertWithIdGeneration<T>(this IContextAdapter contextAdapter, IList<T> forSave)
            where T : class
        {
            if (forSave == null)
            {
                return;
            }

            var pk = contextAdapter.GetPk<T>();
            if (!pk.IsIdentity)
            {
                await contextAdapter.BulkInsert(forSave,true);
            }
            else
            {
                var isNew = ExpressHelper.IsPropertyEmpty<T>(pk.PropertyName, pk.Type).Compile();
                var newObjects = forSave.Where(isNew).ToArray();
                if (!newObjects.Any())
                {
                    return;
                }

                await contextAdapter.RetrieveIds(newObjects);
                await contextAdapter.BulkInsert(newObjects, true);
            }
        }

        public static async Task BulkInsertCascade<T>(this IContextAdapter contextAdapter, IList<T> forSave,
            params ICascade<T>[] cascades) where T : class
        {
            if (forSave == null || !forSave.Any())
            {
                return;
            }

            foreach (var cascade in cascades.Where(o => o.IsCascadeBeforeInsert))
            {
                await cascade.InnerInsert(forSave, contextAdapter);
            }

            await contextAdapter.BulkInsertWithIdGeneration(forSave);
            foreach (var cascade in cascades.Where(o => !o.IsCascadeBeforeInsert))
            {
                await cascade.InnerInsert(forSave, contextAdapter);
            }
        }
    }
}