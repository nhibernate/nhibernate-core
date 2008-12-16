using System.Data;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for Oracle, compatible with Oracle 8.
	/// </summary>
	public class OracleDialect : Oracle9Dialect
	{
		public OracleDialect()
		{
			RegisterColumnType(DbType.DateTime, "DATE");
		}
 
		/// <summary></summary>
		public override JoinFragment CreateOuterJoinFragment()
		{
			return new OracleJoinFragment();
		}

		/// <summary></summary>
		public override CaseFragment CreateCaseFragment()
		{
			return new DecodeCaseFragment(this);
		}
	}
}