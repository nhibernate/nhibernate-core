using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an DateTime column that only stores the 
	/// Hours, Minutes, and Seconds of the DateTime as significant.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This defaults the Date to "1753-01-01" - that should not matter because
	/// using this Type indicates that you don't care about the Date portion of the DateTime.
	/// </para>
	/// <para>
	/// A more appropriate choice to store the duration/time is the <see cref="TimeSpanType"/>.
	/// The underlying <see cref="DbType.Time"/> tends to be handled diffently by different
	/// DataProviders.
	/// </para>
	/// </remarks>
	[Serializable]
	public class TimeType : PrimitiveType, IIdentifierType, ILiteralType
	{
		private static DateTime BaseDateValue = new DateTime(1753, 01, 01);

		/// <summary></summary>
		internal TimeType() : base(SqlTypeFactory.Time)
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
			DateTime dbValue = Convert.ToDateTime(rs[index]);
			return new DateTime(1753, 01, 01, dbValue.Hour, dbValue.Minute, dbValue.Second);
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
			if ((DateTime) value < BaseDateValue)
			{
				parm.Value = DBNull.Value;
			}
			else
			{
				parm.Value = value;
			}
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

			DateTime date1 = (DateTime) x;
			DateTime date2 = (DateTime) y;

			return date1.Hour == date2.Hour
			       && date1.Minute == date2.Minute
			       && date1.Second == date2.Second;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Time"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToString(object val)
		{
			return ((DateTime) val).ToShortTimeString();
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return FromString(xml);
		}

		public override object FromStringValue(string xml)
		{
			return DateTime.Parse(xml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((DateTime) value).ToShortTimeString() + "'";
		}
	}
}