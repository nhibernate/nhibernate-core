using System;
using System.Data;

namespace NHibernate.SqlTypes {

	/// <summary>
	/// Summary description for CurrencySqlType.
	/// 
	/// TODO: determine if I want to remove this.  There is no native .NET Currency class
	/// and the sql server money = decimal(19,4) and smallmoney = decimal(10,4).  So there
	/// is no advantage to using it.
	/// </summary>
	[Serializable]
	public class CurrencySqlType : SqlType
	{
		public CurrencySqlType() : base(DbType.Currency) 
		{
		}
	}
}
