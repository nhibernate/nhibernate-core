using System;

using NHibernate.SqlCommand;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// An SQL dialect for Oracle, compatible with Oracle 8.
	/// </summary>
	public class OracleDialect : Oracle9Dialect	
	{
		public OracleDialect() : base() 
		{
		}
		
		public override JoinFragment CreateOuterJoinFragment() 
		{
			return new OracleJoinFragment();
		}

		public override CaseFragment CreateCaseFragment() 
		{
			return new DecodeCaseFragment(this);
		}
	}
}
