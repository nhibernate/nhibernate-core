using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.String" /> to a <see cref="DbType.String" /> column.
	/// </summary>
	[Serializable]
	public class StringType : ImmutableType, IDiscriminatorType
	{
		/// <summary></summary>
		internal StringType() : base(new StringSqlType())
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlType"></param>
		internal StringType(StringSqlType sqlType) : base(sqlType)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToString(rs[index]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Convert.ToString(rs[name]);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(string); }
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
			get { return "String"; }
		}

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + (string)value + '\'';
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return xml;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(object x, object y)
		{
			return ObjectUtils.Equals(x, y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToString(object value)
		{
			return (string) value;
		}

		public override object FromStringValue(string xml)
		{
			return xml;
		}
	}
}