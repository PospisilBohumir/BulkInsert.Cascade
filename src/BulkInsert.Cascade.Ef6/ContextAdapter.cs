using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Shared;
using EntityFramework.Metadata;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade.Ef6
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
            return ToDescription(_context.Db<T>().Pks.Single());
        }

        private static PropertyDescription ToDescription(IPropertyMap map) => new PropertyDescription
        {
            PropertyName = map.PropertyName,
            Type = map.Type,
            IsIdentity = map.IsIdentity,
            ColumnName = map.ColumnName,
            IsDiscriminator = map.IsDiscriminator,
        };

        public string GetForwardNavigationProperty<TDestination>(string propertyName, Type type)
        {
            var propertyMaps = _context.Db<TDestination>().Properties;
            var property = propertyName == null
                ? propertyMaps.Single(o => o.NavigationProperty?.Type == type)
                : propertyMaps.Single(o => o.PropertyName == propertyName);
            return property.PropertyName;
        }
        public string GetBackwardNavigationProperty<T>(string path)
            => _context.Db<T>().Properties.Single(o => o.NavigationProperty?.PropertyName == path).PropertyName;


        public IEnumerable<PropertyDescription> GetProperties<T>()
            => _context.Db<T>().Properties.Where(o => !o.IsNavigationProperty).Select(ToDescription);


        public string GetTableName<T>() => _context.Db<T>().TableName;

        public async Task<T> RunScalar<T>(string sql) => await _context.Database.SqlQuery<T>(sql).FirstAsync();
        public object GetDiscriminatorValue(Type type) => type.Name;

        public SqlTransaction GeTransaction() => (SqlTransaction) _transaction.UnderlyingTransaction;
    }
}