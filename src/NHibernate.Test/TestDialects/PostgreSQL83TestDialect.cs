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

		/// <summary>
		/// Npgsql does not clone the transaction in its context, and uses it in its prepare phase. When that was a
		/// dependent transaction, it is then usually already disposed of, causing Npgsql to crash.
		/// </summary>
		public override bool SupportsDependentTransaction => false;

		public override bool SupportsAggregateInSubSelect => true;

		/// <inheritdoc />
		public override bool SupportsRowValueConstructorSyntax => true;
	}
}
