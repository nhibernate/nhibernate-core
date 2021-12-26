namespace NHibernate.Test.TestDialects
{
	public class MsSqlCe40TestDialect : TestDialect
	{
		public MsSqlCe40TestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsTime => false;

		public override bool SupportsFullJoin => false;

		public override bool SupportsComplexExpressionInGroupBy => false;

		public override bool SupportsCountDistinct => false;

		/// <summary>
		/// Error 25520: Expressions in the ORDER BY list cannot contain aggregate functions.
		/// </summary>
		public override bool SupportsOrderByAggregate => false;

		/// <summary>
		/// Error 25547: ORDER BY &lt;column number&gt; not supported.
		/// </summary>
		public override bool SupportsOrderByColumnNumber => false;

		public override bool SupportsDuplicatedColumnAliases => false;

		public override bool SupportsEmptyInserts => false;

		/// <summary>
		/// Modulo is not supported on real, float, money, and numeric data types. [ Data type = numeric ]
		/// </summary>
		public override bool SupportsModuloOnDecimal => false;

		/// <summary>
		/// Does not support update locks
		/// </summary>
		public override bool SupportsSelectForUpdate => false;
	}
}
