using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.EfCore7
{
    /// <summary>
    ///     Extender for cascade bulk-insert
    /// </summary>
    public static class ContextExtender
    {
        private const int BulkCopyTimeout = 10 * 60;

        /// <summary>
        ///     Starts transaction and inserts <paramref name="entities" /> in Bulk operation
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Database Context</param>
        /// <param name="entities">List of entities which should be inserted into database</param>
        /// <param name="keepIdentity">True, when id should be inserted ,false when id should be generated</param>
        public static async Task BulkInsert<T>(this DbContext context, IEnumerable<T> entities,
            bool keepIdentity = false)
        {
            if (context.Database.CurrentTransaction != null)
            {
                await new ContextAdapter(context, context.Database.CurrentTransaction).BulkInsert(entities,
                    keepIdentity);
            }
            else
            {
                await using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await new ContextAdapter(context, transaction).BulkInsert(entities, keepIdentity);
                    await transaction.CommitAsync();
                }
            }
        }

        /// <summary>
        ///     Starts transaction and inserts <paramref name="entities" /> in Bulk operation and retrieve primary key using Hi/Lo
        ///     algorithm if key is identity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Database Context</param>
        /// <param name="entities">List of entities which should be inserted into database</param>
        public static async Task BulkInsertWithIdGeneration<T>(this DbContext context, IList<T> entities)
            where T : class
        {
            if (context.Database.CurrentTransaction != null)
            {
                await new ContextAdapter(context, context.Database.CurrentTransaction)
                    .BulkInsertWithIdGeneration(entities);
            }
            else
            {
                await using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await new ContextAdapter(context, transaction).BulkInsertWithIdGeneration(entities);
                    await transaction.CommitAsync();
                }
            }
        }


        /// <summary>
        ///     Starts transaction and inserts <paramref name="entities" /> in Bulk operation in the <paramref name="cascades" />
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Database Context</param>
        /// <param name="entities">List of entities which should be inserted into database</param>
        /// <param name="cascades">description of insert cascade</param>
        public static async Task BulkInsertCascade<T>(this DbContext context, IList<T> entities,
            params ICascade<T>[] cascades) where T : class
        {
            if (context.Database.CurrentTransaction != null)
            {
                await new ContextAdapter(context, context.Database.CurrentTransaction)
                    .BulkInsertCascade(entities, cascades);
            }
            else
            {
                await using (var transaction = await context.Database.BeginTransactionAsync())

                {
                    await new ContextAdapter(context, transaction).BulkInsertCascade(entities, cascades);
                    await transaction.CommitAsync();
                }
            }
        }
    }
}