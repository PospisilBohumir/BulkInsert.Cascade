using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.EfCore7.HiLo
{
    //NOTE: I have no idea how to create generic version of my hi/lo algorithm so I did this copy- paste :-(
    internal static class HiLoFactory
    {
        private static readonly Dictionary<Type, IHiLoWorker> Worker = new IHiLoWorker[]
        {
            new HiLoWorkerShort(),
            new HiLoWorkerInt(),
            new HiLoWorkerLong(),
        }.ToDictionary(o => o.PropertyType);

        public static async Task RetrieveIds<TEntity>(this IContextAdapter contextAdapter, TEntity[] entities)
            where TEntity : class
        {
            await Worker[contextAdapter.GetPk<TEntity>().Type].RetrieveIdsLong(contextAdapter, entities);
        }
    }
}