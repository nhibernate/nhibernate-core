using System;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 8.1 and above.
	/// </summary>
	/// <remarks>
	/// PostgreSQL 8.1 supports <c>FOR UPDATE ... NOWAIT</c> syntax.
	/// See <see cref="PostgreSQLDialect" /> for more information.
	/// </remarks>
	public class PostgreSQL81Dialect : PostgreSQLDialect
	{
		public override string ForUpdateNowaitString
		{
			get { return "for update nowait"; }
		}

		public override string GetForUpdateNowaitString( string aliases )
		{
			return "for update of " + aliases + " nowait";
		}
	}
}
