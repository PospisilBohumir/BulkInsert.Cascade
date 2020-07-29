using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Metadata;
using EntityFramework.Metadata.Extensions;
using Microsoft.SqlServer.Types;

namespace BulkInsert.Cascade.Ef6
{
    internal class ContextAdapter : IContextAdapter
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
            var pks = _context.Db<T>().Pks.ToArray();
            if (pks.Length != 1)
            {
                throw new BulkInsertException($"Only single key entities are supported in object '{typeof(T).Name}' were found {pks.Length} keys");
            }
            return ToDescription(pks[0]);
        }

        private static PropertyDescription ToDescription(IPropertyMap map) => new PropertyDescription
        {
            PropertyName = map.PropertyName,
            Type = map.Type,
            SqlType = map.Type == typeof(DbGeography) ? typeof(SqlGeography) : map.Type,
            IsIdentity = map.IsIdentity,
            ColumnName = map.ColumnName,
            IsDiscriminator = map.IsDiscriminator,
            ValueTransform = map.Type != typeof(DbGeography)
                ? (Func<object, object>) (o => o)
                : o => SqlGeography.Parse(((DbGeography) o).AsText())
        };

        public string GetForwardKeyProperty<TDestination>(string propertyName, Type type)
        {
            var propertyMaps = _context.Db<TDestination>().Properties;
            var properties = (propertyName == null
                ? propertyMaps.Where(o => o.NavigationProperty?.Type == type)
                : propertyMaps.Where(o => o.PropertyName == propertyName)).ToArray();
            if (properties.Length == 1)
            {
                return properties[0].PropertyName;
            }
            var error =
                $"Cannot find in Entity {typeof(TDestination).Name} foreign key property for type {type.Name}" +
                (propertyName == null ? "" : $" and property name '{propertyName}'");
            throw new BulkInsertException(error);
        }

        public string GetBackwardKeyProperty<T>(string path)
        {
            var properties = _context.Db<T>().Properties.Where(o => o.NavigationProperty?.PropertyName == path).ToArray();
            if (properties.Length != 1)
            {
                throw new BulkInsertException($"Cannot find in Entity {typeof(T).Name} find foreign key property");
            }
            return properties[0].PropertyName;
        }

        public IEnumerable<PropertyDescription> GetProperties<T>()
        {
            var propertyMaps = _context.Db<T>().Properties.Where(o => !o.IsNavigationProperty).ToArray();
            return propertyMaps.Select(ToDescription);
        }

        public string GetTableName<T>() => _context.Db<T>().TableName;
        public async Task<T> RunScalar<T>(string sql) => await _context.Database.SqlQuery<T>(sql).FirstAsync();
        public object GetDiscriminatorValue(Type type) => type.Name;
        public SqlTransaction GeTransaction() => (SqlTransaction) _transaction.UnderlyingTransaction;
    }
}