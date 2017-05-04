using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;
using System.Data;

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
		private static readonly DateTime BaseDateValue = new DateTime(1753, 01, 01);

		public TimeAsTimeSpanType()
			: base(SqlTypeFactory.Time)
		{
		}

		public override string Name
		{
			get { return "TimeAsTimeSpan"; }
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				var value = rs[index];
				if(value is TimeSpan time) //For those dialects where DbType.Time means TimeSpan.
					return time;
                
				return ((DateTime)value).TimeOfDay;
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			if (session.Factory.ConnectionProvider.Driver.RequiresTimeSpanForTime)
				st.Parameters[index].Value = value;
			else
				st.Parameters[index].Value = BaseDateValue.AddTicks(((TimeSpan)value).Ticks);
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