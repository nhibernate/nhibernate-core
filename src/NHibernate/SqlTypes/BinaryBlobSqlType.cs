using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Binary"/> that is stored in
	/// a BLOB column with the information required to generate 
	/// an <see cref="DbParameter"/>.
	/// </summary>
	/// <remarks>
	/// <p>
	/// This can store the length of the binary data that the <see cref="DbParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="DbParameter"/> correctly.
	/// </p>
	/// <p>
	/// This is only needed by DataProviders (SqlClient) that need to specify a Size for the
	/// DbParameter.  Most DataProvider(Oralce) don't need to set the Size so a 
	/// BinarySqlType would work just fine.
	/// </p>
	/// </remarks>
	[Serializable]
	public class BinaryBlobSqlType : BinarySqlType
	{
		public BinaryBlobSqlType(int length) : base(length) {}
		public BinaryBlobSqlType() {}
	}
}