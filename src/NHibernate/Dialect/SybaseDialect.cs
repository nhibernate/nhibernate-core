using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect compatible with Sybase and MS SQL Server.
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
			Types.VARCHAR, "VARCHAR($l)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATETIME" );
			Types.TIME, "DATETIME" );
			Types.TIMESTAMP, "DATETIME" );
			Types.VARBINARY, "VARBINARY($l)"
			Types.NUMERIC, "NUMERIC(19,$l)" 
			Types.BLOB, "IMAGE" );
			Types.CLOB, "TEXT" );
			*/			

			Register( DbType.Bit, "TINYINT" ); //Sybase BIT type does not support null values
			Register( DbType.BigInt, "NUMERIC(19,0)" );
			Register( DbType.SmallInt, "SMALLINT" );
			Register( DbType.TinyInt, "TINYINT" );
			Register( DbType.Int, "INT" );
			Register( DbType.Char, "CHAR(1)" );
			Register( DbType.VarChar, "VARCHAR($l)" );
			Register( DbType.Float, "FLOAT" );
			Register( DbType.Real, "DOUBLE PRECISION" );
			Register( DbType.DateTime, "DATETIME" );
			Register( DbType.Timestamp, "DATETIME" );
			Register( DbType.VarBinary, "VARBINARY($l)" );
			Register( DbType.Decimal, "NUMERIC(19,$l)" );
			Register( DbType.Image, "IMAGE" );
			Register( DbType.Text, "TEXT" );
		
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
			*/
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