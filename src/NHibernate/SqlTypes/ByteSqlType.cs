using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Byte"/> with the 
	/// information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Byte"/>.
	/// </remarks>
	[Serializable]
	public class ByteSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ByteSqlType"/> class.
		/// </summary>
		public ByteSqlType() : base( DbType.Byte )
		{
		}
	}
}