using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a System.Single Property to an column that can store a single precision number.
	/// </summary>
	/// <remarks>
	/// Verify through your database's documentation if there is a column type that
	/// matches up with the capabilities of System.Single  
	/// </remarks>
	public class SingleType : PrimitiveType 
	{
		
		internal SingleType(SingleSqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToSingle(rs[index]);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Convert.ToSingle(rs[name]);
		}

		public override System.Type PrimitiveClass 
		{
			get { return typeof(System.Single); }
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(System.Single); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name 
		{
			get { return "Single"; }
		}

		public override string ObjectToSQLString(object value) 
		{
			return value.ToString();
		}
	}
}
