using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// This is almost the exact same type as the DateTime except it can be used
	/// in the version column, stores it to the accuracy the database supports, 
	/// and will default to the value of DateTime.Now if the value is null.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The value stored in the database depends on what your data provider is capable
	/// of storing.  So there is a possibility that the DateTime you save will not be
	/// the same DateTime you get back when you check DateTime.Equals(DateTime) because
	/// they will have their milliseconds off.
	/// </p>  
	/// <p>
	/// For example - SQL Server 2000 is only accurate to 3.33 milliseconds.  So if 
	/// NHibernate writes a value of <c>01/01/98 23:59:59.995</c> to the Prepared Command, MsSql
	/// will store it as <c>1998-01-01 23:59:59.997</c>.
	/// </p>
	/// <p>
	/// Please review the documentation of your Database server.
	/// </p>
	/// </remarks>
	[Serializable]
	public class TimestampType : PrimitiveType, IVersionType, ILiteralType
	{
		public TimestampType() : base(SqlTypeFactory.DateTime)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToDateTime(rs[index]);
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime); }
		}

		/// <summary>
		/// Sets the value of this Type in the IDbCommand.
		/// </summary>
		/// <param name="st">The IDbCommand to add the Type's value to.</param>
		/// <param name="value">The value of the Type.</param>
		/// <param name="index">The index of the IDataParameter in the IDbCommand.</param>
		/// <remarks>
		/// No null values will be written to the IDbCommand for this Type. 
		/// </remarks>
		public override void Set(IDbCommand st, object value, int index)
		{
			((IDataParameter)st.Parameters[index]).Value = (value is DateTime) ? value:DateTime.Now;
		}

		public override string Name
		{
			get { return "Timestamp"; }
		}

		public override string ToString(object val)
		{
			return ((DateTime) val).ToShortTimeString();
		}

		public override object FromStringValue(string xml)
		{
			return DateTime.Parse(xml);
		}

		#region IVersionType Members

		public object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		public static DateTime Round(DateTime value, long resolution)
		{
			return value.AddTicks(-(value.Ticks % resolution));
		}

		public virtual object Seed(ISessionImplementor session)
		{
			if (session == null)
			{
				return DateTime.Now;
			}
			return Round(DateTime.Now, session.Factory.Dialect.TimestampResolutionInTicks);
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion

		public object StringToObject(string xml)
		{
			return DateTime.Parse(xml);
		}

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
			return '\'' + value.ToString() + '\'';
		}
	}
}