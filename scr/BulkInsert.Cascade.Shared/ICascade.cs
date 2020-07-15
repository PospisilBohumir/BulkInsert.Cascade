using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.Shared
{
    public interface ICascade<in T>
    {
        bool IsCascadeBeforeInsert { get; }
        Task InnerInsert(IEnumerable<T> source, IContextAdapter contextAdapter);
    }
}
