using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect compatible with Sybase.
	/// </summary>
	public class SybaseDialect : Dialect {

		public SybaseDialect() : base() {

			/* Java mapping was:
			
			Types.BIT, "TINYINT" );
			Types.BIGINT, "NUMERIC(19,0)" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "TINYINT" );
			Types.INTEGER, "INT" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($1)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATETIME" );
			Types.TIME, "DATETIME" );
			Types.TIMESTAMP, "DATETIME" );
			Types.VARBINARY, "VARBINARY($1)"
			Types.NUMERIC, "NUMERIC(19,$1)" 
			Types.BLOB, "IMAGE" );
			Types.CLOB, "TEXT" );
			*/			
		
			DefaultProperties[Cfg.Environment.OuterJoin] = "true";
		}

		public override string AddColumnString {
			get { return "add"; }
		}
		public override string NullColumnString {
			get { return " null"; }
		}
		public override bool QualifyIndexName {
			get { return false; }
		}
	
		public override bool SupportsForUpdate {
			get { return false; }
		}
	
		public override bool SupportsIdentityColumns {
			get { return true; }
		}
		public override string IdentitySelectString {
			get { return "select @@identity"; }
		}
		public override string IdentityColumnString {
			get { return "IDENTITY NOT NULL"; } 
		}

		public override string NoColumnsInsertString {
			get { return "DEFAULT VALUES"; }
		}

	}
}