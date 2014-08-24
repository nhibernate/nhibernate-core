using System;
using System.Data;
using System.Globalization;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			string str = (string) NHibernateUtil.String.Get(rs, index);
			if (str == null)
			{
				return null;
			}
			else
			{
				return new CultureInfo(str);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand cmd, object value, int index)
		{
			NHibernateUtil.String.Set(cmd, ((CultureInfo) value).Name, index);
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