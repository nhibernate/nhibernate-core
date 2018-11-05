using System;
using System.Data;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an <see cref="DbType.Int64" /> column
	/// that stores the DateTime using the Ticks property. On read, yields an UTC date-time. On
	/// write, the DateTime must already be in UTC.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column, along with <see cref="TicksType" />.
	/// The System.DateTime.Ticks is accurate to 100-nanosecond intervals.
	/// </remarks>
	[Serializable]
	public class UtcTicksType : TicksType
	{
		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Utc;

		/// <inheritdoc />
		public override string Name => "UtcTicks";
	}
}
