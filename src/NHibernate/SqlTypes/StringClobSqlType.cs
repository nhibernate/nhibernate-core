using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.String"/> that is stored in
	/// a CLOB column with the information required to generate 
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
	/// StringSqlType would work just fine.
	/// </p>
	/// </remarks>
	[Serializable]
	public class StringClobSqlType : StringSqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringClobSqlType"/> class.
		/// </summary>
		public StringClobSqlType()
			: base(int.MaxValue / 2)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringClobSqlType"/> class.
		/// </summary>
		/// <param name="length">The length of the string the <see cref="DbParameter"/> should hold.</param>
		public StringClobSqlType(int length) : base(length)
		{
		}
	}
}