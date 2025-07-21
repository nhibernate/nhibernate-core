using System.Data;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 9.5 and above.
	/// </summary>
	/// <remarks>
	/// PostgreSQL 9.5 supports SKIP LOCKED
	/// </remarks>
	public class PostgreSQL95Dialect : PostgreSQL83Dialect
	{
		// https://www.postgresql.org/docs/9.5/sql-select.html
		public override string ForUpdateSkipLockedString => " for update skip locked";

		public override string GetForUpdateSkipLockedString(string aliases)
		{
			return $" for update of {aliases} skip locked";
		}
	}
}
