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

			Register( SqlDbType.Bit, "BOOL" );
			Register( SqlDbType.BigInt, "INT8" );
			Register( SqlDbType.SmallInt, "INT2" );
			Register( SqlDbType.TinyInt, "INT2" );
			Register( SqlDbType.Int, "INT4" );
			Register( SqlDbType.Char, "CHAR(1)" );
			Register( SqlDbType.VarChar, "VARCHAR($l)" );
			Register( SqlDbType.Float, "FLOAT4" );
			Register( SqlDbType.Real, "FLOAT8" );
			Register( SqlDbType.DateTime, "DATE" );
			Register( SqlDbType.Timestamp, "TIMESTAMP" );
			Register( SqlDbType.VarBinary, "BYTEA" );
			Register( SqlDbType.Text, "TEXT" );
			Register( SqlDbType.Image, "BYTEA" );
			Register( SqlDbType.Decimal, "NUMERIC" );
		
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
	
		public string CascadeConstraintsString {
			get { return " cascade"; }
		}
	
		public override bool SupportsSequences {
			get { return true; }
		}
	}
}