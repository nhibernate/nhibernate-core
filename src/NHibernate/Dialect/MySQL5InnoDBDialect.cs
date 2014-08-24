namespace NHibernate.Dialect
{
	public class MySQL5InnoDBDialect : MySQL5Dialect
	{
		public override bool SupportsCascadeDelete
		{
			get { return true; }
		}

		public override string TableTypeString
		{
			get { return " ENGINE=InnoDB"; }
		}

		public override bool HasSelfReferentialForeignKeyBug
		{
			get { return true; }
		}
	}
}
