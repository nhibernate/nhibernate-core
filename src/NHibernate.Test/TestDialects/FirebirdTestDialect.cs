namespace NHibernate.Test.TestDialects
{
	public class FirebirdTestDialect: TestDialect
	{
		public FirebirdTestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsComplexExpressionInGroupBy => false;
		public override bool SupportsNonDataBoundCondition => false;
	}
}
