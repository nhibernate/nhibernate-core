using System;
using System.Data;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> property to a <see cref="DbType.DateTime"/> column that
	/// stores date &amp; time down to the accuracy of the database.
	/// </summary>
	/// <remarks>
	/// If you are looking for the most accurate date and time storage accross databases use the
	/// <see cref="TicksType"/>. If you are looking for the Hibernate <c>DateTime</c> equivalent,
	/// use the <see cref="DateTimeNoMsType" />.
	/// </remarks>
	[Serializable]
	public class DateTimeType : AbstractDateTimeType
	{
		/// <inheritdoc />
		public override string Name => "DateTime";
	}
}
