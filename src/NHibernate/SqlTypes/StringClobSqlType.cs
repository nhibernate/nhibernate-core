using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.String"/> that is stored in
	/// a CLOB column with the information required to generate 
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
	/// StringSqlType would work just fine.
	/// </p>
	/// </remarks>
	[Serializable]
	public class StringClobSqlType : StringSqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringClobSqlType"/> class.
		/// </summary>
		public StringClobSqlType() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringClobSqlType"/> class.
		/// </summary>
		/// <param name="length">The length of the string the <see cref="IDbDataParameter"/> should hold.</param>
		public StringClobSqlType(int length) : base(length)
		{
		}
	}
}