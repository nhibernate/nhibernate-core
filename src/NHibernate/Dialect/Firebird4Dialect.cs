using System.Data;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	public class Firebird4Dialect: Firebird3Dialect
	{
		/// <inheritdoc />
		/// <remarks>
		/// Firebird 4 increases the identifier length limit from 31 to 63 characters.
		/// </remarks>
		public override int MaxAliasLength => 63;

		protected override void RegisterColumnTypes()
		{
			base.RegisterColumnTypes();
			// Firebird 4 raises the maximum precision of exact numeric types from 18 to 38. 29 is the
			// maximum a .Net decimal can hold.
			RegisterColumnType(DbType.Decimal, 29, "DECIMAL($p, $s)");
		}

		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
		}
	}
}
