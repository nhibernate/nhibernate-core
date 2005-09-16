using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.UInt16"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.UInt16"/>.
	/// </remarks>
	[Serializable]
	public class UInt16SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UInt16SqlType"/> class.
		/// </summary>
		public UInt16SqlType() : base( DbType.UInt16 )
		{
		}
	}
}