using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for MySQL
	/// </summary>
	/// <remarks>
	/// The MySQLDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>use_outer_join</term>
	///			<description><see langword="true" /></description>
	///		</item>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.MySqlDataDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MySQLDialect : Dialect
	{
		public MySQLDialect()
		{
			//Reference 3-4.x
			//Numeric:
			//http://dev.mysql.com/doc/refman/4.1/en/numeric-type-overview.html
			//Date and time:
			//http://dev.mysql.com/doc/refman/4.1/en/date-and-time-type-overview.html
			//String:
			//http://dev.mysql.com/doc/refman/5.0/en/string-type-overview.html
			//default:
			//http://dev.mysql.com/doc/refman/5.0/en/data-type-defaults.html

			//string type
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiStringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiString, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.StringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.String, "VARCHAR(255)");
			RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 65535, "TEXT");
			RegisterColumnType(DbType.String, 16777215, "MEDIUMTEXT");
			//todo: future: add compatibility with decimal???
			//An unpacked fixed-point number. Behaves like a CHAR column; 
			//“unpacked” means the number is stored as a string, using one character for each digit of the value.
			//M is the total number of digits and D is the number of digits after the decimal point
			//DECIMAL[(M[,D])] [UNSIGNED] [ZEROFILL]

			//binary type:
			RegisterColumnType(DbType.Binary, "LONGBLOB");
			RegisterColumnType(DbType.Binary, 127, "TINYBLOB");
			RegisterColumnType(DbType.Binary, 65535, "BLOB");
			RegisterColumnType(DbType.Binary, 16777215, "MEDIUMBLOB");

			//Numeric type:
			RegisterColumnType(DbType.Boolean, "TINYINT(1)"); // SELECT IF(0, 'true', 'false');
			RegisterColumnType(DbType.Byte, "TINYINT UNSIGNED");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMERIC($p, $s)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			//The signed range is -32768 to 32767. The unsigned range is 0 to 65535. 
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER"); //alias INT
			//As of MySQL 4.1, SERIAL is an alias for BIGINT UNSIGNED NOT NULL AUTO_INCREMENT UNIQUE. 
			RegisterColumnType(DbType.Int64, "BIGINT");
			//!!!
			//Using FLOAT might give you some unexpected problems because all calculations in MySQL are done with double precision
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.Byte, 1, "BIT"); //Like TinyInt(i)
			RegisterColumnType(DbType.SByte, "TINYINT");

			//UNSINGED Numeric type:
			RegisterColumnType(DbType.UInt16, "SMALLINT UNSIGNED");
			RegisterColumnType(DbType.UInt32, "INTEGER UNSIGNED");
			RegisterColumnType(DbType.UInt64, "BIGINT UNSIGNED");
			//there are no other DbType unsigned...but mysql support Float unsigned, double unsigned, etc..

			//Date and time type:
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Time, "TIME");

			//special:
			RegisterColumnType(DbType.Guid, "VARCHAR(40)");

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.MySqlDataDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}

		/// <summary></summary>
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string IdentitySelectString
		{
			get { return "SELECT LAST_INSERT_ID()"; }
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "NOT NULL AUTO_INCREMENT"; }
		}

		/// <summary></summary>
		public override char CloseQuote
		{
			get { return '`'; }
		}

		/// <summary></summary>
		public override char OpenQuote
		{
			get { return '`'; }
		}

		public override bool SupportsIfExistsBeforeTableName
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new MySQLDataBaseSchema(connection);
		}

		public override bool SupportsSubSelects
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="querySqlString"></param>
		/// <param name="hasOffset"></param>
		/// <returns></returns>
		public override SqlString GetLimitString(SqlString querySqlString, bool hasOffset)
		{
			var pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" limit ");
			pagingBuilder.Add(Parameter.Placeholder);

			if (hasOffset)
			{
				pagingBuilder.Add(", ");
				pagingBuilder.Add(Parameter.Placeholder);
			}
			return pagingBuilder.ToSqlString();
		}

		public override string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey,
		                                                        string referencedTable, string[] primaryKey,
		                                                        bool referencesPrimaryKey)
		{
			string cols = String.Join(StringHelper.CommaSpace, foreignKey);
			return
				new StringBuilder(30).Append(" add index (").Append(cols).Append("), add constraint ").Append(constraintName).Append
					(" foreign key (").Append(cols).Append(") references ").Append(referencedTable).Append(" (").Append(
					String.Join(StringHelper.CommaSpace, primaryKey)).Append(')').ToString();
		}

		/// <summary>
		/// Create the SQL string to drop a foreign key constraint.
		/// </summary>
		/// <param name="constraintName">The name of the foreign key to drop.</param>
		/// <returns>The SQL string to drop the foreign key constraint.</returns>
		public override string GetDropForeignKeyConstraintString(string constraintName)
		{
			return " drop foreign key " + constraintName;
		}

		/// <summary>
		/// Create the SQL string to drop a primary key constraint.
		/// </summary>
		/// <param name="constraintName">The name of the primary key to drop.</param>
		/// <returns>The SQL string to drop the primary key constraint.</returns>
		public override string GetDropPrimaryKeyConstraintString(string constraintName)
		{
			return " drop primary key " + constraintName;
		}

		/// <summary>
		/// Create the SQL string to drop an index.
		/// </summary>
		/// <param name="constraintName">The name of the index to drop.</param>
		/// <returns>The SQL string to drop the index constraint.</returns>
		public override string GetDropIndexConstraintString(string constraintName)
		{
			return " drop index " + constraintName;
		}

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string CreateTemporaryTableString
		{
			get { return "create temporary table if not exists"; }
		}
	}
}