using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Summary description for DateSqlType.
	/// </summary>
	[Serializable]
	public class DateSqlType : SqlType
	{
		public DateSqlType() : base(DbType.Date)
		{
		}
	}
}
