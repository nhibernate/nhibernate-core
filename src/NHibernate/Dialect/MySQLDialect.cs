using System;
using System.Text;
using System.Data;

using NHibernate.Util;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect for MySQL
	/// </summary>
	public class MySQLDialect : Dialect	{

		public MySQLDialect() : base() {
			/*  Type mapping in Java was:
			 
				Types.BIT, "BIT"
				Types.BIGINT, "BIGINT"
				Types.SMALLINT, "SMALLINT"
				Types.TINYINT, "TINYINT"
				Types.INTEGER, "INTEGER"
				Types.CHAR, "CHAR(1)"
				Types.VARCHAR, "LONGTEXT"
				Types.VARCHAR, 16777215, "MEDIUMTEXT"
				Types.VARCHAR, 65535, "TEXT"
				Types.VARCHAR, 255, "VARCHAR($l)"
				Types.FLOAT, "FLOAT"
				Types.DOUBLE, "DOUBLE PRECISION"
				Types.DATE, "DATE"
				Types.TIME, "TIME"
				Types.TIMESTAMP, "DATETIME"
				Types.VARBINARY, "LONGBLOB"
				Types.VARBINARY, 16777215, "MEDIUMBLOB"
				Types.VARBINARY, 65535, "BLOB"
				Types.VARBINARY, 255, "VARCHAR($l) BINARY"
				Types.NUMERIC, "NUMERIC(19, $l)"
				Types.BLOB, "LONGBLOB"
				Types.BLOB, 16777215, "MEDIUMBLOB"
				Types.BLOB, 65535, "BLOB"
				Types.CLOB, "LONGTEXT"
				Types.CLOB, 16777215, "MEDIUMTEXT"
				Types.CLOB, 65535, "TEXT"
			*/

			Register( DbType.Bit, "BIT" );
			Register( DbType.BigInt,	"BIGINT" );
			Register( DbType.SmallInt, "SMALLINT" );
			Register( DbType.TinyInt, "TINYINT" );
			Register( DbType.Int, "INTEGER" );
			Register( DbType.Char, "CHAR(1)" );
			Register( DbType.VarChar, "LONGTEXT" );
			Register( DbType.VarChar, 1677215, "MEDIUMTEXT" );
			Register( DbType.VarChar, 65535, "TEXT" );
			Register( DbType.VarChar, 255, "VARCHAR($1)" );
			Register( DbType.Float, "FLOAT" );
			Register( DbType.Real, "DOUBLE PRECISION" );
			/*
			And these?
			register( Types.DATE, "DATE" );
			register( Types.TIME, "TIME" );
			*/
			Register( DbType.Timestamp, "DATETIME" );
			Register( DbType.VarBinary, "LONGBLOB" );
			Register( DbType.VarBinary, 16777215, "MEDIUMBLOB" );
			Register( DbType.VarBinary, 65535, "BLOB" );
			Register( DbType.VarBinary, 255, "VARCHAR($l) BINARY" );
			Register( DbType.Decimal, "NUMERIC(19, $l)" );
			Register( DbType.Image, "LONGBLOB" );
			Register( DbType.Image, 16777215, "MEDIUMBLOB" );
			Register( DbType.Image, 65535, "BLOB" );
			Register( DbType.Text, "LONGTEXT" );
			Register( DbType.Text, 16777215, "MEDIUMTEXT" );
			Register( DbType.Text, 65535, "TEXT" );
			
			/* TODO:
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			*/
		}

		public override string AddColumnString {
			get{ return "add column"; }
		}
		public override bool DropConstraints {
			get { return false; }
		}
		public override bool QualifyIndexName {
			get { return false; }
		}
	
		public override bool SupportsIdentityColumns {
			get { return true; }
		}
		public string GetIdentitySelectString {
			get { return "SELECT LAST_INSERT_ID()"; }
		}
	
		public string GetIdentityColumnString {
			get { return "NOT NULL AUTO_INCREMENT"; }
		}
	
		public override string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey) {
			string cols = StringHelper.Join(StringHelper.CommaSpace, foreignKey);
			return new StringBuilder(30)
				.Append(" add index (")
				.Append(cols)
				.Append("), add constraint ")
				.Append(constraintName)
				.Append(" foreign key (")
				.Append(cols)
				.Append(") references ")
				.Append(referencedTable)
				.Append(" (")
				.Append( StringHelper.Join(StringHelper.CommaSpace, primaryKey) )
				.Append(')')
				.ToString();
		}
	}
}
