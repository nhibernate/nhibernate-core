using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect {

		public GenericDialect() {
			Register( SqlDbType.Bit, "BIT" );
			Register( SqlDbType.BigInt, "BIGINT" );
			Register( SqlDbType.SmallInt, "SMALLINT" );
			Register( SqlDbType.TinyInt, "TINYINT" );
			Register( SqlDbType.Int, "INTEGER" );
			Register( SqlDbType.Char, "CHAR(1)" );
			Register( SqlDbType.VarChar, "VARCHAR($1)" );
			Register( SqlDbType.Float, "FLOAT" );
			Register( SqlDbType.DateTime, "DATE" );
			Register( SqlDbType.Timestamp, "TIMESTAMP" );
			Register( SqlDbType.VarBinary, "VARBINARY($1)" );
			Register( SqlDbType.Decimal, "DECIMAL" );
		}

		public override string AddColumnString {
			get { return "add column"; }
		}
		
	}
}
