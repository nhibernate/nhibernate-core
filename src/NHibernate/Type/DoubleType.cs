using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Double"/> Property 
	/// to a <see cref="DbType.Double"/> column.
	/// </summary>
	[Serializable]
	public class DoubleType : PrimitiveType
	{
		/// <summary></summary>
		internal DoubleType() : base(SqlTypeFactory.Double)
		{
		}

		internal DoubleType(SqlType sqlType) : base(sqlType) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToDouble(rs[index]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Convert.ToDouble(rs[name]);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(double); }
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
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Double"; }
		}

		public override object FromStringValue(string xml)
		{
			return double.Parse(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(double); }
		}

		public override object DefaultValue
		{
			get { return 0D; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}