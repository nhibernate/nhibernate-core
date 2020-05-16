namespace NHibernate.Test.TestDialects
{
	public class MySQL5TestDialect : TestDialect
	{
		public MySQL5TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsAggregateInSubSelect => true;
	}
}
