using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Int32"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Int32"/>.
	/// </remarks>
	[Serializable]
	public class Int32SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Int32SqlType"/> class.
		/// </summary>
		public Int32SqlType() : base( DbType.Int32 )
		{
		}

	}
}