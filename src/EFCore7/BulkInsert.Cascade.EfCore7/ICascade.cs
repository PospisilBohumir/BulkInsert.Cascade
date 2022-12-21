using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.EfCore7
{
    public interface ICascade<in T>
    {
        bool IsCascadeBeforeInsert { get; }
        Task InnerInsert(IEnumerable<T> source, IContextAdapter contextAdapter);
    }
}
