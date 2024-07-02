using System;
using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine;
using NHibernate.SqlTypes;

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
			if (!rs.TryGetString(index, out var dbValue))
			{
				var locale = session.Factory.Settings.Locale;

				dbValue = Convert.ToString(rs[index], locale);
			}

			if (dbValue == null)
			{
				return null;
			}
			else
			{
				return GetBooleanAsObject(dbValue.Equals(TrueString, StringComparison.InvariantCultureIgnoreCase));
			}
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

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public override object StringToObject(String xml)
		{
			if (string.Equals(TrueString, xml, StringComparison.InvariantCultureIgnoreCase))
			{
				return TrueObject;
			}
			else if (string.Equals(FalseString, xml, StringComparison.InvariantCultureIgnoreCase))
			{
				return FalseObject;
			}
			else
			{
				throw new HibernateException("Could not interpret: " + xml);
			}
		}
	}
}
