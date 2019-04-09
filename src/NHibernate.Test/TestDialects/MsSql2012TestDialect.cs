namespace NHibernate.Test.TestDialects
{
	public class MsSql2012TestDialect : TestDialect
	{
		public MsSql2012TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsAggregateInSubSelect => false;
	}
}
