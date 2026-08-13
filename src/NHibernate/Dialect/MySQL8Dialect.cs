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

		public override string GetForUpdateNowaitString(string aliases) => ForUpdateNowaitString;
	}
}
