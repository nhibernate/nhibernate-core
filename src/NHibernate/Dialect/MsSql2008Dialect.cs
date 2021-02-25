using System.Collections.Generic;
using System.Data;
using NHibernate.Dialect.Function;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	public class MsSql2008Dialect : MsSql2005Dialect
	{
		/// <summary>
		/// Should <see cref="DbType.DateTime" /> be preserved instead of switching it to <see cref="DbType.DateTime2" />?
		/// </summary>
		/// <value>
		/// <see langword="true" /> for preserving <see cref="DbType.DateTime" />, <see langword="false" /> for
		/// replacing it with <see cref="DbType.DateTime2" />.
		/// </value>
		protected bool KeepDateTime { get; private set; }

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			KeepDateTime = PropertiesHelper.GetBoolean(Environment.SqlTypesKeepDateTime, settings, false);
			if (KeepDateTime)
			{
				// Re-register functions, they depend on this setting.
				RegisterFunctions();
			}
		}

		protected override void RegisterDateTimeTypeMappings()
		{
			base.RegisterDateTimeTypeMappings();
			// Not overriding default scale: it is already the max, 7.
			RegisterColumnType(DbType.DateTime2, "DATETIME2");
			RegisterColumnType(DbType.DateTime2, 7, "DATETIME2($s)");
			RegisterColumnType(DbType.DateTimeOffset, "DATETIMEOFFSET");
			RegisterColumnType(DbType.DateTimeOffset, 7, "DATETIMEOFFSET($s)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.Time, "TIME");
			RegisterColumnType(DbType.Time, 7, "TIME($s)");
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			if (!KeepDateTime)
			{
				RegisterFunction(
					"current_timestamp",
					new NoArgSQLFunction("sysdatetime", NHibernateUtil.LocalDateTime, true));
				RegisterFunction(
					"current_utctimestamp",
					new NoArgSQLFunction("sysutcdatetime", NHibernateUtil.UtcDateTime, true));
			}

			RegisterFunction("current_date", new SQLFunctionTemplate(NHibernateUtil.LocalDate, "cast(getdate() as date)"));
			RegisterFunction(
				"current_timestamp_offset",
				new NoArgSQLFunction("sysdatetimeoffset", NHibernateUtil.DateTimeOffset, true));
			RegisterFunction(
				"current_utctimestamp_offset",
				new SQLFunctionTemplate(NHibernateUtil.DateTimeOffset, "todatetimeoffset(sysutcdatetime(), 0)"));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "cast(?1 as date)"));
		}

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
			RegisterKeyword("datetimeoffset");
			RegisterKeyword("datetime2");
			RegisterKeyword("hierarchyid");
		}

		protected override void RegisterDefaultProperties()
		{
			base.RegisterDefaultProperties();
			DefaultProperties[Environment.ConnectionDriver] = typeof(Sql2008ClientDriver).AssemblyQualifiedName;
		}

		public override string CurrentTimestampSQLFunctionName =>
			KeepDateTime ? base.CurrentTimestampSQLFunctionName : "SYSDATETIME()";

		/// <inheritdoc />
		public override string CurrentUtcTimestampSQLFunctionName =>
			KeepDateTime ? base.CurrentUtcTimestampSQLFunctionName : "SYSUTCDATETIME()";

		public override long TimestampResolutionInTicks =>
			KeepDateTime
				? base.TimestampResolutionInTicks
				// MS SQL resolution with datetime2 is 100ns, one tick.
				: 1;

		/// <inheritdoc />
		public override SqlType OverrideSqlType(SqlType type)
		{
			type = base.OverrideSqlType(type);
			return !KeepDateTime && type is DateTimeSqlType dateTimeType
				? dateTimeType.ScaleDefined
					? SqlTypeFactory.GetDateTime2(dateTimeType.Scale)
					: SqlTypeFactory.DateTime2
				: type;
		}

		/// <inheritdoc />
		public override bool SupportsDateTimeScale => !KeepDateTime;
	}
}
