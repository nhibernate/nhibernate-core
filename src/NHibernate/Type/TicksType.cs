using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an <see cref="DbType.Int64" /> column
	/// that stores the DateTime using the Ticks property.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column, along with <see cref="UtcTicksType" />.
	/// The System.DateTime.Ticks is accurate to 100-nanosecond intervals.
	/// This type yields dates with an unspecified <see cref="DateTime.Kind"/>. On writes, it
	/// does not perform any checks or conversions related to the kind of the date value to persist.
	/// </remarks>
	[Serializable]
	public partial class TicksType : AbstractDateTimeType
	{
		/// <summary></summary>
		public TicksType()
			: base(SqlTypeFactory.Int64) {}

		/// <summary>
		/// Get the <see cref="DateTime" /> in the <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>An object with the value from the database.</returns>
		protected override DateTime GetDateTime(DbDataReader rs, int index, ISessionImplementor session)
		{
			return new DateTime(Convert.ToInt64(rs[index]), Kind);
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			var dateValue = (DateTime) value;
			// We could try to convert. This is always doable when going to local. But the other way may encounter
			// ambiguous times. .Net then always assumes the time to be without daylight shift, causing the ambiguous
			// hour with daylight shift to be always wrongly converted. So better just fail.
			if (Kind != DateTimeKind.Unspecified && dateValue.Kind != Kind)
				throw new ArgumentException($"{Name} expect date kind {Kind} but it is {dateValue.Kind}", nameof(value));
			st.Parameters[index].Value = dateValue.Ticks;
		}

		/// <inheritdoc />
		public override string Name => "Ticks";

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return ((DateTime)val).Ticks.ToString();
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior. Replace override keyword by virtual after having
		// removed the obsoleted base.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
#pragma warning disable 672
		public override object FromStringValue(string xml)
#pragma warning restore 672
		{
			return new DateTime(Int64.Parse(xml));
		}

		#region IVersionType Members

		public override object Seed(ISessionImplementor session)
		{
			return Now;
		}

		// This does not replace AbstractDateTimeType.StringToObject even when StringToObject is called
		// through IIdentifierType interface, as long as TicksType does not re-declare it implements the interface.
		// We need to keep the base implementation for the IIdentifierType method, because it has to yield values
		// of the type ReturnedClass, not of the type persisted to DB.
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public new object StringToObject(string xml)
		{
			return Int64.Parse(xml);
		}

		#endregion

		/// <inheritdoc />
		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((DateTime)value).Ticks.ToString() + '\'';
		}
	}
}
