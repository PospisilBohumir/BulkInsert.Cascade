namespace BulkInsert.Cascade.HiLo
{
    internal class HiLoWorkerInt : HiLoWorkerBase<int>
    {
        public HiLoWorkerInt() : base("int")
        {
        }

        protected override int AddOne(int value) => value + 1;

        protected override int Minus(int left, int right) => left - right;
    }
}