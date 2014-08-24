using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.Driver;

namespace NHibernate.Dialect
{
	public class MsSql2008Dialect : MsSql2005Dialect
	{
		protected override void RegisterDateTimeTypeMappings()
		{
			base.RegisterDateTimeTypeMappings();
			RegisterColumnType(DbType.DateTime2, "DATETIME2");
			RegisterColumnType(DbType.DateTimeOffset, "DATETIMEOFFSET");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.Time, "TIME");
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("current_timestamp", new NoArgSQLFunction("sysdatetime", NHibernateUtil.DateTime2, true));
			RegisterFunction("current_timestamp_offset", new NoArgSQLFunction("sysdatetimeoffset", NHibernateUtil.DateTimeOffset, true));
		}

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
			RegisterKeyword("date");
			RegisterKeyword("datetimeoffset");
			RegisterKeyword("datetime2");
			RegisterKeyword("time");
			RegisterKeyword("hierarchyid");
		}

		protected override void RegisterDefaultProperties()
		{
			base.RegisterDefaultProperties();
			DefaultProperties[Environment.ConnectionDriver] = typeof(Sql2008ClientDriver).AssemblyQualifiedName;
		}
	}
}