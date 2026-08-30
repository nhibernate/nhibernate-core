using System.Data;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for Firebird 4 and above.
	/// </summary>
	public class Firebird4Dialect : Firebird3Dialect
	{
		/// <inheritdoc />
		/// <remarks>
		/// <c>CURRENT_TIMESTAMP</c> is time zone aware since Firebird 4, <c>LOCALTIMESTAMP</c> is not.
		/// </remarks>
		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		/// <inheritdoc />
		public override string CurrentTimestampSQLFunctionName => "localtimestamp";

		protected override void RegisterColumnTypes()
		{
			base.RegisterColumnTypes();
			// Firebird 4 raises the maximum precision of exact numeric types from 18 to 38. 29 is the
			// maximum a .Net decimal can hold.
			RegisterColumnType(DbType.Decimal, 29, "DECIMAL($p, $s)");
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("localtimestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
		}
	}
}
