namespace NHibernate.Test.TestDialects
{
	public class MsSqlCe40TestDialect : TestDialect
	{
		public MsSqlCe40TestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsDistributedTransactions => false;

		public override bool SupportsConcurrentTransactions => false;

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
	}
}