namespace NHibernate.Test.TestDialects
{
	public class HanaColumnStoreTestDialect : HanaTestDialectBase
	{
		public HanaColumnStoreTestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// When the target table is a column store, HANA 2 SP3 with its .Net data provider fails to process DML of
		/// a batch of rows when some of them depends on other rows of the batch.
		/// </remarks>
		public override bool SupportsBatchingDependentDML => false;
	}
}
