using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Time" /> column 
	/// This is an extra way to map a <see cref="DbType.Time"/>. You already have <see cref="TimeType"/>
	/// but mapping against a <see cref="DateTime"/>.
	/// </summary>
	[Serializable]
	public partial class TimeAsTimeSpanType : PrimitiveType, IVersionType
	{
		private static readonly object ZeroObject = TimeSpan.Zero;
		private static readonly DateTime BaseDateValue = new DateTime(1753, 01, 01);

		/// <summary />
		public TimeAsTimeSpanType() : base(SqlTypeFactory.Time)
		{
		}

		/// <summary />
		public TimeAsTimeSpanType(TimeSqlType sqlType) : base(sqlType)
		{
		}

		public override string Name => "TimeAsTimeSpan";

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				if (rs.TryGetTimeSpan(index, out var dbTimeSpan))
				{
					return dbTimeSpan;
				}

				if (!rs.TryGetDateTime(index, out var dbDateTime))
				{
					var locale = session.Factory.Settings.Locale;

					dbDateTime = Convert.ToDateTime(rs[index], locale);
				}

				return dbDateTime.TimeOfDay;
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			if (session.Factory.ConnectionProvider.Driver.RequiresTimeSpanForTime)
				st.Parameters[index].Value = value;
			else
				st.Parameters[index].Value = BaseDateValue.AddTicks(((TimeSpan)value).Ticks);
		}

		public override System.Type ReturnedClass => typeof(TimeSpan);

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
			return ((TimeSpan)val).Ticks.ToString();
		}

		#region IVersionType Members

		public object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return new TimeSpan(DateTime.Now.Ticks);
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public object StringToObject(string xml)
		{
			return TimeSpan.Parse(xml);
		}

		public IComparer Comparator => Comparer<TimeSpan>.Default;

		#endregion

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior. Replace override keyword by virtual after having
		// removed the obsoleted base.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
#pragma warning disable 672
		public override object FromStringValue(string xml)
#pragma warning restore 672
		{
			return TimeSpan.Parse(xml);
		}

		public override System.Type PrimitiveClass => typeof(TimeSpan);

		public override object DefaultValue => ZeroObject;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((TimeSpan)value).Ticks.ToString() + '\'';
		}
	}
}
