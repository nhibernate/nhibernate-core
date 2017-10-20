using System.Data;

namespace NHibernate.Dialect
{
	public class MySQL57Dialect : MySQL55Dialect
	{
		public MySQL57Dialect()
		{
			RegisterColumnType(DbType.DateTime, "DATETIME(6)");
			RegisterColumnType(DbType.DateTime, 6, "DATETIME($s)");
			RegisterColumnType(DbType.Time, "TIME(6)");
			RegisterColumnType(DbType.Time, 6, "TIME($s)");
		}

		public override long TimestampResolutionInTicks => 10;

		/// <inheritdoc />
		public override bool SupportsDateTimeScale => true;
	}
}
