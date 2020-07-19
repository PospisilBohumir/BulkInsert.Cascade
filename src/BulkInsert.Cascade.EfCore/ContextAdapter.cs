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

        public ContextAdapter(DbContext context,DbTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public PropertyDescription GetPk<T>() => _context.Model.FindEntityType(typeof(T)).GetKeys()
            .Select(o => GetPropDescription(o.Properties.Single())).Single();

        public string GetNavigationProperty<TDestination>(string propertyName, Type type)
        {
            throw new NotImplementedException();
        }

        public string GetNavigationProperty<T>(string path)
        {
            //_context.Db<T>().Properties.Single(o => o.NavigationProperty?.PropertyName == path).PropertyName;
            //            return _context.Model.FindEntityType(typeof(T)).GetNavigations().Single(o => o.Name == path).Name;
            throw new NotImplementedException();
        }
        
        public string GetTableName<T>() => _context.Model.FindEntityType(typeof(T)).GetTableName();

        public IEnumerable<PropertyDescription> GetProperties<T>() => _context.Model.FindEntityType(typeof(T)).GetProperties()
            //.Where(o => !o.IsNavigationProperty)
            .Select(GetPropDescription);

        private PropertyDescription GetPropDescription(IProperty o)
        {
            return new PropertyDescription
            {
                ColumnName = o.GetColumnName(),
                IsDiscriminator = false, //TODO: figure out how to find it
                Type = o.ClrType,
                PropertyName = o.Name,
                IsIdentity = false, //TODO: figure out how to find it
            };
        }

        public SqlTransaction GeTransaction() => (SqlTransaction)_transaction;
        public async Task<T> RunScalar<T>(string sql)
        {
            await using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                await _context.Database.OpenConnectionAsync();
                return (T) await command.ExecuteScalarAsync();
            }
        }
    }
}
