using System;
using System.Data;

namespace NHibernate.SqlTypes {

	/// <summary>
	/// Summary description for DoubleSqlType.
	/// </summary>
	public class DoubleSqlType : SqlType {

		public DoubleSqlType(int length) : base(DbType.Double, length) {
		}
	}
}