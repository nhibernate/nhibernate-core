using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a System.Timespan Property to an Int64 column 
	/// </summary>
	public class TimeSpanType : MutableType, IVersionType, ILiteralType
	{
		internal TimeSpanType() : base( new Int64SqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return new TimeSpan( Convert.ToInt64(rs[index]) );
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(TimeSpan); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = ((TimeSpan)value).Ticks;
		}

		public override string Name 
		{
			get { return "TimeSpan"; }
		}

		public override string ToXML(object val) 
		{
			return ((TimeSpan)val).Ticks.ToString();
		}

		public override object DeepCopyNotNull(object value) 
		{
			return value;
		}

		public override bool Equals(object x, object y) 
		{
			if (x==y) return true;
			if (x==null || y==null) return false;

			long xTime = ((TimeSpan)x).Ticks;
			long yTime = ((TimeSpan)y).Ticks;
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
			return "'" + ((TimeSpan)value).Ticks.ToString() + "'";
		}
	}
}

