using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect {

		public GenericDialect() : base() {
			Register( DbType.Boolean, "BIT" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.String, "VARCHAR($1)" );
			Register( DbType.Single, "FLOAT" );
			Register( DbType.DateTime, "DATETIME" );
			Register( DbType.Time, "TIMESTAMP" );
			Register( DbType.Binary, "VARBINARY($1)" );
			Register( DbType.Decimal, "DECIMAL" );
		}

		public override string AddColumnString {
			get { return "add column"; }
		}
		
	}
}
