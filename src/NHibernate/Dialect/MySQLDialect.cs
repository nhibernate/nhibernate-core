using System;
using System.Data;
using System.Text;

using NHibernate.SqlTypes;
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
//			Register( DbType.Bit, "BIT" );
//			Register( DbType.BigInt,	"BIGINT" );
//			Register( DbType.SmallInt, "SMALLINT" );
//			Register( DbType.TinyInt, "TINYINT" );
//			Register( DbType.Int, "INTEGER" );
//			Register( DbType.Char, "CHAR(1)" );
//			Register( DbType.VarChar, "LONGTEXT" );
//			Register( DbType.VarChar, 1677215, "MEDIUMTEXT" );
//			Register( DbType.VarChar, 65535, "TEXT" );
//			Register( DbType.VarChar, 255, "VARCHAR($1)" );
//			Register( DbType.Float, "FLOAT" );
//			Register( DbType.Real, "DOUBLE PRECISION" );
//			/*
//			And these?
//			register( Types.DATE, "DATE" );
//			register( Types.TIME, "TIME" );
//			*/
//			Register( DbType.Timestamp, "DATETIME" );
//			Register( DbType.VarBinary, "LONGBLOB" );
//			Register( DbType.VarBinary, 16777215, "MEDIUMBLOB" );
//			Register( DbType.VarBinary, 65535, "BLOB" );
//			Register( DbType.VarBinary, 255, "VARCHAR($l) BINARY" );
//			Register( DbType.Decimal, "NUMERIC(19, $l)" );
//			Register( DbType.Image, "LONGBLOB" );
//			Register( DbType.Image, 16777215, "MEDIUMBLOB" );
//			Register( DbType.Image, 65535, "BLOB" );
//			Register( DbType.Text, "LONGTEXT" );
//			Register( DbType.Text, 16777215, "MEDIUMTEXT" );
//			Register( DbType.Text, 65535, "TEXT" );
//			
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
		
		public override string IdentitySelectString
		{
			get	{return "SELECT LAST_INSERT_ID()";}
		}

		public override string IdentityColumnString
		{
			get{return "NOT NULL AUTO_INCREMENT"; }
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

		public override string SqlTypeToString(SqlType sqlType) 
		{
			switch(sqlType.DbType) 
			{
				case DbType.AnsiStringFixedLength: 
					return SqlTypeToString((AnsiStringFixedLengthSqlType)sqlType);
					//break;
				case DbType.Binary :
					return SqlTypeToString((BinarySqlType)sqlType);
					//break;
				case DbType.Boolean :
					return "BIT";
					//break;
				case DbType.Byte:
					return "TINYINT UNSIGNED";
					//break;
				case DbType.Currency:
					return "MONEY";
					//break;
				case DbType.DateTime:
					return "DATETIME";
					//break;
				case DbType.Decimal:
					return SqlTypeToString("NUMERIC", sqlType.Precision, sqlType.Scale);
					//break;
				case DbType.Double:
					return SqlTypeToString("FLOAT", sqlType.Length);
					//break;
				case DbType.Int16:
					return "SMALLINT";
					//break;
				case DbType.Int32:
					return "INTEGER";
					//break;
				case DbType.Int64:
					return "BIGINT";
					//break;
				case DbType.Single:
					return SqlTypeToString("FLOAT", sqlType.Length);
					//break;
				case DbType.StringFixedLength:
					return SqlTypeToString((StringFixedLengthSqlType)sqlType);
					//break;
				case DbType.String:
					return SqlTypeToString((StringSqlType)sqlType);
					//break;
				default:
					throw new ApplicationException("Unmapped DBType");
					//break;
			}

		}
						
			
		private string SqlTypeToString(string name, int length) 
		{
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) 
		{
			return name + "(" + precision + ", " + scale + ")";
		}

		private string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
		{
			
			if(sqlType.Length <= 255) 
			{
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else if(sqlType.Length <= 65535)
			{
				return "TEXT"; 
			}
			else if(sqlType.Length <= 16777215) 
			{
				return "MEDIUMTEXT";
			}
			else 
			{
				return "LONGTEXT";
			}
					
		}

		private string SqlTypeToString(BinarySqlType sqlType) 
		{
			
			if(sqlType.Length <= 255) 
			{
				//return SqlTypeToString("VARBINARY", sqlType.Length);
				return "TINYBLOB";
			}
			else if (sqlType.Length <= 65535)
			{
				return "BLOB";
			}
			else if (sqlType.Length <= 16777215) 
			{
				return "MEDIUMBLOB";
			}
			else {
				return "LONGBLOB"; 
			}
					
		}

		private string SqlTypeToString(StringFixedLengthSqlType sqlType) 
		{
			
			if(sqlType.Length <= 255) 
			{
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else if(sqlType.Length <= 65535)
			{
				return "TEXT"; 
			}
			else if(sqlType.Length <= 16777215) 
			{
				return "MEDIUMTEXT";
			}
			else 
			{
				return "LONGTEXT";
			}
					
		}

		private string SqlTypeToString(StringSqlType sqlType) 
		{
			
			if(sqlType.Length <= 255) 
			{
				return SqlTypeToString("VARCHAR", sqlType.Length);
			}
			else if(sqlType.Length <= 65535)
			{
				return "TEXT"; 
			}
			else if(sqlType.Length <= 16777215) 
			{
				return "MEDIUMTEXT";
			}
			else 
			{
				return "LONGTEXT";
			}
					
		}
	}
}
