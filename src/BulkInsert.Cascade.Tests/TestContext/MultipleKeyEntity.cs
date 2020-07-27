using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkInsert.Cascade.Tests.TestContext
{
    public class MultipleKeyEntity
    {
        [Key, Column(Order = 0)]
        public string SomeId { get; set; }

        [Key, Column(Order = 1)]
        public int OtherId { get; set; }
    }
}