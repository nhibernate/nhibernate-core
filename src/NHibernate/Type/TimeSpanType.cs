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
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Int64" /> column 
	/// </summary>
	[Serializable]
	public partial class TimeSpanType : PrimitiveType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		public TimeSpanType()
			: base(SqlTypeFactory.Int64)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "TimeSpan"; }
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
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

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
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

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = ((TimeSpan)value).Ticks;
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

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public object StringToObject(string xml)
		{
			return TimeSpan.Parse(xml);
		}

		public IComparer Comparator
		{
			get { return Comparer<TimeSpan>.Default; }
		}

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
