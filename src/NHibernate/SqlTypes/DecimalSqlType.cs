using System;
using System.Data;

namespace NHibernate.SqlTypes {

	/// <summary>
	/// Summary description for DecimalSqlType.
	/// </summary>
	public class DecimalSqlType : SqlType {

		public DecimalSqlType(byte precision, byte scale) : base(DbType.Decimal, precision, scale) {
		}

	}
}
