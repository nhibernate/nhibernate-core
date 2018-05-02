namespace NHibernate.Test.TestDialects
{
	public class HanaRowStoreTestDialect : HanaTestDialectBase
	{
		public HanaRowStoreTestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsSelectForUpdateOnOuterJoin => false;
	}
}
