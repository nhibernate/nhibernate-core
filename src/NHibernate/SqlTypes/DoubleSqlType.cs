using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Double"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Double"/>.
	/// </remarks>
	[Serializable]
	public class DoubleSqlType : SqlType 
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleSqlType"/> class.
		/// </summary>
		public DoubleSqlType() : base(DbType.Double)
		{
		}
	}
}