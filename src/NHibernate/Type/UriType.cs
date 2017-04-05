using System;
using System.Data.Common;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class UriType : ImmutableType, IDiscriminatorType
	{
		public UriType()
			: base(new StringSqlType())
		{
		}

		public UriType(SqlType sqlType) : base(sqlType)
		{
		}

		public override string Name
		{
			get { return "Uri"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Uri); }
		}

		public object StringToObject(string xml)
		{
			return new Uri(xml, UriKind.RelativeOrAbsolute);
		}

		public override void Set(DbCommand cmd, object value, int index)
		{
			cmd.Parameters[index].Value = ToString(value);
		}

		public override object Get(DbDataReader rs, int index)
		{
			return StringToObject(Convert.ToString(rs[index]));
		}

		public override object Get(DbDataReader rs, string name)
		{
			return StringToObject(Convert.ToString(rs[name]));
		}

		public override string ToString(object val)
		{
			return ((Uri)val).OriginalString;
		}

		public override object FromStringValue(string xml)
		{
			return StringToObject(xml);
		}

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((Uri)value).OriginalString + "'";
		}
	}
}