using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for Int64SqlType.
	/// </summary>
	public class Int64SqlType : SqlType
	{
		public Int64SqlType(): base(DbType.Int64) 
		{
		}
	}
}