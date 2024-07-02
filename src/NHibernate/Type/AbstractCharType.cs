using System;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Common base class for <see cref="CharType" /> and <see cref="AnsiCharType" />.
	/// </summary>
	[Serializable]
	public abstract class AbstractCharType : PrimitiveType, IDiscriminatorType
	{
		/// <summary />
		public AbstractCharType(SqlType sqlType) : base(sqlType)
		{
		}

		public override object DefaultValue => throw new NotSupportedException("not a valid id type");

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			if (rs.TryGetChar(index, out var dbValue))
			{
				return dbValue;
			}

			if (!rs.TryGetString(index, out var strValue))
			{
				var locale = session.Factory.Settings.Locale;

				strValue = Convert.ToString(rs[index], locale);
			}

			// The check of the Length is a workaround see NH-2340
			if (strValue.Length > 0)
			{
				return strValue[0];
			}

			return '\0'; // This line should never be executed
		}

		public override System.Type PrimitiveClass => typeof(char);

		public override System.Type ReturnedClass => typeof(char);

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = Convert.ToChar(value);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + value.ToString() + '\'';
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public virtual object StringToObject(string xml)
		{
			if (xml.Length != 1)
				throw new MappingException("multiple or zero characters found parsing string");

			return xml[0];
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return xml[0];
		}
	}
}
