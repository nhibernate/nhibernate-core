using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// A SqlType that uses a <see cref="DbType.String "/> to generate a Parameter
	/// for the IDriver to write a CLOB value to the database.
	/// </summary>
	/// <remarks>
	/// This is only needed by DataProviders (SqlClient) that need to specify a Size for the
	/// IDbDataParameter.  Most DataProvider(Oralce) don't need to set the Size so a 
	/// StringSqlType would work just fine.
	/// </remarks>
	[Serializable]
	public class StringClobSqlType : StringSqlType 
	{
		
		public StringClobSqlType() : base() 
		{
		}
		public StringClobSqlType(int length) : base(length) 
		{
		}
	}
}