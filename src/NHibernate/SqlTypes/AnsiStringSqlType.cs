using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	
	/// <summary>
	/// Summary description for AnsiStringSqlType.
	/// </summary>
	public class AnsiStringSqlType : SqlType 
	{
		public AnsiStringSqlType() : base(DbType.AnsiString) 
		{
		}
		public AnsiStringSqlType(int length) : base(DbType.AnsiString, length) 
		{	
		}


	}
}
