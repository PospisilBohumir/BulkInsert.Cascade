using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.Shared
{
    public interface IContextAdapter
    {
        PropertyDescription GetPk<T>();
        string GetForwardNavigationProperty<TDestination>(string propertyName, Type type);
        string GetBackwardNavigationProperty<T>(string path);
        string GetTableName<T>();
        IEnumerable<PropertyDescription> GetProperties<T>();
        SqlTransaction GeTransaction();
        Task<T> RunScalar<T>(string sql);

        object GetDiscriminatorValue(Type type);
    }

    public class PropertyDescription
    {
        public string ColumnName { get; set; }
        public bool IsDiscriminator { get; set; }
        public string PropertyName { get; set; }
        public Type Type { get; set; }
        public bool IsIdentity { get; set; }
        public Func<object, object> DataTransform { get; set; } = o => o;
    }
}