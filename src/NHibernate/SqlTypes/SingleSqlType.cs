using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for SingleSqlType.
	/// </summary>
	[Serializable]
	public class SingleSqlType : SqlType 
	{
		public SingleSqlType() : base(DbType.Single)
		{
		}
	}
}
