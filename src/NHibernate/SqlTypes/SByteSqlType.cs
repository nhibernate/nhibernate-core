using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.SByte"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.SByte"/>.
	/// </remarks>
	[Serializable]
	public class SByteSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SByteSqlType"/> class.
		/// </summary>
		public SByteSqlType() : base(DbType.SByte)
		{
		}
	}
}

