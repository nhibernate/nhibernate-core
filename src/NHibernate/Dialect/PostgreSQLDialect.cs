using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	///  An SQL dialect for PostgreSQL.
	/// </summary>
	public class PostgreSQLDialect {

		public PostgreSQLDialect() : base()	{

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
			Register( SqlDbType.DATE, "DATE" );
			Register( SqlDbType.TIME, "TIME" );
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

		public string GetAddColumnString() {
			return "add column";
		}
		public bool DropConstraints() {
			return false;
		}
		public string GetSequenceNextValString(string sequenceName) {
			return string.Concat( "select nextval ('", sequenceName, "')" );
		}
		public string GetCreateSequenceString(string sequenceName) {
			return "create sequence " + sequenceName;
		}
		public string GetDropSequenceString(string sequenceName) {
			return "drop sequence " + sequenceName;
		}
	
		public string GetCascadeConstraintsString() {
			return " cascade";
		}
	
		public bool SupportsSequences() {
			return true;
		}
	}
}