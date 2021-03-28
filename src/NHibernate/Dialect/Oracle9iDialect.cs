using System.Data;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect
{
	public class Oracle9iDialect : Oracle8iDialect
	{
		public override string CurrentTimestampSelectString
		{
			get { return "select systimestamp from dual"; }
		}

		public override string CurrentTimestampSQLFunctionName
		{
			get
			{
				// the standard SQL function name is current_timestamp...
				return "current_timestamp";
			}
		}

		// Current_timestamp is a timestamp with time zone, so it can always be converted back to UTC.
		/// <inheritdoc />
		public override string CurrentUtcTimestampSQLFunctionName => "SYS_EXTRACT_UTC(current_timestamp)";

		/// <inheritdoc />
		public override string CurrentUtcTimestampSelectString =>
			$"select {CurrentUtcTimestampSQLFunctionName} from dual";

		/// <inheritdoc />
		public override bool SupportsCurrentUtcTimestampSelection => true;

		protected override void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP(7)");
			RegisterColumnType(DbType.DateTime, 9, "TIMESTAMP($s)");
			RegisterColumnType(DbType.Time, "TIMESTAMP(7)");
			RegisterColumnType(DbType.Time, 9, "TIMESTAMP($s)");
			RegisterColumnType(DbType.Xml, "XMLTYPE");
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();

			RegisterFunction(
				"current_utctimestamp",
				new SQLFunctionTemplate(NHibernateUtil.UtcDateTime, "SYS_EXTRACT_UTC(current_timestamp)"));
		}

		public override long TimestampResolutionInTicks => 1;

		public override string GetSelectClauseNullString(SqlType sqlType)
		{
			return GetBasicSelectClauseNullString(sqlType);
		}

		public override CaseFragment CreateCaseFragment()
		{
			// Oracle did add support for ANSI CASE statements in 9i
			return new ANSICaseFragment(this);
		}

		/// <inheritdoc />
		public override bool SupportsDateTimeScale => true;

		/// <inheritdoc />
		public override bool SupportsRowValueConstructorSyntaxInInList => true;
	}
}
