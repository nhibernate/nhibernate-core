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
	public class CultureInfoType : ImmutableType, ILiteralType
	{
		/// <summary></summary>
		internal CultureInfoType() : base(new StringSqlType(5))
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			string str = (string) NHibernateUtil.String.Get(rs, index, session);
			if (str == null)
			{
				return null;
			}
			else
			{
				return new CultureInfo(str);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			NHibernateUtil.String.Set(cmd, ((CultureInfo) value).Name, index, session);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToString(object value)
		{
			return ((CultureInfo) value).Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
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
	}
}