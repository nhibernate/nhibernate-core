using System;
using System.Data;
using System.Text;

using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// An SQL dialect for MySQL
	/// </summary>
	public class MySQLDialect : Dialect	
	{
		public MySQLDialect() : base() 
		{
			
			/* TODO:
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			*/
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
			
		/// <summary>
		/// The character used to close a Quoted identifier
		/// </summary>
		/// <value>MySql overrides Dialects default value with '`'</value>
		public override char CloseQuote
		{
			get { return '`'; }
		}

		/// <summary>
		/// The character used to open a Quoted identifier
		/// </summary>
		/// <value>MySql overrides Dialects default value with '`'</value>
		public override char OpenQuote
		{
			get { return '`'; }
		}

		private string SqlTypeToString(string name, int length) 
		{
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) 
		{
			return name + "(" + precision + ", " + scale + ")";
		}

		protected override string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
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

		protected override string SqlTypeToString(BinarySqlType sqlType) 
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

		protected override string SqlTypeToString(BooleanSqlType sqlType)
		{
			return "TINYINT(1)";
		}

		
		protected override string SqlTypeToString(ByteSqlType sqlType)
		{
			return "TINYINT UNSIGNED";
		}

		protected override string SqlTypeToString(CurrencySqlType sqlType)
		{
			return "MONEY";
		}

		protected override string SqlTypeToString(DateSqlType sqlType)
		{
			return "DATE";
		}

		protected override string SqlTypeToString(DateTimeSqlType sqlType)
		{
			return "DATETIME";
		}

		protected override string SqlTypeToString(DecimalSqlType sqlType)
		{
			return SqlTypeToString("NUMERIC", sqlType.Precision, sqlType.Scale);
		}

		protected override string SqlTypeToString(DoubleSqlType sqlType)
		{
			return SqlTypeToString("FLOAT", sqlType.Length);
		}

		protected override string SqlTypeToString(Int16SqlType sqlType)
		{
			return "SMALLINT";
		}

		protected override string SqlTypeToString(Int32SqlType sqlType)
		{
			return "INTEGER";
		}

		protected override string SqlTypeToString(Int64SqlType sqlType)
		{
			return "BIGINT";
		}

		protected override string SqlTypeToString(SingleSqlType sqlType)
		{
			return SqlTypeToString("FLOAT", sqlType.Length);
		}

		protected override string SqlTypeToString(StringFixedLengthSqlType sqlType) 
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

		protected override string SqlTypeToString(StringSqlType sqlType) 
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

		protected override string SqlTypeToString(TimeSqlType sqlType)
		{
			return "TIME";
		}

	}
}
