using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.AnsiStringFixedLength"/> column.
	/// </summary>
	[Serializable]
	public abstract class CharBooleanType : BooleanType
	{
		/// <summary></summary>
		protected abstract string TrueString { get; }

		/// <summary></summary>
		protected abstract string FalseString { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlType"></param>
		protected CharBooleanType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			string code = Convert.ToString(rs[index]);
			if (code == null)
			{
				return null;
			}
			else
			{
				return StringHelper.EqualsCaseInsensitive(code, TrueString);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override void Set(DbCommand cmd, Object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = ToCharacter(value);
		}

		private string ToCharacter(object value)
		{
			return ((bool) value) ? TrueString : FalseString;
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ToCharacter(value) + "'";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object StringToObject(String xml)
		{
			if (StringHelper.EqualsCaseInsensitive(TrueString, xml))
			{
				return true;
			}
			else if (StringHelper.EqualsCaseInsensitive(FalseString, xml))
			{
				return false;
			}
			else
			{
				throw new HibernateException("Could not interpret: " + xml);
			}
		}
	}
}