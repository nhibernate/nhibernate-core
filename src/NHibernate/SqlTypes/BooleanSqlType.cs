using System;
using System.Data;

namespace NHibernate.SqlTypes {
	/// <summary>
	/// Summary description for BooleanSqlType.
	/// </summary>
	public class BooleanSqlType : SqlType {
		
		public BooleanSqlType() : base(DbType.Boolean){
		}
	}
}
