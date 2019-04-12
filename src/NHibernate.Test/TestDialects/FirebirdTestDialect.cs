namespace NHibernate.Test.TestDialects
{
	public class FirebirdTestDialect: TestDialect
	{
		public FirebirdTestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsComplexExpressionInGroupBy => false;
		public override bool SupportsNonDataBoundCondition => false;
		/// <summary>
		/// Non-integer arguments are rounded before the division takes place. So, “7.5 mod 2.5” gives 2 (8 mod 3), not 0.
		/// </summary>
		public override bool SupportsModuloOnDecimal => false;

		/// <summary>
		/// Does not support update locks
		/// </summary>
		public override bool SupportsSelectForUpdate => false;
	}
}
