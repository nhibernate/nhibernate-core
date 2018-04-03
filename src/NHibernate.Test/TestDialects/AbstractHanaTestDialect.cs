namespace NHibernate.Test.TestDialects
{
	public abstract class AbstractHanaTestDialect : TestDialect
	{
        public AbstractHanaTestDialect(Dialect.Dialect dialect)
            : base(dialect)
        {
        }

		public override bool SupportsComplexExpressionInGroupBy => false;

		public override bool SupportsEmptyInserts => false;
	}
}
