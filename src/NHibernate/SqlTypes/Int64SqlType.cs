using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Int64"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Int64"/>.
	/// </remarks>
	[Serializable]
	public class Int64SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Int64SqlType"/> class.
		/// </summary>
		public Int64SqlType() : base( DbType.Int64 )
		{
		}
	}
}