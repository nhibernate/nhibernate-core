using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Single"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Single"/>.
	/// </remarks>
	[Serializable]
	public class SingleSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SingleSqlType"/> class.
		/// </summary>
		public SingleSqlType() : base( DbType.Single )
		{
		}
	}
}