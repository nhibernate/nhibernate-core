using NHibernate.Cfg;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for DB2 on iSeries OS/400.
	/// </summary>
	/// <remarks>
	/// The DB2400Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.DB2400Driver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class DB2400Dialect : DB2Dialect
	{
		public DB2400Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.DB2400Driver";
		}

		public override bool SupportsSequences
		{
			get { return false; }
		}

		public override string GetIdentitySelectString(string identityColumn, string tableName)
		{
			return "select identity_val_local() from sysibm.sysdummy1";
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			return new SqlStringBuilder(querySqlString)
				.Add(" fetch first ")
				.Add(limit.ToString())
				.Add(" rows only ")
				.ToSqlString();
		}

		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}
	}
}