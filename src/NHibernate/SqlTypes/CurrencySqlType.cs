using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Describes the details of a <see cref="DbType.Currency"/> with the 
	/// information required to generate an <see cref="IDbDataParameter"/>.
	/// </summary>
	/// <remarks>
	/// There is not any extra information needed for a <see cref="DbType.Currency"/>.
	/// </remarks>
	[Serializable]
	public class CurrencySqlType : SqlType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CurrencySqlType"/> class.
		/// </summary>
		public CurrencySqlType() : base( DbType.Currency )
		{
		}
	}
}