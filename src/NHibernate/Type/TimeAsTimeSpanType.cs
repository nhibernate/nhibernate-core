using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Time" /> column 
	/// This is an extra way to map a <see cref="DbType.Time"/>. You already have <see cref="TimeType"/>
	/// but mapping against a <see cref="DateTime"/>.
	/// </summary>
	[Serializable]
	public class TimeAsTimeSpanType : PrimitiveType, IVersionType
	{
		private static readonly DateTime BaseDateValue = new DateTime(1753, 01, 01);

		public TimeAsTimeSpanType()
			: base(SqlTypeFactory.Time)
		{
		}

		public override string Name
		{
			get { return "TimeAsTimeSpan"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				object value = rs[index];
				if(value is TimeSpan)
					return (TimeSpan)value;
                
				return ((DateTime)value).TimeOfDay;
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			try
			{
				object value = rs[name];
				if (value is TimeSpan) //For those dialects where DbType.Time means TimeSpan.
					return (TimeSpan)value;

				return ((DateTime)value).TimeOfDay;
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override void Set(IDbCommand st, object value, int index)
		{
			DateTime date = BaseDateValue.AddTicks(((TimeSpan)value).Ticks);
			((IDataParameter) st.Parameters[index]).Value = date;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(TimeSpan); }
		}

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

		public object StringToObject(string xml)
		{
			return TimeSpan.Parse(xml);
		}

		public IComparer Comparator
		{
			get { return Comparer<TimeSpan>.Default; }
		}

		#endregion

		public override object FromStringValue(string xml)
		{
			return TimeSpan.Parse(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(TimeSpan); }
		}

		public override object DefaultValue
		{
			get { return TimeSpan.Zero; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((TimeSpan)value).Ticks.ToString() + '\'';
		}
	}
}