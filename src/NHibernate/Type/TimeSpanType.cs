using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a <see cref="System.Timespan" /> Property to an <see cref="DbType.Int64" /> column 
	/// </summary>
	public class TimeSpanType : ValueTypeType, IVersionType, ILiteralType
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

		#region IVersionType Members

		public object Next(object current) 
		{
			return Seed;
		}
		
		public object Seed 
		{
			get { return new TimeSpan( DateTime.Now.Ticks ); }
		}

		#endregion

		public override string ObjectToSQLString(object value) 
		{
			return "'" + ((TimeSpan)value).Ticks.ToString() + "'";
		}
	}
}

