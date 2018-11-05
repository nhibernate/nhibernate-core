using System;
using NHibernate.Engine;

namespace NHibernate.Type
{
	// Obsolete since v5.0
	/// <summary>
	/// This is almost the exact same type as the <see cref="DateTimeType" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The value stored in the database depends on what your data provider is capable
	/// of storing. So there is a possibility that the DateTime you save will not be
	/// the same DateTime you get back when you check <see cref="DateTime.Equals(DateTime)" /> because
	/// they will have their milliseconds off.
	/// </para>
	/// <para>
	/// For example - SQL Server 2000 is only accurate to 3.33 milliseconds. So if
	/// NHibernate writes a value of <c>01/01/98 23:59:59.995</c> to the Prepared Command, MsSql
	/// will store it as <c>1998-01-01 23:59:59.997</c>.
	/// </para>
	/// <para>
	/// Please review the documentation of your Database server.
	/// </para>
	/// <para>
	/// If you are looking for the most accurate date and time storage accross databases use the
	/// <see cref="TicksType"/>.
	/// </para>
	/// </remarks>
	[Obsolete("Please use DateTimeType instead.")]
	[Serializable]
	public class TimestampType : AbstractDateTimeType
	{
		/// <inheritdoc />
		public override string Name => "Timestamp";

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		/// <summary>
		/// Retrieve the string representation of the timestamp object. This is in the following format:
		/// <code>
		/// 2011-01-27T14:50:59.6220000Z
		/// </code>
		/// </summary>
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val) =>
			((DateTime) val).ToString("O");
	}
}
