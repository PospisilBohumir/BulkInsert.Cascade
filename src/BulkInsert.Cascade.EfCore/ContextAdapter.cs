using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BulkInsert.Cascade.EfCore
{
    public class ContextAdapter : IContextAdapter
    {
        private readonly DbContext _context;
        private readonly DbTransaction _transaction;

        public ContextAdapter(DbContext context, DbTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public PropertyDescription GetPk<T>() => _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
            .Select(GetPropDescription).Single();

        public string GetForwardNavigationProperty<TDestination>(string propertyName, Type type)
        {
            throw new NotImplementedException();
        }

        public string GetBackwardNavigationProperty<T>(string path)
        {
            //_context.Db<T>().Properties.Single(o => o.NavigationProperty?.PropertyName == path).PropertyName;
            //            return _context.Model.FindEntityType(typeof(T)).GetNavigations().Single(o => o.Name == path).Name;
            throw new NotImplementedException();
        }

        public string GetTableName<T>() => _context.Model.FindEntityType(typeof(T)).GetTableName();

        public IEnumerable<PropertyDescription> GetProperties<T>() => _context.Model.FindEntityType(typeof(T)).GetProperties()
            .Select(GetPropDescription);

        private static PropertyDescription GetPropDescription(IProperty o)
        {
            return new PropertyDescription
            {
                ColumnName = o.GetColumnName(),
                IsDiscriminator = o.Name == o.DeclaringEntityType.GetDiscriminatorProperty().Name,
                Type = o.ClrType,
                PropertyName = o.Name,
                IsIdentity = o.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn
            };
        }

        public SqlTransaction GeTransaction() => (SqlTransaction) _transaction;

        public async Task<T> RunScalar<T>(string sql)
        {
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                await _context.Database.OpenConnectionAsync();
                return (T) await command.ExecuteScalarAsync();
            }
        }

        public object GetDiscriminatorValue(Type type) => _context.Model.FindEntityType(type).GetDiscriminatorValue();
    }
}