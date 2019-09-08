namespace NHibernate.Test.TestDialects
{
	public class PostgreSQL83TestDialect : TestDialect
	{
        public PostgreSQL83TestDialect(Dialect.Dialect dialect)
            : base(dialect)
        {
        }

        public override bool SupportsSelectForUpdateOnOuterJoin
        {
            get { return false; }
        }

        public override bool SupportsNullCharactersInUtfStrings
        {
            get { return false; }
        }

		/// <summary>
		/// Npgsql does not clone the transaction in its context, and uses it in its prepare phase. When that was a
		/// dependent transaction, it is then usually already disposed of, causing Npgsql to crash.
		/// </summary>
		public override bool SupportsDependentTransaction => false;
	}
}
