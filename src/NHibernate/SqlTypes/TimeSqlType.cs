using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Summary description for TimeSqlType.
	/// </summary>
	[Serializable]
	public class TimeSqlType : SqlType
	{
		public TimeSqlType() : base(DbType.Time)
		{
		}
	}
}