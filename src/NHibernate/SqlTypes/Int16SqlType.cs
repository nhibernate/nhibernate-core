using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for Int16SqlType.
	/// </summary>
	[Serializable]
	public class Int16SqlType : SqlType
	{
		public Int16SqlType(): base(DbType.Int16) 
		{
		}
	}
}
