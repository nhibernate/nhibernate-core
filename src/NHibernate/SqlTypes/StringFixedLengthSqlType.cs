using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for StringFixedLengthSqlType.
	/// </summary>
	public class StringFixedLengthSqlType : SqlType 
	{
		public StringFixedLengthSqlType() : base(DbType.StringFixedLength)
		{	
		}
		
		public StringFixedLengthSqlType(int length) : base(DbType.StringFixedLength, length) 
		{	
		}

	}
}

