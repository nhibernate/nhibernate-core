using System;
using System.Data;

namespace NHibernate.SqlTypes {
	/// <summary>
	/// Summary description for DateSqlType.
	/// </summary>
	public class DateTimeSqlType : SqlType 	{
		
		public DateTimeSqlType() : base(DbType.DateTime) {
		}
	}
}
