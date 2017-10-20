using System;
using System.Data;
using NHibernate.SqlTypes;

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
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DateTimeType()
		{
		}

		/// <summary>
		/// Constructor for specifying a datetime with a scale. Use <see cref="SqlTypeFactory.GetDateTime"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public DateTimeType(DateTimeSqlType sqlType) : base(sqlType)
		{
		}

		/// <inheritdoc />
		public override string Name => "DateTime";
	}
}
