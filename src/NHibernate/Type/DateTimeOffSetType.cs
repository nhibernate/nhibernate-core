using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTimeOffset" /> Property to a <see cref="DbType.DateTimeOffset"/>
	/// </summary>
	[Serializable]
	public partial class DateTimeOffsetType : PrimitiveType, IIdentifierType, ILiteralType, IVersionType
	{
		static readonly DateTimeOffset BaseDateValue = DateTimeOffset.MinValue;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public DateTimeOffsetType() : base(SqlTypeFactory.DateTimeOffSet)
		{
		}

		/// <summary>
		/// Constructor for specifying a datetimeoffset with a scale. Use <see cref="SqlTypeFactory.GetDateTimeOffset"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public DateTimeOffsetType(DateTimeOffsetSqlType sqlType) : base(sqlType)
		{
		}

		public override string Name
		{
			get { return "DateTimeOffset"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (DateTimeOffset); }
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof (DateTimeOffset); }
		}

		public override object DefaultValue
		{
			get { return BaseDateValue; }
		}

		public IComparer Comparator
		{
			get { return Comparer<DateTimeOffset>.Default; }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = value;
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return (DateTimeOffset) rs[index];
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		/// <summary>
		/// Truncate a <see cref="DateTimeOffset"/> according to specified resolution.
		/// </summary>
		/// <param name="value">The value to round.</param>
		/// <param name="resolution">The resolution in ticks (100ns).</param>
		/// <returns>A rounded <see cref="DateTimeOffset"/>.</returns>
		public static DateTimeOffset Round(DateTimeOffset value, long resolution) =>
			value.AddTicks(-(value.Ticks % resolution));

		/// <inheritdoc />
		public virtual object Seed(ISessionImplementor session) =>
			session == null ? DateTimeOffset.Now : Round(DateTimeOffset.Now, session.Factory.Dialect.TimestampResolutionInTicks);

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

			var date1 = (DateTimeOffset) x;
			var date2 = (DateTimeOffset) y;

			return date1.Equals(date2);
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null :
				// 6.0 TODO: remove warning disable/restore
#pragma warning disable 618
				FromStringValue(xml);
#pragma warning restore 618
		}

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
			return ((DateTimeOffset) val).ToString();
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior. Replace override keyword by virtual after having
		// removed the obsoleted base.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
#pragma warning disable 672
		public override object FromStringValue(string xml)
#pragma warning restore 672
		{
			return DateTimeOffset.Parse(xml);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((DateTimeOffset) value) + "'";
		}
	}
}
