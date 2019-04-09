namespace NHibernate.Test.TestDialects
{
	public class MsSql2005TestDialect : TestDialect
	{
		public MsSql2005TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsAggregateInSubSelect => false;
	}
}
