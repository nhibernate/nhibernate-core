using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for DoubleSqlType.
	/// </summary>
	[Serializable]
	public class DoubleSqlType : SqlType 
	{
		public DoubleSqlType() : base(DbType.Double)
		{
		}
	}
}