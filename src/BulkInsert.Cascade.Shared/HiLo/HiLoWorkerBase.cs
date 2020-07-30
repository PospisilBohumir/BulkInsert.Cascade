using System;
using System.Threading.Tasks;
using BulkInsert.Cascade.Shared.Helpers;

namespace BulkInsert.Cascade.Shared.HiLo
{
    internal abstract class HiLoWorkerBase<TValue> : IHiLoWorker
    {
        private readonly string _sqlDataType;

        protected HiLoWorkerBase(string sqlDataType)
        {
            _sqlDataType = sqlDataType;
        }

        protected string RetrieveIdsSql(string tableName, int entitiesLength) => $@"
declare
     @Maxid {_sqlDataType};
begin
	select @Maxid=IDENT_CURRENT ('{tableName}');
	set @Maxid =@Maxid+{entitiesLength};
	DBCC CHECKIDENT ('{tableName}',Reseed,@Maxid);
	select @Maxid;
end;
";

        protected abstract TValue AddOne(TValue value);
        protected abstract TValue Minus(TValue left,int right);

        public Type PropertyType => typeof(TValue);

        public async Task RetrieveIdsLong<TEntity>(IContextAdapter context, TEntity[] entities)
        {
            var tableName = context.GetTableName<TEntity>();

            var entitiesLength = entities.Length;
            var minId = Minus(await context.RunScalar<TValue>(RetrieveIdsSql(tableName, entitiesLength)), entitiesLength);
            var pkSetter = ExpressHelper.GetPropSetter<TEntity, TValue>(context.GetPk<TEntity>().PropertyName).Compile();
            var id = minId;
            foreach (var entity in entities)
            {
                id = AddOne(id);
                pkSetter(entity, id);
            }
        }
    }
}