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
			
			Register( DbType.Bit, "NUMBER(1,0)" );
			Register( DbType.BigInt, "NUMBER(19,0)" );
			Register( DbType.SmallInt, "NUMBER(5,0)" );
			Register( DbType.TinyInt, "NUMBER(3,0)" );
			Register( DbType.Int, "NUMBER(10,0)" );
			Register( DbType.Char, "CHAR(1)" );
			Register( DbType.VarChar, "VARCHAR2($l)" );
			Register( DbType.Float, "FLOAT" );
			Register( DbType.Real, "DOUBLE PRECISION" );
			Register( DbType.DateTime, "DATE" );
			Register( DbType.SmallDateTime, "DATE" );
			Register( DbType.Timestamp, "DATE" );
			Register( DbType.VarBinary, "RAW($l)" );
			Register( DbType.Decimal, "NUMBER(19, $l)" );
			Register( DbType.Image, "BLOB" );
			Register( DbType.Text, "CLOB" );

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

		public override bool UseNamedParameters {
			get { return true; }
		}

		public override string NamedParametersPrefix {
			get { return ":"; }
		}
	}
}
