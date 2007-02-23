using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for Oracle, compatible with Oracle 8.
	/// </summary>
	public class OracleDialect : Oracle9Dialect
	{
		/// <summary></summary>
		public OracleDialect() : base()
		{
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