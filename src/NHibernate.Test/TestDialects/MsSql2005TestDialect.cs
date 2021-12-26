namespace NHibernate.Test.TestDialects
{
	public class MsSql2005TestDialect : TestDialect
	{
		public MsSql2005TestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		public override bool SupportsTime => false;

		/// <summary>
		/// Does not support SELECT FOR UPDATE with paging
		/// </summary>
		public override bool SupportsSelectForUpdateWithPaging => false;
	}
}
