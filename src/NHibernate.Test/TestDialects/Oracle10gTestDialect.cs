namespace NHibernate.Test.TestDialects
{
	public class Oracle10gTestDialect : TestDialect
	{
		public Oracle10gTestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		/// <summary>
		/// Does not support SELECT FOR UPDATE with paging
		/// </summary>
		public override bool SupportsSelectForUpdateWithPaging => false;
	}
}
