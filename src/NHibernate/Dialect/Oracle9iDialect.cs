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
			RegisterColumnType(DbType.DateTime, "TIMESTAMP(4)");
			RegisterColumnType(DbType.Time, "TIMESTAMP(4)");
		}

		public override long TimestampResolutionInTicks
		{
			// matches precision of TIMESTAMP(4)
			get { return 1000L; }
		}

		public override string GetSelectClauseNullString(SqlType sqlType)
		{
			return GetBasicSelectClauseNullString(sqlType);
		}

		public override CaseFragment CreateCaseFragment()
		{
			// Oracle did add support for ANSI CASE statements in 9i
			return new ANSICaseFragment(this);
		}
	}
}