using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{

	public class GuidType : PrimitiveType, IDiscriminatorType 
	{
	
		internal GuidType(GuidSqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return new Guid( Convert.ToString(rs[index]) );
		}

		public override object Get(IDataReader rs, string name) 
		{
			return new Guid( Convert.ToString(rs[name]) );
		}

		public override System.Type PrimitiveClass 
		{
			get { return typeof(System.Guid); }
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(System.Guid); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) 
		{
			IDataParameter parm = cmd.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name 
		{
			get { return "Guid"; }
		}

		public override string ObjectToSQLString(object value) 
		{
			return value.ToString();
		}

		public object StringToObject(string xml) 
		{
			return new Guid(xml);
		}

	}
}
