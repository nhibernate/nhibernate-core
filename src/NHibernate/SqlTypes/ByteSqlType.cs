using System;
using System.Data;

namespace NHibernate.SqlTypes {
	
	/// <summary>
	/// Summary description for ByteSqlType.
	/// </summary>
	[Serializable]
	public class ByteSqlType : SqlType
	{
		public ByteSqlType() : base(DbType.Byte){
		}
	}
}
