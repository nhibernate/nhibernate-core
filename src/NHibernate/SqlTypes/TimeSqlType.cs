using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Time"/>.
	/// </summary>
	[Serializable]
	public class TimeSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSqlType"/> class.
		/// </summary>
		public TimeSqlType() : base(DbType.Time)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSqlType"/> class.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The number of digit below seconds.</param>
		public TimeSqlType(byte fractionalSecondsPrecision) : base(DbType.Time, fractionalSecondsPrecision)
		{
		}
	}
}
