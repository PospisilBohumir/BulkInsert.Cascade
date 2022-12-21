using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BulkInsert.Cascade.EfCore
{
    public interface IContextAdapter
    {
        PropertyDescription GetPk<T>();
        string GetForwardKeyProperty<TDestination>(string propertyName, Type type);
        string GetBackwardKeyProperty<T>(string path);
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
        public Type SqlType { get; set; }
        public bool IsIdentity { get; set; }
        public Func<object,object> ValueTransform { get; set; }
    }
}