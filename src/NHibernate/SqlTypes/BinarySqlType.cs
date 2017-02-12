using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Binary"/> with the 
	/// information required to to generate an <see cref="DbParameter"/>.
	/// </summary>
	/// <remarks>
	/// This can store the binary data that the <see cref="DbParameter"/> can hold.
	/// If no value is provided for the length then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="DbParameter"/> correctly.
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
		/// <param name="length">The length of the binary data the <see cref="DbParameter"/> should hold</param>
		public BinarySqlType(int length) : base(DbType.Binary, length)
		{
		}
	}
}