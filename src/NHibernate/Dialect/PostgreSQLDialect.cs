using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	///  An SQL dialect for PostgreSQL.
	/// </summary>
	public class PostgreSQLDialect : Dialect {

		public PostgreSQLDialect()	{

			/* Java mapping was:
			Types.BIT, "BOOL" );
			Types.BIGINT, "INT8" );
			Types.SMALLINT, "INT2" );
			Types.TINYINT, "INT2" );
			Types.INTEGER, "INT4" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($l)"
			Types.FLOAT, "FLOAT4" );
			Types.DOUBLE, "FLOAT8" );
			Types.DATE, "DATE" );
			Types.TIME, "TIME" );
			Types.TIMESTAMP, "TIMESTAMP"
			Types.VARBINARY, "BYTEA" );
			Types.CLOB, "TEXT" );
			Types.BLOB, "BYTEA" );
			Types.NUMERIC, "NUMERIC" );
			*/

			Register( DbType.Boolean, "BOOL" );
			Register( DbType.Int64, "INT8" );
			Register( DbType.Int16, "INT2" );
			Register( DbType.Int32, "INT4" );
			Register( DbType.AnsiStringFixedLength, "CHAR(1)" );
			Register( DbType.AnsiString, "VARCHAR($l)" );
			Register( DbType.Single, "FLOAT4" );
			Register( DbType.Double, "FLOAT8" );
			Register( DbType.DateTime, "DATE" );
			Register( DbType.Time, "TIMESTAMP" );
			Register( DbType.Binary, "BYTEA" );
			Register( DbType.AnsiString, "TEXT" );
			//Register( DbType.Image, "BYTEA" );
			Register( DbType.Decimal, "NUMERIC" );
		
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			*/
		}

		public override string AddColumnString {
			get { return "add column"; }
		}
		public override bool DropConstraints {
			get { return false; }
		}
		public override string GetSequenceNextValString(string sequenceName) {
			return string.Concat( "select nextval ('", sequenceName, "')" );
		}
		public override string GetCreateSequenceString(string sequenceName) {
			return "create sequence " + sequenceName;
		}
		public override string GetDropSequenceString(string sequenceName) {
			return "drop sequence " + sequenceName;
		}
	
		public override string CascadeConstraintsString {
			get { return " cascade"; }
		}
	
		public override bool SupportsSequences {
			get { return true; }
		}
	}
}