using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.Shared.Helpers;

namespace BulkInsert.Cascade.Shared
{
    public class CascadeReverse<TSource, TDestination> : ICascade<TSource> where TDestination : class
    {
        private readonly Expression<Func<TSource, TDestination>> _extractor;
        private readonly ICascade<TDestination>[] _cascades;

        public CascadeReverse(
            Expression<Func<TSource, TDestination>> extractor,
            params ICascade<TDestination>[] cascades)
        {
            _extractor = extractor;
            _cascades = cascades;
        }

        public async Task InnerInsert(IEnumerable<TSource> source, IContextAdapter contextAdapter)
        {
            if (source == null)
            {
                return;
            }

            var extractor = _extractor.Compile();
            var s = source.Where(o => extractor(o) != null).ToArray();
            if (!s.Any())
            {
                return;
            }

            var copyPk = GetCopyPk(contextAdapter);
            var destinations = s.Select(extractor).Distinct().ToArray();
            await contextAdapter.BulkInsertCascade( destinations, _cascades);
            foreach (var destination in s)
            {
                copyPk(destination, destination);
            }
        }

        private Action<TSource, TSource> GetCopyPk(IContextAdapter contextAdapter)
        {
            var path = ExpressHelper.GetPath(_extractor);
            var propertyName = contextAdapter.GetBackwardKeyProperty<TSource>(path);
            var expression = ExpressHelper.CreateCopy<TSource, TSource>(
                $"{path}.{contextAdapter.GetPk<TSource>().PropertyName}", propertyName);
            return expression.Compile();
        }

        public bool IsCascadeBeforeInsert => true;

        public override string ToString()
        {
            return $"{nameof(_extractor)}: {ExpressHelper.GetPath(_extractor)}, Cascades Count {_cascades?.Length}";
        }
    }
}