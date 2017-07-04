using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public abstract class AbstractStringType: ImmutableType, IDiscriminatorType
	{
		public AbstractStringType(SqlType sqlType)
			: base(sqlType)
		{
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];

			// set the parameter value before the size check, since ODBC changes the size automatically
			parameter.Value = value;

			if (parameter.Size > 0 && ((string)value).Length > parameter.Size)
				throw new HibernateException("The length of the string value exceeds the length configured in the mapping/parameter.");
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return Convert.ToString(rs[index]);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Convert.ToString(rs[name]);
		}

		public override string ToString(object val)
		{
			return (string)val;
		}

		public override object FromStringValue(string xml)
		{
			return xml;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(string); }
		}

		#region IIdentifierType Members

		public object StringToObject(string xml)
		{
			return xml;
		}

		#endregion

		#region ILiteralType Members

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + (string)value + "'";
		}

		#endregion
	}
}