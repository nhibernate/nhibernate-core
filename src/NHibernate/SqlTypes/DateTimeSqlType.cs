using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.DateTime"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.DateTime"/>.
	/// </remarks>
	[Serializable]
	public class DateTimeSqlType : SqlType 	
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeSqlType"/> class.
		/// </summary>
		public DateTimeSqlType() : base(DbType.DateTime) 
		{
		}
	}
}
