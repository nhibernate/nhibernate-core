using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// A SqlType that uses a <see cref="DbType.Binary"/> to generate a Parameter
	/// for the IDriver to write a BLOB value to the database.
	/// </summary>
	/// <remarks>
	/// This is only needed by DataProviders (SqlClient) that need to specify a Size for the
	/// IDbDataParameter.  Most DataProvider(Oralce) don't need to set the Size so a 
	/// BinarySqlType would work just fine.
	/// </remarks>
	[Serializable]
	public class BinaryBlobSqlType : BinarySqlType 
	{
		
		public BinaryBlobSqlType() : base() 
		{
		}
		public BinaryBlobSqlType(int length) : base(length) 
		{
		}
	}
}