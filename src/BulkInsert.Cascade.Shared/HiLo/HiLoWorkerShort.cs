namespace BulkInsert.Cascade.Shared.HiLo
{
    internal class HiLoWorkerShort : HiLoWorkerBase<short>
    {
        public HiLoWorkerShort() : base("smallint")
        {
        }

        protected override short AddOne(short value) => (short)(value + 1);

        protected override short Minus(short left, int right) => (short)(left - right);
    }
}