using System;
using System.Data;
using System.Text;

using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// A SQL dialect for MySQL
	/// </summary>
	public class MySQLDialect : Dialect	
	{
		public MySQLDialect() : base() 
		{
			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 255, "CHAR($1)" );
			Register( DbType.AnsiStringFixedLength, 65535, "TEXT" );
			Register( DbType.AnsiStringFixedLength, 16777215, "MEDIUMTEXT" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 255, "VARCHAR($1)" );
			Register( DbType.AnsiString, 65535, "TEXT" );
			Register( DbType.AnsiString, 16777215, "MEDIUMTEXT" );
			Register( DbType.Binary, "LONGBLOB");
			Register( DbType.Binary, 255, "VARCHAR($1) BINARY");
			Register( DbType.Binary, 65535, "BLOB");
			Register( DbType.Binary, 16777215, "MEDIUMBLOB");
			Register( DbType.Boolean, "TINYINT(1)" ); 
			Register( DbType.Byte, "TINYINT UNSIGNED" );
			Register( DbType.Currency, "MONEY");
			Register( DbType.Date, "DATE");
			Register( DbType.DateTime, "DATETIME" );
			Register( DbType.Decimal, "NUMERIC(19,5)" );
			Register( DbType.Decimal, 19, "NUMERIC(19, $1)");
			Register( DbType.Double, "FLOAT" );
			Register( DbType.Guid, "VARCHAR(40)" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "FLOAT" ); 
			Register( DbType.StringFixedLength, "CHAR(255)");
			Register( DbType.StringFixedLength, 255, "CHAR($1)" );
			Register( DbType.StringFixedLength, 65535, "TEXT" );
			Register( DbType.StringFixedLength, 16777215, "MEDIUMTEXT" );
			Register( DbType.String, "VARCHAR(255)" );
			Register( DbType.String, 255, "VARCHAR($1)" );
			Register( DbType.String, 65535, "TEXT" );
			Register( DbType.String, 16777215, "MEDIUMTEXT" );
			Register( DbType.Time, "TIME" );
			
			DefaultProperties[Cfg.Environment.OuterJoin] = "true";
			DefaultProperties[Cfg.Environment.StatementBatchSize] = DefaultBatchSize;
		}

		public override string AddColumnString 
		{
			get{ return "add column"; }
		}
		public override bool DropConstraints 
		{
			get { return false; }
		}
		public override bool QualifyIndexName 
		{
			get { return false; }
		}
	
		public override bool SupportsIdentityColumns 
		{
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

	
		protected override char CloseQuote
		{
			get { return '`';}
		}

		protected override char OpenQuote
		{
			get { return '`';}
		}

		public override string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey) 
		{
			string cols = String.Join(StringHelper.CommaSpace, foreignKey);
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
				.Append( String.Join(StringHelper.CommaSpace, primaryKey) )
				.Append(')')
				.ToString();
		}
	}
}
