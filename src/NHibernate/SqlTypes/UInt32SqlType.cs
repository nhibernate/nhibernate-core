using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.UInt32"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.UInt32"/>.
	/// </remarks>
	[Serializable]
	public class UInt32SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UInt32SqlType"/> class.
		/// </summary>
		public UInt32SqlType() : base( DbType.UInt32 )
		{
		}
	}
}