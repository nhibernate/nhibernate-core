using System;
using System.Data;

namespace NHibernate.SqlTypes {
	
	/// <summary>
	/// Summary description for StringSqlType.
	/// </summary>
	public class StringSqlType : SqlType {
		public StringSqlType(int length) : base(DbType.String, length) {	
		}
	}
}
