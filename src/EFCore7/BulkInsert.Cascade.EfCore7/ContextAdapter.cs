using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace BulkInsert.Cascade.EfCore
{
    public class ContextAdapter : IContextAdapter
    {
        private readonly DbContext _context;
        private readonly IDbContextTransaction _transaction;

        public ContextAdapter(DbContext context, IDbContextTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public PropertyDescription GetPk<T>() => _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
            .Select(GetPropDescription).Single();

        public string GetForwardKeyProperty<TDestination>(string propertyName, Type type)
        {
            return string.IsNullOrWhiteSpace(propertyName)
                ? _context.Model
                    .FindEntityType(typeof(TDestination))
                    .GetProperties()
                    .Where(o => o.IsForeignKey())
                    .Where(o => o.GetContainingForeignKeys().Select(x => x.PrincipalEntityType.ClrType).Contains(type))
                    .Select(o => o.PropertyInfo.Name)
                    .Single()
                : propertyName;
        }

        public string GetBackwardKeyProperty<T>(string path)
        {
            var destinationType = typeof(T);
            var type = destinationType.GetProperty(path)!.PropertyType;
            return _context.Model.FindEntityType(destinationType)
                .GetProperties()
                .Where(o => o.IsForeignKey())
                .Where(o => o.GetContainingForeignKeys().Select(x => x.PrincipalEntityType.ClrType).Contains(type))
                .Select(o => o.PropertyInfo.Name)
                .Single();
        }

        public string GetTableName<T>() => _context.Model.FindEntityType(typeof(T)).GetTableName();

        public IEnumerable<PropertyDescription> GetProperties<T>() 
            => GetProperties(_context.Model.FindEntityType(typeof(T)),string.Empty,o=> true);
        
        private IEnumerable<PropertyDescription> GetProperties(IEntityType entityType,string prefix,Func<IProperty,bool> filter) =>
            entityType.GetProperties()
                .Where(filter)
                .Select(o => GetPropDescription(o,prefix))
                .Concat(GetOwnTypes(entityType, prefix));

        private IEnumerable<PropertyDescription> GetOwnTypes(IEntityType entityType,string prefix) =>
            entityType.GetNavigations().Select(a => new
                {
                    TargetType = a.GetTargetType(),
                    a.Name,
                }).Where(o => o.TargetType.IsOwned())
                .SelectMany(o => GetProperties(o.TargetType, $"{prefix}{o.Name}.",
                    p => !p.IsPrimaryKey()));

        static PropertyDescription GetPropDescription(IProperty o) => GetPropDescription(o, "");

        private static PropertyDescription GetPropDescription(IProperty o,string prefix) =>
            new PropertyDescription
            {
                ColumnName = o.GetColumnName(),
                IsDiscriminator = o.Name == o.DeclaringEntityType?.GetDiscriminatorProperty()?.Name,
                Type = o.ClrType,
                PropertyName = $"{prefix}{o.Name}",
                IsIdentity = o.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn,
                SqlType = o.ClrType,
                ValueTransform = x => x,
            };

        public SqlTransaction GeTransaction() => (SqlTransaction) _transaction.GetDbTransaction();

        public async Task<T> RunScalar<T>(string sql)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = GeTransaction();
                await _context.Database.OpenConnectionAsync();
                return (T) await command.ExecuteScalarAsync();
            }
        }

        public object GetDiscriminatorValue(Type type) => _context.Model.FindEntityType(type).GetDiscriminatorValue();
    }
}