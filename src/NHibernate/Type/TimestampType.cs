using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// This is almost the exact same type as the DateTime except it can be used
	/// in the version column and stores it to the accuracy the Database supports.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The value stored in the database depends on what your Data Provider is capable
	/// of storing.  So there is a possibility that the DateTime you save will not be
	/// the same DateTime you get back when you check DateTime.Equals(DateTime) because
	/// they will have their milliseconds off.
	/// </p>  
	/// <p>
	/// For example - MsSql Server 2000 is only accurate to 3.33 milliseconds.  So if 
	/// NHibernate writes a value of '01/01/98 23:59:59.995' to the Prepared Command, MsSql
	/// will store it as '1998-01-01 23:59:59.997'.
	/// </p>
	/// <p>
	/// Please review the documentation of your Database server.
	/// </p>
	/// 
	/// </remarks>
	public class TimestampType : MutableType, IVersionType, ILiteralType
	{
		
		public TimestampType(DateTimeSqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return rs.GetDateTime(index);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));// rs.[name];
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name 
		{
			get { return "Timestamp"; }
		}

		public override string ToXML(object val) 
		{
			return ((DateTime)val).ToShortTimeString();
		}

		public override object DeepCopyNotNull(object value) 
		{
			return value;
		}

		public override bool Equals(object x, object y) 
		{
			if (x==y) return true;
			if (x==null || y==null) return false;

			long xTime = ((DateTime)x).Ticks;
			long yTime = ((DateTime)y).Ticks;
			return xTime == yTime; //TODO: Fixup
		}

		public override bool HasNiceEquals 
		{
			get { return true; }
		}

		public object Next(object current) 
		{
			return Seed;
		}
		
		public object Seed 
		{
			get { return DateTime.Now; }
		}

		public string ObjectToSQLString(object value) 
		{
			return "'" + value.ToString() + "'";
		}
	}
}
