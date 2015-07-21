using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to a <see cref="DbType.DateTime"/>
	/// </summary>
	[Serializable]
	public class DateTime2Type : DateTimeType
	{
		/// <summary></summary>
		internal DateTime2Type() : base(SqlTypeFactory.DateTime2)
		{
		}

		public override string Name
		{
			get { return "DateTime2"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				return Convert.ToDateTime(rs[index]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override void Set(IDbCommand st, object value, int index)
		{
			((IDataParameter) st.Parameters[index]).Value = (DateTime) value;
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			return x.Equals(y);
		}

		public override object Next(object current, Engine.ISessionImplementor session)
		{
			return Seed(session);
		}

		public override object Seed(Engine.ISessionImplementor session)
		{
			return DateTime.Now;
		}
	}
}