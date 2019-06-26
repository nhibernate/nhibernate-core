namespace NHibernate.Test.TestDialects
{
	public class PostgreSQL83TestDialect : TestDialect
	{
		public PostgreSQL83TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		public override bool SupportsSelectForUpdateOnOuterJoin => false;

		public override bool SupportsNullCharactersInUtfStrings => false;

		/// <summary>
		/// Npgsql since its 3.2.5 version fails some tests requiring this feature. The trouble was not occuring with
		/// Npgsql 3.2.4.1.
		/// </summary>
		public override bool SupportsUsingConnectionOnSystemTransactionPrepare => false;

		public override bool SupportsAggregateInSubSelect => true;
	}
}
