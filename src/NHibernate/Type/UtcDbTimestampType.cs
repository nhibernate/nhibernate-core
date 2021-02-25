using System;

namespace NHibernate.Type
{
	/// <summary>
	/// When used as a version, gets seeded and incremented by querying the database's
	/// current UTC timestamp, rather than the application host's current timestamp.
	/// </summary>
	[Serializable]
	public class UtcDbTimestampType : DbTimestampType
	{
		/// <inheritdoc />
		public override string Name => "UtcDbTimestamp";

		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Utc;

		protected override bool SupportsCurrentTimestampSelection(Dialect.Dialect dialect)
		{
			return dialect.SupportsCurrentUtcTimestampSelection;
		}

		protected override string GetCurrentTimestampSelectString(Dialect.Dialect dialect)
		{
			return dialect.CurrentUtcTimestampSelectString;
		}
	}
}
