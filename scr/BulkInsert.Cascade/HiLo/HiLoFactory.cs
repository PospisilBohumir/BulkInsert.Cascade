using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade.HiLo
{
    //NOTE: I have no idea how to create generic version of my hi/lo algorithm so I did this copy- paste :-(
    public static class HiLoFactory
    {
        private static readonly Dictionary<Type, IHiLoWorker> Worker = new IHiLoWorker[]
        {
            new HiLoWorkerShort(),
            new HiLoWorkerInt(),
            new HiLoWorkerLong(),
        }.ToDictionary(o => o.PropertyType);

        public static async Task RetrieveIds<TEntity>(this DbContext context, TEntity[] entities)
            where TEntity : class
        {
            await Worker[context.Db<TEntity>().Pks.Single().Type].RetrieveIdsLong(context, entities);
        }
    }
}