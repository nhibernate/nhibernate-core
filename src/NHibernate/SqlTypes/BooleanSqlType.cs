using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for BooleanSqlType.
	/// </summary>
	[Serializable]
	public class BooleanSqlType : SqlType 
	{
		public BooleanSqlType() : base(DbType.Boolean)
		{
		}
	}
}
