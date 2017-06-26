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
	}
}
