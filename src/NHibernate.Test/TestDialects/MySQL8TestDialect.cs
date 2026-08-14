namespace NHibernate.Test.TestDialects
{
	public class MySQL8TestDialect : MySQL5TestDialect
	{
		public MySQL8TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsNoWaitLock => true;

		public override bool SupportsCorrelatedColumnsInSubselectJoin => true;
	}
}
