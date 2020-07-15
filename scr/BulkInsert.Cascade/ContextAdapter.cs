using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Helpers;
using BulkInsert.Cascade.Shared;
using EntityFramework.Metadata;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade
{
    public class ContextAdapter : IContextAdapter
    {
        private readonly DbContext _context;
        private readonly DbContextTransaction _transaction;

        public ContextAdapter(DbContext dbContext, DbContextTransaction transaction)
        {
            _context = dbContext;
            _transaction = transaction;
        }


        public PropertyDescription GetPk<T>()
        {
            return ToDescription(_context.GetPk<T>());
        }

        private static PropertyDescription ToDescription(IPropertyMap map) => new PropertyDescription
        {
            PropertyName = map.PropertyName,
            Type = map.Type,
            IsIdentity = map.IsIdentity,
            ColumnName = map.ColumnName,
            IsDiscriminator = map.IsDiscriminator
        };

        public string GetNavigationProperty<TDestination>(string propertyName, Type type)
        {
            var propertyMaps = _context.Db<TDestination>().Properties;
            var property = propertyName == null
                ? propertyMaps.Single(o => o.NavigationProperty?.Type == type)
                : propertyMaps.Single(o => o.PropertyName == propertyName);
            return property.PropertyName;
        }

        public IEnumerable<PropertyDescription> GetProperties<T>()
            => _context.Db<T>().Properties.Where(o => !o.IsNavigationProperty).Select(ToDescription);

        public string GetNavigationProperty<T>(string path)
            => _context.Db<T>().Properties.Single(o => o.NavigationProperty?.PropertyName == path).PropertyName;

        public string GetTableName<T>() => _context.Db<T>().TableName;

        public async Task<T> RunScalar<T>(string sql) => await _context.Database.SqlQuery<T>(sql).FirstAsync();

        public SqlTransaction GeTransaction() => (SqlTransaction) _transaction.UnderlyingTransaction;
    }
}