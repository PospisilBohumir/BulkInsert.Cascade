using System;

namespace BulkInsert.Cascade.Tests.EfCore7.TestContext;

public class AllColumnTypesEntity : IId
{
    public long Id { get; set; }
        
    public string StringValue { get; set; }
    public DateTime DateTimeValue { get; set; } = new DateTime(1800,1,1);
    public DateTime? DateTimeNullableValue { get; set; }
    public bool BoolValue { get; set; }
    public bool? BoolNullableValue { get; set; }
    public decimal DecimalValue { get; set; }
    public decimal? DecimalNullableValue { get; set; }
    public int IntValue { get; set; }
    public int? IntNullableValue { get; set; }
    public long LongValue { get; set; }
    public long? LongNullableValue { get; set; }
        
    public byte[] ByteArrayValue { get; set; }
        
    public TestEnum EnumValue { get; set; }
    public TestEnum? EnumNullableValue { get; set; }
}

public enum TestEnum
{
    Item1 = 3,
    Item2 = 18
}