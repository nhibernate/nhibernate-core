using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.StringFixedLength"/> with the 
	/// information required to to generate an <see cref="DbParameter"/>.
	/// </summary>
	/// <remarks>
	/// This can store the length of the string that the <see cref="DbParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="DbParameter"/> correctly.
	/// </remarks>
	[Serializable]
	public class StringFixedLengthSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringFixedLengthSqlType"/> class.
		/// </summary>
		public StringFixedLengthSqlType() : base(DbType.StringFixedLength)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringFixedLengthSqlType"/> class.
		/// </summary>
		/// <param name="length">The length of the string the <see cref="DbParameter"/> should hold.</param>
		public StringFixedLengthSqlType(int length) : base(DbType.StringFixedLength, length)
		{
		}
	}
}