using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Int64" /> column 
	/// </summary>
	[Serializable]
	public class TimeSpanType : PrimitiveType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		internal TimeSpanType()
			: base(SqlTypeFactory.Int64)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "TimeSpan"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				return new TimeSpan(Convert.ToInt64(rs[index]));
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
				return new TimeSpan(Convert.ToInt64(rs[name]));
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(TimeSpan); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand st, object value, int index)
		{
			((IDataParameter)st.Parameters[index]).Value = ((TimeSpan)value).Ticks;
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

		/// <summary></summary>
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