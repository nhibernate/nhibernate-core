using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Binary"/> that is stored in
	/// a BLOB column with the information required to generate 
	/// an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// <p>
	/// This can store the length of the binary data that the <see cref="IDbDataParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="IDbDataParameter"/> correctly.
	/// </p>
	/// <p>
	/// This is only needed by DataProviders (SqlClient) that need to specify a Size for the
	/// IDbDataParameter.  Most DataProvider(Oralce) don't need to set the Size so a 
	/// BinarySqlType would work just fine.
	/// </p>
	/// </remarks>
	[Serializable]
	public class BinaryBlobSqlType : BinarySqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BinaryBlobSqlType"/> class.
		/// </summary>
		public BinaryBlobSqlType() : base()
		{
		}
	}
}