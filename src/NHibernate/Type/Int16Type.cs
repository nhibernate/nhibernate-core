using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	/// <summary>
	/// Maps a <see cref="System.Int16"/> Property 
	/// to a <see cref="DbType.Int16"/> column.
	/// </summary>
	public class Int16Type : ValueTypeType, IDiscriminatorType, IVersionType 
	{
		
		internal Int16Type() : base( new Int16SqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) {
			return Convert.ToInt16(rs[index]);
		}

		public override object Get(IDataReader rs, string name) {
			return Convert.ToInt16(rs[name]);
		}

		public override System.Type ReturnedClass {
			get { return typeof(System.Int16); }
		}

		public override void Set(IDbCommand rs, object value, int index) {
			IDataParameter parm = rs.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Int16"; }
		}
		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
		public object StringToObject(string xml) {
			return short.Parse(xml);
		}

		#region IVersionType Members

		public object Next(object current) 
		{
			return (short)( (short)current + 1 );
		}
		
		public object Seed 
		{
			get { return (short)0; }
		}

		#endregion

	}
}
