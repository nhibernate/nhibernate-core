using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect for Interbase.
	/// </summary>
	public class InterbaseDialect : Dialect {

		public InterbaseDialect() {

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
		
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
			getDefaultProperties().setProperty(Environment.STATEMENT_CACHE_SIZE, "0");
			*/
		}

		public override string AddColumnString {
			get { return "add"; }
		}
		public override string GetSequenceNextValString(string sequenceName) {
			return string.Concat( "select gen_id( ", sequenceName, ", 1 ) from RDB$DATABASE" );
		}
		public override string GetCreateSequenceString(string sequenceName) {
			return "create generator " + sequenceName;
		}
		public override string GetDropSequenceString(string sequenceName) {
			return string.Concat("delete from RDB$GENERATORS where RDB$GENERATOR_NAME = '", sequenceName.ToUpper(), "'" );
		}
	
		public override bool SupportsSequences {
			get { return true; }
		}
	}
}