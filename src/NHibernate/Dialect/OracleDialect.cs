using System;

using NHibernate.Sql;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect for Oracle, compatible with Oracle 8.
	/// </summary>
	public class OracleDialect : Oracle9Dialect	{

		public OracleDialect() : base() {
		}
		
		public OuterJoinFragment CreateOuterJoinFragment() {
			return new OracleOuterJoinFragment();
		}
		public CaseFragment CreateCaseFragment() {
			return new DecodeCaseFragment();
		}
	}
}