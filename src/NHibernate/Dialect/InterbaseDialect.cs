using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect for Interbase.
	/// </summary>
	public class InterbaseDialect {

		public InterbaseDialect() : base() {

			/* Java mapping was:
			
			Types.BIT, "SMALLINT" );
			Types.BIGINT, "NUMERIC(18,0)" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "SMALLINT" );
			Types.INTEGER, "INTEGER" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($l)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATE" );
			Types.TIME, "TIME" );
			Types.TIMESTAMP, "TIMESTAMP" );
			Types.VARBINARY, "BLOB" );
			Types.NUMERIC, "NUMERIC(18, $l)"
			Types.BLOB, "BLOB" );
			Types.CLOB, "BLOB SUB_TYPE 1" );
			*/

			Register( SqlDbType.Bit, "SMALLINT" );
			Register( SqlDbType.BigInt, "NUMERIC(18,0)" );
			Register( SqlDbType.SmallInt, "SMALLINT" );
			Register( SqlDbType.TinyInt, "SMALLINT" );
			Register( SqlDbType.Int, "INTEGER" );
			Register( SqlDbType.Char, "CHAR(1)" );
			Register( SqlDbType.VarChar, "VARCHAR($l)" );
			Register( SqlDbType.Float, "FLOAT" );
			Register( SqlDbType.Real, "DOUBLE PRECISION" );
			// Register( SqlDbType.DATE, "DATE" );  ???
			// Register( SqlDbType.TIME, "TIME" );  ???
			Register( SqlDbType.Timestamp, "TIMESTAMP" );
			Register( SqlDbType.VarBinary, "BLOB" );
			Register( SqlDbType.Decimal, "NUMERIC(18, $l)" );
			Register( SqlDbType.Image, "BLOB" );
			Register( SqlDbType.Text, "BLOB SUB_TYPE 1" );
		
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
			getDefaultProperties().setProperty(Environment.STATEMENT_CACHE_SIZE, "0");
			*/
		}

		public string GetAddColumnString() {
			return "add";
		}
		public string GetSequenceNextValString(string sequenceName) {
			return string.Concat( "select gen_id( ", sequenceName, ", 1 ) from RDB$DATABASE" );
		}
		public string GetCreateSequenceString(string sequenceName) {
			return "create generator " + sequenceName;
		}
		public string GetDropSequenceString(string sequenceName) {
			return string.Concat("delete from RDB$GENERATORS where RDB$GENERATOR_NAME = '", sequenceName.ToUpper(), "'" );
		}
	
		public bool SupportsSequences() {
			return true;
		}
	}
}