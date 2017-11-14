using System.Data;
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

		protected override void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP(7)");
			RegisterColumnType(DbType.DateTime, 9, "TIMESTAMP($s)");
			RegisterColumnType(DbType.Time, "TIMESTAMP(7)");
			RegisterColumnType(DbType.Time, 9, "TIMESTAMP($s)");
			RegisterColumnType(DbType.Xml, "XMLTYPE");
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
	}
}
