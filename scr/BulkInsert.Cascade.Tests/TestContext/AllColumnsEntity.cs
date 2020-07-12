using System;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class AllColumnsEntity
    {
        public long Id { get; set; }
        public string StringValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public DateTime? DateTimeNullableValue { get; set; }
        public bool BoolValue { get; set; }
        public bool? BoolNullableValue { get; set; }

        public int IntValue { get; set; }
        public int? IntNullableValue { get; set; }

        public long LongValue { get; set; }
        public long? LongNullableValue { get; set; }

        public byte[] ByteArrayValue { get; set; }
    }
}