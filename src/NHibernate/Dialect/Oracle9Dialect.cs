using System;
using System.Text;
using System.Data;

using NHibernate.Util;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect for Oracle 9 (uses ANSI-style syntax where possible).
	/// </summary>
	public class Oracle9Dialect : Dialect {

		public Oracle9Dialect() : base() {
			/* Type mapping in Java was:
			  
			   Types.BIT, "NUMBER(1,0)"
			   Types.BIGINT, "NUMBER(19,0)"
			   Types.SMALLINT, "NUMBER(5,0)"
			   Types.TINYINT, "NUMBER(3,0)"
			   Types.INTEGER, "NUMBER(10,0)"
			   Types.CHAR, "CHAR(1)"
			   Types.VARCHAR, "VARCHAR2($l)"
			   Types.FLOAT, "FLOAT"
			   Types.DOUBLE, "DOUBLE PRECISION"
			   Types.DATE, "DATE"
			   Types.TIME, "DATE"
			   Types.TIMESTAMP, "DATE"
			   Types.VARBINARY, "RAW($l)"
			   Types.NUMERIC, "NUMBER(19, $l)" 
			   Types.BLOB, "BLOB"
			   Types.CLOB, "CLOB"
			*/
			
			Register( SqlDbType.Bit, "NUMBER(1,0)" );
			Register( SqlDbType.BigInt, "NUMBER(19,0)" );
			Register( SqlDbType.SmallInt, "NUMBER(5,0)" );
			Register( SqlDbType.TinyInt, "NUMBER(3,0)" );
			Register( SqlDbType.Int, "NUMBER(10,0)" );
			Register( SqlDbType.Char, "CHAR(1)" );
			Register( SqlDbType.VarChar, "VARCHAR2($l)" );
			Register( SqlDbType.Float, "FLOAT" );
			Register( SqlDbType.Real, "DOUBLE PRECISION" );
			Register( SqlDbType.DateTime, "DATE" );
			Register( SqlDbType.SmallDateTime, "DATE" );
			Register( SqlDbType.Timestamp, "DATE" );
			Register( SqlDbType.VarBinary, "RAW($l)" );
			Register( SqlDbType.Decimal, "NUMBER(19, $l)" );
			Register( SqlDbType.Image, "BLOB" );
			Register( SqlDbType.Text, "CLOB" );

			/* TODO:
			getDefaultProperties().setProperty(Environment.USE_STREAMS_FOR_BINARY, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			*/
		}
	
		public override string AddColumnString {
			get { return "add"; }
		}
	
		public override string GetSequenceNextValString(string sequenceName) {
			return  string.Concat( "select ", sequenceName, ".nextval from dual" );
		}
		public override string GetCreateSequenceString(string sequenceName) {
			return "create sequence " + sequenceName;
		}
		public override string GetDropSequenceString(string sequenceName) {
			return "drop sequence " + sequenceName;
		}
	
		public string CascadeConstraintsString {
			get { return " cascade constraints"; }
		}
	
		public bool SupportsForUpdateNowait {
			get { return true; }
		}
	
		public override bool SupportsSequences {
			get { return true; }
		}
	}
}
