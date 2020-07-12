using System;
using System.Data.Entity;
using System.Threading.Tasks;
using BulkInsert.Cascade.Helpers;
using EntityFramework.Metadata.Extensions;

namespace BulkInsert.Cascade.HiLo
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

        public async Task RetrieveIdsLong<TEntity>(DbContext context, TEntity[] entities)
        {
            var tableName = context.Db<TEntity>().TableName;

            var entitiesLength = entities.Length;
            var minId = Minus(await context.Database.SqlQuery<TValue>(RetrieveIdsSql(tableName, entitiesLength)).FirstAsync(), entitiesLength);
            var pkSetter = context.GetPkSetter<TEntity, TValue>();
            foreach (var entity in entities)
            {
                pkSetter(entity, AddOne(minId));
            }
        }
    }
}