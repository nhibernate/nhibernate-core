using System;
using System.Data;

namespace NHibernate.SqlTypes 
{
	/// <summary>
	/// Summary description for DoubleSqlType.
	/// </summary>
	public class DoubleSqlType : SqlType 
	{
		public DoubleSqlType() : base(DbType.Double)
		{
		}
	}
}