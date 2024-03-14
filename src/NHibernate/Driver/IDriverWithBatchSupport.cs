using System.Data.Common;

namespace NHibernate.Driver
{
#if NET6_0_OR_GREATER
	//TODO: Include in IDriver for NET6_0_OR_GREATER
	internal interface IDriverWithBatchSupport : IDriver
	{
		public DbBatch CreateBatch();
		public bool CanCreateBatch{ get; }

		public void AdjustBatch(DbBatch dbBatch);
		public void PrepareBatch(DbBatch dbBatch);
	}
#endif
}
