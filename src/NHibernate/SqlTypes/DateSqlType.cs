using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Date"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Date"/>.
	/// </remarks>
	[Serializable]
	public class DateSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateSqlType"/> class.
		/// </summary>
		public DateSqlType() : base( DbType.Date )
		{
		}
	}
}