using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Time"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Time"/>.
	/// </remarks>
	[Serializable]
	public class TimeSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSqlType"/> class.
		/// </summary>
		public TimeSqlType() : base( DbType.Time )
		{
		}
	}
}