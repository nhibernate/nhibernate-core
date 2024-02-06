namespace NHibernate.Dialect
{
	public class MySQL8InnoDBDialect : MySQL8Dialect
	{
		public override bool SupportsCascadeDelete => true;

		public override string TableTypeString => " ENGINE=InnoDB";

		public override bool HasSelfReferentialForeignKeyBug => true;
	}
}
