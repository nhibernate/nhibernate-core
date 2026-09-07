using System.Data;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for Firebird 4 and above.
	/// </summary>
	public class Firebird4Dialect : Firebird3Dialect
	{
		private const string UtcTimestampExpression = "cast(CURRENT_TIMESTAMP at time zone 'UTC' as timestamp)";

		/// <inheritdoc />
		/// <remarks>
		/// <c>CURRENT_TIMESTAMP</c> is time zone aware since Firebird 4, <c>LOCALTIMESTAMP</c> is not.
		/// </remarks>
		public override string CurrentTimestampSelectString => "select LOCALTIMESTAMP from RDB$DATABASE";

		/// <inheritdoc />
		public override string CurrentTimestampSQLFunctionName => "localtimestamp";

		/// <inheritdoc />
		public override string CurrentUtcTimestampSelectString =>
			"select " + CurrentUtcTimestampSQLFunctionName + " from RDB$DATABASE";

		/// <inheritdoc />
		/// <remarks>
		/// Firebird has no UTC function. Move the time zone aware <c>CURRENT_TIMESTAMP</c> to UTC and
		/// remove the time zone.
		/// </remarks>
		public override string CurrentUtcTimestampSQLFunctionName => UtcTimestampExpression;

		/// <inheritdoc />
		public override bool SupportsCurrentUtcTimestampSelection => true;

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

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("current_utctimestamp", new SQLFunctionTemplate(NHibernateUtil.UtcDateTime, UtcTimestampExpression));
			RegisterFunction("localtimestamp", new NoArgSQLFunction("localtimestamp", NHibernateUtil.LocalDateTime, false));

			RegisterFunction("base64_encode", new StandardSQLFunction("base64_encode", NHibernateUtil.String));
			RegisterFunction("base64_decode", new StandardSQLFunction("base64_decode", NHibernateUtil.Binary));
			RegisterFunction("hex_encode", new StandardSQLFunction("hex_encode", NHibernateUtil.String));
			RegisterFunction("hex_decode", new StandardSQLFunction("hex_decode", NHibernateUtil.Binary));
			RegisterFunction("first_day", new SQLFunctionTemplate(NHibernateUtil.Date, "first_day(of month from ?1)"));
			RegisterFunction("last_day", new SQLFunctionTemplate(NHibernateUtil.Date, "last_day(of month from ?1)"));
		}
	}
}
