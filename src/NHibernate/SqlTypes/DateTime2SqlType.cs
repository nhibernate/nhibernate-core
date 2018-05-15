using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.DateTime2"/>.
	/// </summary>
	[Serializable]
	public class DateTime2SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTime2SqlType"/> class.
		/// </summary>
		public DateTime2SqlType() : base(DbType.DateTime2)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTime2SqlType"/> class.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The number of digit below seconds.</param>
		public DateTime2SqlType(byte fractionalSecondsPrecision) : base(DbType.DateTime2, fractionalSecondsPrecision)
		{
		}
	}
}
