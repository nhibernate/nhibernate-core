namespace NHibernate.Test.TestDialects
{
	public abstract class HanaTestDialectBase : TestDialect
	{
		protected HanaTestDialectBase(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsComplexExpressionInGroupBy => false;

		public override bool SupportsEmptyInserts => false;
	}
}
