using System;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.Ef6.HiLo
{
    internal interface IHiLoWorker
    {
        Type PropertyType { get; }
        Task RetrieveIdsLong<TEntity>(IContextAdapter context, TEntity[] entities);
    }
}