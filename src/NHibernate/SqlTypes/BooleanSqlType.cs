using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Boolean"/> with the 
	/// information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Boolean"/>.
	/// </remarks>
	[Serializable]
	public class BooleanSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanSqlType"/> class.
		/// </summary>
		public BooleanSqlType() : base( DbType.Boolean )
		{
		}
	}
}