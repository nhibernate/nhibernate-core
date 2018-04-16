using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
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
		public DoubleType() : base(SqlTypeFactory.Double)
		{
		}

		public DoubleType(SqlType sqlType) : base(sqlType) {}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return Convert.ToDouble(rs[index]);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Convert.ToDouble(rs[name]);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(double); }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = value;
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