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

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return (string)val;
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return xml;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(string); }
		}

		#region IIdentifierType Members

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
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
