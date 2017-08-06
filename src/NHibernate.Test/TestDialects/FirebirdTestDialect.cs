namespace NHibernate.Test.TestDialects
{
	public class FirebirdTestDialect: TestDialect
	{
		public FirebirdTestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsDistributedTransactions => false;

		public override bool SupportsComplexExpressionInGroupBy => false;
	}
}