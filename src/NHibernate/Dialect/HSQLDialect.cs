using System;
using System.Data;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect compatible with HSQLDB (Hypersonic SQL).
	/// </summary>
	public class HSQLDialect {

		public HSQLDialect() : base() {

			/* Java mapping was:
			
			Types.BIGINT, "BIGINT" );
			Types.BINARY, "BINARY" );
			Types.BIT, "BIT" );
			Types.CHAR, "CHAR(1)" );
			Types.DATE, "DATE" );
			Types.DECIMAL, "DECIMAL" );
			Types.DOUBLE, "DOUBLE" );
			Types.FLOAT, "FLOAT" );
			Types.INTEGER, "INTEGER" );
			Types.LONGVARBINARY, "LONGVARBINARY" )
			Types.LONGVARCHAR, "LONGVARCHAR" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "TINYINT" );
			Types.TIME, "TIME" );
			Types.TIMESTAMP, "TIMESTAMP" );
			Types.VARCHAR, "VARCHAR($l)" );
			Types.VARBINARY, "VARBINARY($l)" );
			Types.NUMERIC, "NUMERIC" );
			*/
	
			Register( SqlDbType.BigInt, "BIGINT" );
			Register( SqlDbType.Binary, "BINARY" );
			Register( SqlDbType.Bit, "BIT" );
			Register( SqlDbType.Char, "CHAR(1)" );
			Register( SqlDbType.DateTime, "DATE" ); //???
			Register( SqlDbType.Decimal, "DECIMAL" );
			Register( SqlDbType.Real, "DOUBLE" );
			Register( SqlDbType.Float, "FLOAT" );
			Register( SqlDbType.Int, "INTEGER" );
			// Register( SqlDbType.LONGVARBINARY, "LONGVARBINARY" );  ???
			Register( SqlDbType.NVarChar, "LONGVARCHAR" );
			Register( SqlDbType.SmallInt, "SMALLINT" );
			Register( SqlDbType.TinyInt, "TINYINT" );
			// Register( SqlDbType.TIME, "TIME" ); ???
			Register( SqlDbType.Timestamp, "TIMESTAMP" );
			Register( SqlDbType.VarChar, "VARCHAR($l)" );
			Register( SqlDbType.VarBinary, "VARBINARY($l)" );
			Register( SqlDbType.Decimal, "NUMERIC" );
		
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "false");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
			*/
		}

		public string GetAddColumnString() {
			return "add column";
		}
	
		public bool HasAlterTable(){
			return false;
		}
	
		public bool DropConstraints() {
			return false;
		}
	
		public bool SupportsIdentityColumns() {
			return true;
		}

		public string GetIdentityColumnString() {
			return "NOT NULL IDENTITY";
		}

		public string GetIdentitySelectString() {
			return "call IDENTITY()";
		}

		public string GetIdentityInsertString() {
			return "null";
		}
	
		public bool SupportsForUpdate() {
			return false;
		}
	
		public bool SupportsUnique() {
			return false;
		}
	}
}