namespace BulkInsert.Cascade.Shared.HiLo
{
    internal class HiLoWorkerLong : HiLoWorkerBase<long>
    {
        public HiLoWorkerLong() : base("bigint")
        {
        }

        protected override long AddOne(long value) => value + 1;

        protected override long Minus(long left, int right) => left - right;
    }
}