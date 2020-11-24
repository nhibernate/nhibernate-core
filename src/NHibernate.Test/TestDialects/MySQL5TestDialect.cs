namespace NHibernate.Test.TestDialects
{
	public class MySQL5TestDialect : TestDialect
	{
		public MySQL5TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsAggregateInSubSelect => true;

		/// <summary>
		/// In MySQL, you can't modify the same table which you use in the SELECT part.
		/// This behaviour is documented at: http://dev.mysql.com/doc/refman/5.6/en/update.html
		/// </summary>
		public override bool SupportsModifyAndSelectSameTable => false;
	}
}
