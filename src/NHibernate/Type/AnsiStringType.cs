using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.String"/> Property 
	/// to a <see cref="DbType.AnsiString"/> column.
	/// </summary>
	public class AnsiStringType : ImmutableType, IDiscriminatorType 
	{
		internal AnsiStringType(AnsiStringSqlType sqlType) : base(sqlType) {}

		public override object Get(IDataReader rs, int index) 
		{
			return rs.GetString(index);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));
		}
		public override System.Type ReturnedClass 
		{
			get { return typeof(string); }
		}
		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name 
		{
			get { return "AnsiString"; }
		}
		public string ObjectToSQLString(object value) 
		{
			return "'" + (string) value + "'";
		}
		public object StringToObject(string xml) 
		{
			return xml;
		}

		public override bool Equals(object x, object y) 
		{
			return ObjectUtils.Equals(x, y);
		}

		public override string ToXML(object value) 
		{
			return (string) value;
		}
	}
}


