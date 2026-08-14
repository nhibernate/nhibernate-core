using System.Data;

namespace NHibernate.Dialect
{
	public class MySQL8Dialect : MySQL57Dialect
	{
		public MySQL8Dialect() => RegisterColumnType(DbType.Boolean, "BOOLEAN");

		/// <summary>
		/// MySQL supports no-wait locks since 8.0.1.
		/// </summary>
		public override string ForUpdateNowaitString => " for update nowait";

		/// <summary>
		/// MySQL locks only the named tables since 8.0.1. It names tables, not columns.
		/// </summary>
		public override bool SupportsForUpdateOf => true;

		public override string GetForUpdateString(string aliases) => ForUpdateString + " of " + aliases;

		public override string GetForUpdateNowaitString(string aliases) => ForUpdateString + " of " + aliases + " nowait";
	}
}
