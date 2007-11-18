namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 7.
	/// </summary>
	/// <remarks>
	/// There have been no test run with this because the NHibernate team does not
	/// have a machine with Sql 7 installed on it.  But there have been users using
	/// Ms Sql 7 with NHibernate.  As issues with Ms Sql 7 and NHibernate become known
	/// this Dialect will be updated.
	/// </remarks>
	public class MsSql7Dialect : MsSql2000Dialect
	{
		/// <summary>
		/// Uses @@identity to get the Id value.
		/// </summary>
		/// <remarks>
		/// There is a well known problem with @@identity and triggers that insert into
		/// rows into other tables that also use an identity column.  The only way I know
		/// of to get around this problem is to upgrade your database server to Ms Sql 2000.
		/// </remarks>
		public override string IdentitySelectString
		{
			get { return "select @@identity"; }
		}
	}
}