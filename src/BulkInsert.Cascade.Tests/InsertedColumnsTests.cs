using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Ef6;
using BulkInsert.Cascade.Shared;
using BulkInsert.Cascade.Tests.TestContext;
using FluentAssertions;
using Xunit;

namespace BulkInsert.Cascade.Tests
{
    public class InsertedColumnsTests : TestBase
    {
        [Fact]
        public async Task AllDefaultsTest()
        {
            await DefaultTest(new AllColumnTypesEntity());
        }

        [Fact]
        public async Task AllFilledOutTest()
        {
            await DefaultTest(new AllColumnTypesEntity
            {
                BoolNullableValue = true,
                BoolValue = true,
                ByteArrayValue = Guid.NewGuid().ToByteArray(),
                DateTimeNullableValue = DateTime.Today,
                DateTimeValue = DateTime.Today,
                DecimalNullableValue = 3.14m,
                DecimalValue = 3.14m,
                EnumNullableValue = TestEnum.Item2,
                EnumValue = TestEnum.Item1,
                IntNullableValue = 42,
                IntValue = 666,
                LongNullableValue = 65536,
                LongValue = 151,
                StringValue = Guid.NewGuid().ToString(),
                //Location = DbGeography.FromText("POINT(-122.336106 47.605049)"),
            });
        }

        [Fact]
        public async Task DbGeographyTest()
        {
            Func<Task> geography = async () =>
            {
                await Context.BulkInsertWithIdGeneration(new[] { new DbGeographyEntity()  });
            };
            await geography.Should().ThrowAsync<BulkInsertException>();
        }
    }
}