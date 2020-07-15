using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.Shared
{
    public interface IContextAdapter
    {
        PropertyDescription GetPk<T>();
        string GetNavigationProperty<TDestination>(string propertyName, Type type);
        string GetTableName<T>();
        IEnumerable<PropertyDescription> GetProperties<T>();
        public string GetNavigationProperty<T>(string path);
        SqlTransaction GeTransaction();
        Task<T> RunScalar<T>(string sql);
    }

    public class PropertyDescription
    {
        public string ColumnName { get; set; }
        public bool IsDiscriminator { get; set; }
        public string PropertyName { get; set; }
        public Type Type { get; set; }
        public bool IsIdentity { get; set; }
    }
}