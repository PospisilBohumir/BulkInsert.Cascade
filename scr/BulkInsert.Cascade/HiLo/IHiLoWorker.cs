using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.HiLo
{
    internal interface IHiLoWorker
    {
        Type PropertyType { get; }
        Task RetrieveIdsLong<TEntity>(DbContext context, TEntity[] entities);
    }
}