using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.DateTimeOffset"/>.
	/// </summary>
	[Serializable]
	public class DateTimeOffsetSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeOffsetSqlType"/> class.
		/// </summary>
		public DateTimeOffsetSqlType() : base(DbType.DateTimeOffset)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeOffsetSqlType"/> class.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The number of digit below seconds.</param>
		public DateTimeOffsetSqlType(byte fractionalSecondsPrecision) : base(DbType.DateTimeOffset, fractionalSecondsPrecision)
		{
		}
	}
}
