namespace NHibernate.Test.TestDialects
{
	public class MsSql2008TestDialect : TestDialect
	{
		public MsSql2008TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		/// <summary>
		/// Does not support SELECT FOR UPDATE with paging
		/// </summary>
		public override bool SupportsSelectForUpdateWithPaging => false;
	}
}
