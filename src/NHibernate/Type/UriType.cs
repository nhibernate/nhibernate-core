using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public partial class UriType : ImmutableType, IDiscriminatorType
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

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			return new Uri(xml, UriKind.RelativeOrAbsolute);
		}

		private static string GetStringRepresentation(object value)
		{
			return ((Uri) value).OriginalString;
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value =
				// 6.0 TODO: inline the call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return StringToObject(Convert.ToString(rs[index]));
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return StringToObject(Convert.ToString(rs[name]));
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
			return GetStringRepresentation(val);
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return StringToObject(xml);
		}

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((Uri) value).OriginalString + "'";
		}

		/// <inheritdoc />
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			var str = cached as string;
			return str == null ? null : StringToObject(cached as string);
		}

		/// <inheritdoc />
		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return value == null ? null : GetStringRepresentation(value);
		}
	}
}
