using System;
using System.Data;

namespace NHibernate.SqlTypes {
	
	/// <summary>
	/// Summary description for StringFixedLengthSqlType.
	/// </summary>
	public class StringFixedLengthSqlType : SqlType {
		
		public StringFixedLengthSqlType(int length) : base(DbType.StringFixedLength, length) {	
		}

	}
}

