using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Single" /> Property to an 
	/// <see cref="DbType.Single" /> column.
	/// </summary>
	/// <remarks>
	/// Verify through your database's documentation if there is a column type that
	/// matches up with the capabilities of <see cref="System.Single" />  
	/// </remarks>
	[Serializable]
	public class SingleType : PrimitiveType
	{
		/// <summary></summary>
		public SingleType() : base(SqlTypeFactory.Single)
		{
		}

		public SingleType(SqlType sqlType) : base(sqlType) {}

		/// <summary></summary>
		public override string Name
		{
			get { return "Single"; }
		}

		private static readonly Single ZERO = 0;
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return Convert.ToSingle(rs[index]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			try
			{
				return Convert.ToSingle(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Single); }
		}

		public override void Set(DbCommand rs, object value, int index, ISessionImplementor session)
		{
			rs.Parameters[index].Value = value;
		}

		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return Single.Parse(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(Single); }
		}

		public override object DefaultValue
		{
			get { return ZERO; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}