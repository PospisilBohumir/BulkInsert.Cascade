using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BulkInsert.Cascade
{
    public interface ICascade<in T>
    {
        bool IsCascadeBeforeInsert { get; }
        Task InnerInsert(IEnumerable<T> source, DbContext context, DbContextTransaction transaction);
    }
}