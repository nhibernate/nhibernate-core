using System.Data.Common;
using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	public partial class SapSQLAnywhere17Driver : ReflectionBasedDriver
	{
		public SapSQLAnywhere17Driver()
			: base("Sap.Data.SQLAnywhere", "Sap.Data.SQLAnywhere.v4.5", "Sap.Data.SQLAnywhere.SAConnection", "Sap.Data.SQLAnywhere.SACommand")
		{
		}

		public override bool UseNamedPrefixInSql => true;

		public override bool UseNamedPrefixInParameter => true;

		public override string NamedPrefix => ":";

		public override bool RequiresTimeSpanForTime => true;

		public override DbDataReader ExecuteReader(DbCommand command)
		{
			var reader = command.ExecuteReader();

			return new SqlAnywhereDbDataReader(reader);
		}
	}
}
