using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.AnsiString"/> with the 
	/// information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// This can store the length of the string that the <see cref="IDbDataParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="IDbDataParameter"/> correctly.
	/// </remarks>
	[Serializable]
	public class AnsiStringSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AnsiStringSqlType"/> class.
		/// </summary>
		public AnsiStringSqlType() : base(DbType.AnsiString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AnsiStringSqlType"/> class.
		/// </summary>
		/// <param name="length">The length of the string the <see cref="IDbDataParameter"/> should hold.</param>
		public AnsiStringSqlType(int length) : base(DbType.AnsiString, length)
		{
		}
	}
}