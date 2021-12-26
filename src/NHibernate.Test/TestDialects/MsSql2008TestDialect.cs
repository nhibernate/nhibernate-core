namespace NHibernate.Test.TestDialects
{
	public class MsSql2008TestDialect : MsSql2005TestDialect
	{
		public MsSql2008TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsTime => true;
	}
}
