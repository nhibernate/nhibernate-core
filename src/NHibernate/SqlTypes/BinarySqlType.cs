using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// Summary description for BinarySqlType.
	/// </summary>
	public class BinarySqlType : SqlType 
	{
		
		public BinarySqlType() : base (DbType.Binary) 
		{
		}
		public BinarySqlType(int length) : base (DbType.Binary, length) 
		{
		}
	}
}
