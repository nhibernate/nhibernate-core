using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Globalization.CultureInfo"/> Property 
	/// to a <see cref="DbType.String"/> column.
	/// </summary>
	/// <remarks>
	/// CultureInfoType stores the culture name (not the Culture ID) of the 
	/// <see cref="System.Globalization.CultureInfo"/> in the DB.
	/// </remarks>
	[Serializable]
	public partial class CultureInfoType : ImmutableType, ILiteralType
	{
		internal CultureInfoType() : base(new StringSqlType(5))
		{
		}

		/// <inheritdoc />
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return ParseStringRepresentation(NHibernateUtil.String.Get(rs, index, session));
		}

		/// <inheritdoc />
		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		/// <inheritdoc />
		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			NHibernateUtil.String.Set(cmd, GetStringRepresentation(value), index, session);
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
		public override string ToString(object value)
		{
			return GetStringRepresentation(value);
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return CultureInfo.CreateSpecificCulture(xml);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(CultureInfo); }
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "CultureInfo"; }
		}

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return ((ILiteralType) NHibernateUtil.String).ObjectToSQLString(value.ToString(), dialect);
		}

		/// <inheritdoc />
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return ParseStringRepresentation(cached);
		}

		/// <inheritdoc />
		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return GetStringRepresentation(value);
		}

		private string GetStringRepresentation(object value)
		{
			return ((CultureInfo) value)?.Name;
		}

		private static object ParseStringRepresentation(object value)
		{
			var str = value as string;
			return str == null ? null : new CultureInfo(str);
		}
	}
}
