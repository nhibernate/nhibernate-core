using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Decimal"/> with the 
	/// information required to to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// This can store the precision &amp; scale of the decimal value that the 
	/// <see cref="IDbDataParameter"/> can hold. If no value is provided for the 
	/// precision &amp; scale then the <c>Driver</c> is responsible for 
	/// setting the properties on the <see cref="IDbDataParameter"/> correctly.
	/// </remarks>
	[Serializable]
	public class DecimalSqlType : SqlType 
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalSqlType"/> class.
		/// </summary>
		public DecimalSqlType() : base(DbType.Decimal) 
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalSqlType"/> class.
		/// </summary>
		/// <param name="precision">The precision of the Decimal the <see cref="IDbDataParameter"/> should hold.</param>
		/// <param name="scale">The scale of the Decimal the <see cref="IDbDataParameter"/> should hold.</param>
		public DecimalSqlType(byte precision, byte scale) : base(DbType.Decimal, precision, scale) 
		{
		}
	}
}
