using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{

	/// <summary>
	/// DecimalType
	/// </summary>
	public class DecimalType : ValueTypeType, IIdentifierType
	{
		internal DecimalType() : this( new DecimalSqlType() ) 
		{
		}
		
		internal DecimalType(DecimalSqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return Convert.ToDecimal(rs[index]);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Convert.ToDecimal(rs[name]);
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(Decimal); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name 
		{
			get { return "Decimal"; }
		}

		public object StringToObject(string xml) 
		{
			return long.Parse(xml);
		}

		public override string ObjectToSQLString(object value) 
		{
			return value.ToString();
		}
	}
}