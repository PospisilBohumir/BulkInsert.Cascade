﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Helpers;
using BulkInsert.Cascade.HiLo;
using EntityFramework.Metadata;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade
{
    public static class ContextExtender
    {
        private const int BulkCopyTimeout = 10 * 60;

        /// <summary>
        ///     Inserts <paramref name="entities" /> in Bulk operation
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Database Context</param>
        /// <param name="transaction">Open transaction</param>
        /// <param name="entities">List of entities which should be inserted into database</param>
        /// <param name="keepIdentity">True, when id should be inserted ,false when id should be generated</param>
        public static async Task BulkInsert<T>(this DbContext context, DbContextTransaction transaction, IEnumerable<T> entities,
            bool keepIdentity = false)
        {
            var underlyingTransaction = (SqlTransaction) transaction.UnderlyingTransaction;

            using var sqlBulkCopy = new SqlBulkCopy(underlyingTransaction.Connection,
                keepIdentity ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default, underlyingTransaction)
            {
                BulkCopyTimeout = BulkCopyTimeout,
                DestinationTableName = context.Db<T>().TableName
            };
            var table = context.GetDataReader(entities);
            foreach (var column in table.Columns.OfType<DataColumn>().Select(o => o.ColumnName))
            {
                sqlBulkCopy.ColumnMappings.Add(column, column);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }

        public static DataTable GetDataReader<T>(this DbContext context, IEnumerable<T> entities)
        {
            var propertyMaps = context.Db<T>().Properties.Where(o => !o.IsNavigationProperty).ToArray();
            var table = propertyMaps.CreateTable();
            var columns = propertyMaps.Select(o => new
            {
                o.ColumnName,
                GetValue = o.IsDiscriminator
                    ? x => typeof(T).Name
                    : ExpressHelper.GetPropGetter<T>(o.PropertyName).Compile()
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

        private static DataTable CreateTable(this IEnumerable<IPropertyMap> columns)
        {
            var result = new DataTable();
            foreach (var column in columns)
            {
                var propertyType = column.Type;
                result.Columns.Add(column.ColumnName,
                    propertyType.IsNullable() ? propertyType.GetGenericArguments().Single() : propertyType);
            }

            return result;
        }

        public static async Task BulkInsertWithIdGeneration<T>(this DbContext context, DbContextTransaction transaction, IList<T> forSave)
            where T : class
        {
            if (forSave == null)
            {
                return;
            }

            var pk = context.GetPk<T>();
            if (!pk.IsIdentity)
            {
                await context.BulkInsert(transaction, forSave, true);
            }
            else
            {
                var isNew = ExpressHelper.IsPropertyEmpty<T>(pk.PropertyName, pk.Type).Compile();
                var newObjects = forSave.Where(isNew).ToArray();
                if (!newObjects.Any())
                {
                    return;
                }

                await context.RetrieveIds(newObjects);
                await context.BulkInsert(transaction, newObjects, true);
            }
        }

        public static async Task BulkInsertCascade<T>(this DbContext context, DbContextTransaction transaction, IList<T> forSave,
            params ICascade<T>[] cascades) where T : class
        {
            if (forSave == null || !forSave.Any())
            {
                return;
            }

            foreach (var cascade in cascades.Where(o => o.IsCascadeBeforeInsert))
            {
                await cascade.InnerInsert(forSave, context, transaction);
            }

            await context.BulkInsertWithIdGeneration(transaction, forSave);
            foreach (var cascade in cascades.Where(o => !o.IsCascadeBeforeInsert))
            {
                await cascade.InnerInsert(forSave, context, transaction);
            }
        }
    }
}