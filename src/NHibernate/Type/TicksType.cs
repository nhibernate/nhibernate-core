using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a System.DateTime Property to an Int64 column that stores the DateTime using
	/// the Ticks property.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column.  The System.DateTime.Ticks 
	/// is accurate to 100-nanosecond intervals. 
	/// </remarks>
	public class TicksType : MutableType, IVersionType, ILiteralType
	{
		
		public TicksType(Int64SqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return new DateTime( Convert.ToInt64(rs[index]) );
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = ((DateTime)value).Ticks;
		}

		public override string Name 
		{
			get { return "Ticks"; }
		}

		public override string ToXML(object val) 
		{
			return ((DateTime)val).Ticks.ToString();
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
			return xTime == yTime; 
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
			get { return DateTime.Now.Ticks; }
		}

		public string ObjectToSQLString(object value) 
		{
			return "'" + ((DateTime)value).Ticks.ToString() + "'";
		}
	}
}

