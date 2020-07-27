using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BulkInsert.Cascade.Shared.Helpers;

namespace BulkInsert.Cascade.Shared
{
    public class Cascade<TSource, TDestination> : ICascade<TSource> where TDestination : class
    {
        private readonly ICascade<TDestination>[] _cascades;
        private readonly Expression<Func<TSource, IEnumerable<TDestination>>> _extractor;
        private readonly string _propertyName;


        public Cascade(Expression<Func<TSource, IEnumerable<TDestination>>> extractor,
            params ICascade<TDestination>[] cascades)
            : this(extractor, (string) null, cascades)
        {
        }

        public Cascade(Expression<Func<TSource, IEnumerable<TDestination>>> extractor,
            Expression<Func<TDestination, object>> propertyExpression, params ICascade<TDestination>[] cascades)
            : this(extractor, ExpressHelper.GetPath(propertyExpression), cascades)
        {
        }

        private Cascade(Expression<Func<TSource, IEnumerable<TDestination>>> extractor,
            string propertyName, params ICascade<TDestination>[] cascades)
        {
            _extractor = extractor;
            _propertyName = propertyName;
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
            var copy = GetCopyPk(contextAdapter);
            foreach (var parent in s)
            {
                foreach (var child in extractor(parent))
                {
                    copy.Invoke(parent, child);
                }
            }

            await contextAdapter.BulkInsertCascade(s.SelectMany(extractor).Distinct().ToArray(), _cascades);
        }

        public bool IsCascadeBeforeInsert => false;

        private Action<TSource, TDestination> GetCopyPk(IContextAdapter contextAdapter)
        {
            var expression = ExpressHelper.CreateCopy<TSource, TDestination>(contextAdapter.GetPk<TSource>().PropertyName, 
                contextAdapter.GetForwardKeyProperty<TDestination>(_propertyName, typeof(TSource)));
            return expression.Compile();
        }

        public override string ToString()
        {
            return $"{nameof(_extractor)}: {ExpressHelper.GetPath(_extractor)}, {nameof(_propertyName)}: {_propertyName}, Cascades Count {_cascades?.Length}";
        }
    }
}