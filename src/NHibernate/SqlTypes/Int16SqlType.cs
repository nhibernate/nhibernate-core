using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Int16"/> with the 
	/// extra information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Int16"/>.
	/// </remarks>
	[Serializable]
	public class Int16SqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Int16SqlType"/> class.
		/// </summary>
		public Int16SqlType(): base(DbType.Int16) 
		{
		}
	}
}
