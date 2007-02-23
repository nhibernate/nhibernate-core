using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Binary"/> with the 
	/// information required to to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// This can store the binary data that the <see cref="IDbDataParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="IDbDataParameter"/> correctly.
	/// </remarks>
	[Serializable]
	public class BinarySqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BinarySqlType"/> class.
		/// </summary>
		public BinarySqlType() : base(DbType.Binary)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BinarySqlType"/> class.
		/// </summary>
		/// <param name="length">The length of the binary data the <see cref="IDbDataParameter"/> should hold</param>
		public BinarySqlType(int length) : base(DbType.Binary, length)
		{
		}
	}
}