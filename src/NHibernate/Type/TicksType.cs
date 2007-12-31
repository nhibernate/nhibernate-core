using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an <see cref="DbType.Int64" /> column 
	/// that stores the DateTime using the Ticks property.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column.  The System.DateTime.Ticks 
	/// is accurate to 100-nanosecond intervals. 
	/// </remarks>
	[Serializable]
	public class TicksType : PrimitiveType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		internal TicksType() : base(SqlTypeFactory.Int64)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			return new DateTime(Convert.ToInt64(rs[index]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand st, object value, int index)
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = ((DateTime) value).Ticks;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Ticks"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToString(object val)
		{
			return ((DateTime) val).Ticks.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue(string xml)
		{
			return new DateTime(long.Parse(xml));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			long xTime = ((DateTime) x).Ticks;
			long yTime = ((DateTime) y).Ticks;
			return xTime == yTime;
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return true; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return long.Parse(xml);
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + ((DateTime) value).Ticks.ToString() + '\'';
		}
	}
}