using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Base class for date time types.
	/// </summary>
	[Serializable]
	public abstract partial class AbstractDateTimeType : PrimitiveType, IIdentifierType, ILiteralType, IVersionType
	{
		private static readonly DateTime BaseDateValue = DateTime.MinValue;

		/// <summary>
		/// Returns the <see cref="DateTimeKind" /> for the type.
		/// </summary>
		protected virtual DateTimeKind Kind => DateTimeKind.Unspecified;

		/// <inheritdoc />
		public override System.Type ReturnedClass => typeof(DateTime);

		/// <summary>
		/// Retrieve the current system time.
		/// </summary>
		/// <value><see cref="DateTime.UtcNow" /> if <see cref="Kind" /> is <see cref="DateTimeKind.Utc" />,
		/// <see cref="DateTime.Now" /> otherwise.</value>
		protected virtual DateTime Now => Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected AbstractDateTimeType() : base(SqlTypeFactory.DateTime)
		{
		}

		/// <summary>
		/// Constructor for overriding the default <see cref="SqlType"/>.
		/// </summary>
		/// <param name="sqlTypeDateTime">The <see cref="SqlType"/> to use.</param>
		protected AbstractDateTimeType(SqlType sqlTypeDateTime) : base(sqlTypeDateTime)
		{
		}

		/// <summary>
		/// Adjust the date time value for this type from an arbitrary date time value.
		/// </summary>
		/// <param name="dateValue">The <see cref="System.DateTime" /> to adjust.</param>
		/// <returns>A <see cref="System.DateTime" />.</returns>
		protected virtual DateTime AdjustDateTime(DateTime dateValue) =>
			Kind == DateTimeKind.Unspecified ? dateValue : DateTime.SpecifyKind(dateValue, Kind);

		/// <inheritdoc />
		public override object Get(DbDataReader rs, int index, ISessionImplementor session) =>
			GetDateTime(rs, index, session);

		/// <inheritdoc />
		public override object Get(DbDataReader rs, string name, ISessionImplementor session) =>
			Get(rs, rs.GetOrdinal(name), session);

		/// <summary>
		/// Get the <see cref="DateTime" /> in the <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>An object with the value from the database.</returns>
		protected virtual DateTime GetDateTime(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return AdjustDateTime(Convert.ToDateTime(rs[index]));
			}
			catch (Exception ex)
			{
				throw new FormatException($"Input string '{rs[index]}' was not in the correct format.", ex);
			}
		}

		/// <inheritdoc />
		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			var dateValue = (DateTime) value;
			// We could try convert. This is always doable when going to local. But the other way may encounter
			// ambiguous times. .Net then always assumes the time to be without daylight shift, causing the ambiguous
			// hour with daylight shift to be always wrongly converted. So better just fail.
			if (Kind != DateTimeKind.Unspecified && dateValue.Kind != Kind)
				throw new ArgumentException($"{Name} expect date kind {Kind} but it is {dateValue.Kind}", nameof(value));
			st.Parameters[index].Value = AdjustDateTime(dateValue);
		}

		#region IVersionType Members

		/// <inheritdoc />
		public object Next(object current, ISessionImplementor session) =>
			Seed(session);

		/// <summary>
		/// Round a <see cref="DateTime"/> according to specified resolution.
		/// </summary>
		/// <param name="value">The value to round.</param>
		/// <param name="resolution">The resolution in ticks (100ns).</param>
		/// <returns>A rounded <see cref="DateTime"/>.</returns>
		public static DateTime Round(DateTime value, long resolution) =>
			value.AddTicks(-(value.Ticks % resolution));

		/// <inheritdoc />
		public virtual object Seed(ISessionImplementor session) =>
			session == null ? Now : Round(Now, session.Factory.Dialect.TimestampResolutionInTicks);

		/// <inheritdoc />
		public virtual IComparer Comparator => Comparer<DateTime>.Default;

		#endregion

		/// <summary>
		/// Compares two <see cref="DateTime" /> object and also compare its Kind if needed, which is not used by the
		/// .Net Framework <see cref="DateTime.Equals(object)"/> implementation.
		/// </summary>
		/// <param name="x">The first date time to compare.</param>
		/// <param name="y">The second date time to compare.</param>
		/// <returns><see langword="true" /> if they are equals, <see langword="false" /> otherwise.</returns>
		public override bool IsEqual(object x, object y) =>
			base.IsEqual(x, y) &&
			(Kind == DateTimeKind.Unspecified || x == null || ((DateTime) x).Kind == ((DateTime) y).Kind);

		/// <inheritdoc />
		public override string ToString(object val) =>
			((DateTime) val).ToString(CultureInfo.CurrentCulture);

		/// <inheritdoc />
		public object StringToObject(string xml) =>
			string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);

		/// <inheritdoc />
		public override object FromStringValue(string xml)
		{
			// Parsing with .Net always yield a Local date.
			var date = DateTime.Parse(xml);
			if (Kind == DateTimeKind.Utc)
				date = date.ToUniversalTime();
			return date;
		}

		/// <inheritdoc />
		public override System.Type PrimitiveClass => typeof(DateTime);

		/// <inheritdoc />
		public override object DefaultValue => BaseDateValue;

		/// <inheritdoc />
		public override string ObjectToSQLString(object value, Dialect.Dialect dialect) =>
			"'" + (DateTime) value + "'";
	}
}
