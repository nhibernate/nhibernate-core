using System;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	/// <summary>
	/// Maps a <see cref="System.SByte"/> Property 
	/// to a <see cref="DbType.SByte"/> column.
	/// </summary>
	public class SByteType : ValueTypeType, IDiscriminatorType 
	{
		
		internal SByteType() : base( new SByteSqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			return Convert.ToSByte(rs[index]); 
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Convert.ToSByte(rs[name]);
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(SByte); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) 
		{
			( (IDataParameter)cmd.Parameters[index] ).Value = (SByte) value;
		}

		public override string Name 
		{
			get { return "SByte"; }
		}

		public override string ObjectToSQLString(object value) 
		{
			return value.ToString();
		}

		public virtual object StringToObject(string xml) 
		{
			return SByte.Parse(xml);
		}
	}
}
