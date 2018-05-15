using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.DateTime"/>.
	/// </summary>
	[Serializable]
	public class DateTimeSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeSqlType"/> class.
		/// </summary>
		public DateTimeSqlType() : base(DbType.DateTime)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeSqlType"/> class.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The number of digit below seconds.</param>
		public DateTimeSqlType(byte fractionalSecondsPrecision) : base(DbType.DateTime, fractionalSecondsPrecision)
		{
		}
	}
}
