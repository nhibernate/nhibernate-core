using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for AnsiStringFixedLengthSqlType.
	/// </summary>
	public class AnsiStringFixedLengthSqlType : SqlType 
	{
		public AnsiStringFixedLengthSqlType() 
			: base(DbType.AnsiStringFixedLength)
		{
		}

		public AnsiStringFixedLengthSqlType(int length) : base(DbType.AnsiStringFixedLength, length) {	
		}

	}
}
