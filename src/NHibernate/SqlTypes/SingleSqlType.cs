using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for SingleSqlType.
	/// </summary>
	public class SingleSqlType : SqlType 
	{
		public SingleSqlType() : base(DbType.Single)
		{
		}
	}
}
