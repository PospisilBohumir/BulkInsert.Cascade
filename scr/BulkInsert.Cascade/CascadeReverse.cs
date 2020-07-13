using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.Helpers;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade
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

        public async Task InnerInsert(IEnumerable<TSource> source, DbContext context, DbContextTransaction transaction)
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

            var copyPk = GetCopyPk(context);
            var destinations = s.Select(extractor).Distinct().ToArray();
            await context.BulkInsertCascade(transaction, destinations, _cascades);
            foreach (var destination in s)
            {
                copyPk(destination, destination);
            }
        }

        private Action<TSource, TSource> GetCopyPk(DbContext context)
        {
            var path = ExpressHelper.GetPath(_extractor);
            var property = context.Db<TSource>().Properties.Single(o => o.NavigationProperty?.PropertyName == path);
            var expression = ExpressHelper.CreateCopy<TSource, TSource>(
                $"{path}.{context.GetPkName<TSource>()}", property.PropertyName);
            return expression.Compile();
        }

        public bool IsCascadeBeforeInsert => true;

        public override string ToString()
        {
            return $"{nameof(_extractor)}: {ExpressHelper.GetPath(_extractor)}, Cascades Count {_cascades?.Length}";
        }
    }
}