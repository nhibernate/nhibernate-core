using System;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> property to a <see cref="DbType.DateTime"/> column that
	/// stores date &amp; time down to the accuracy of a second.
	/// </summary>
	/// <remarks>
	/// This only stores down to a second, so if you are looking for the most accurate
	/// date and time storage your provider can give you use the <see cref="DateTimeType" />
	/// or the <see cref="TicksType"/>. This type is equivalent to the Hibernate <c>DateTime</c> type.
	/// </remarks>
	[Serializable]
	public partial class DateTimeNoMsType : AbstractDateTimeType
	{
		/// <inheritdoc />
		public override string Name => "DateTimeNoMs";

		/// <inheritdoc />
		protected override DateTime AdjustDateTime(DateTime dateValue) =>
			base.AdjustDateTime(new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second));

		#region IVersionType Members

		public override object Seed(ISessionImplementor session)
		{
			return Round(Now, TimeSpan.TicksPerSecond);
		}

		#endregion

		/// <inheritdoc />
		public override bool IsEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			var date1 = (DateTime) x;
			var date2 = (DateTime) y;

			if (date1.Equals(date2))
				return true;

			return (date1.Equals(date2) ||
					date1.Year == date2.Year &&
					date1.Month == date2.Month &&
					date1.Day == date2.Day &&
					date1.Hour == date2.Hour &&
					date1.Minute == date2.Minute &&
					date1.Second == date2.Second) &&
				(Kind == DateTimeKind.Unspecified || date1.Kind == date2.Kind);
		}

		/// <inheritdoc />
		public override int GetHashCode(object x)
		{
			// Custom hash code implementation because DateTimeType is only accurate
			// up to seconds.
			var date = (DateTime) x;
			var hashCode = 1;
			unchecked
			{
				hashCode = 31 * hashCode + date.Second;
				hashCode = 31 * hashCode + date.Minute;
				hashCode = 31 * hashCode + date.Hour;
				hashCode = 31 * hashCode + date.Day;
				hashCode = 31 * hashCode + date.Month;
				hashCode = 31 * hashCode + date.Year;
				if (Kind != DateTimeKind.Unspecified)
					hashCode = 31 * hashCode + date.Kind.GetHashCode();
			}

			return hashCode;
		}
	}
}
