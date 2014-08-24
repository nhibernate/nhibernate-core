using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Common base class for <see cref="CharType" /> and <see cref="AnsiCharType" />.
	/// </summary>
	[Serializable]
	public abstract class AbstractCharType : PrimitiveType, IDiscriminatorType
	{
		public AbstractCharType(SqlType sqlType)
			: base(sqlType) {}

		public override object DefaultValue
		{
			get { throw new NotSupportedException("not a valid id type"); }
		}

		public override object Get(IDataReader rs, int index)
		{
			string dbValue = Convert.ToString(rs[index]);
			// The check of the Length is a workaround see NH-2340
			if (dbValue.Length > 0)
			{
				return dbValue[0];
			}
			return '\0'; // This line should never be executed
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(char); }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(char); }
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			((IDataParameter)cmd.Parameters[index]).Value = Convert.ToChar(value);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + value.ToString() + '\'';
		}

		public virtual object StringToObject(string xml)
		{
			if (xml.Length != 1)
				throw new MappingException("multiple or zero characters found parsing string");

			return xml[0];
		}

		public override object FromStringValue(string xml)
		{
			return xml[0];
		}
	}
}