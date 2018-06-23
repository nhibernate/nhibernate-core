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
	/// Maps a <see cref="System.DateTime" /> Property to an <see cref="DbType.Int64" /> column 
	/// that stores the DateTime using the Ticks property.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column.  
	/// The System.DateTime.Ticks is accurate to 100-nanosecond intervals. 
	/// </remarks>
	[Serializable]
	public partial class TicksType : PrimitiveType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		public TicksType()
			: base(SqlTypeFactory.Int64) {}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return new DateTime(Convert.ToInt64(rs[index]));
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime); }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = ((DateTime)value).Ticks;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Ticks"; }
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

		public object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return DateTime.Now;
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public object StringToObject(string xml)
		{
			return Int64.Parse(xml);
		}

		public IComparer Comparator
		{
			get { return Comparer<DateTime>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof(DateTime); }
		}

		public override object DefaultValue
		{
			get { return DateTime.MinValue; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((DateTime)value).Ticks.ToString() + '\'';
		}
	}
}
