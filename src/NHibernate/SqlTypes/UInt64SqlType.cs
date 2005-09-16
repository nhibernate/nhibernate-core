using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.UInt64"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.UInt64"/>.
	/// </remarks>
	[Serializable]
	public class UInt64SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UInt64SqlType"/> class.
		/// </summary>
		public UInt64SqlType() : base( DbType.UInt64 )
		{
		}
	}
}