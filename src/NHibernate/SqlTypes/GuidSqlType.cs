using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Guid"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Guid"/>.
	/// </remarks>
	[Serializable]
	public class GuidSqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GuidSqlType"/> class.
		/// </summary>
		public GuidSqlType() : base(DbType.Guid)
		{
		}
	}
}
