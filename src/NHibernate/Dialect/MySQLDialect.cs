using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
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
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.MySqlDataDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MySQLDialect : Dialect
	{
		private readonly TypeNames castTypeNames = new TypeNames();

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
			RegisterColumnType(DbType.Currency, "NUMERIC(18,4)");
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

			RegisterCastTypes();

			//functions:
			RegisterFunctions();

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.MySqlDataDriver";
		}

		protected virtual void RegisterFunctions()
		{
			RegisterFunction("iif", new StandardSQLFunction("if"));

			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));

			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("truncate", new StandardSQLFunction("truncate"));
			
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));
			
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));

			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "concat(", ",", ")"));
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("rtrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("left", new StandardSQLFunction("left", NHibernateUtil.String));
			RegisterFunction("right", new StandardSQLFunction("right", NHibernateUtil.String));

			RegisterFunction("ucase", new StandardSQLFunction("ucase"));
			RegisterFunction("lcase", new StandardSQLFunction("lcase"));

			RegisterFunction("chr", new StandardSQLFunction("char", NHibernateUtil.Character));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("instr", new StandardSQLFunction("instr", NHibernateUtil.Int32));
			RegisterFunction("lpad", new StandardSQLFunction("lpad", NHibernateUtil.String));
			RegisterFunction("rpad", new StandardSQLFunction("rpad", NHibernateUtil.String));

			RegisterFunction("hex", new StandardSQLFunction("hex", NHibernateUtil.String));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));

			RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.Date, false));
			RegisterFunction("current_time", new NoArgSQLFunction("current_time", NHibernateUtil.Time, false));

			RegisterFunction("second", new StandardSQLFunction("second", NHibernateUtil.Int32));
			RegisterFunction("minute", new StandardSQLFunction("minute", NHibernateUtil.Int32));
			RegisterFunction("hour", new StandardSQLFunction("hour", NHibernateUtil.Int32));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
			RegisterFunction("date", new StandardSQLFunction("date", NHibernateUtil.Date));
			RegisterFunction("last_day", new StandardSQLFunction("last_day", NHibernateUtil.Date));
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

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			var pagingBuilder = new SqlStringBuilder(queryString);
			pagingBuilder.Add(" limit ");

			if (offset != null)
			{
				pagingBuilder.Add(offset);
				pagingBuilder.Add(", ");
			}

			if (limit != null)
				pagingBuilder.Add(limit);
			else
				pagingBuilder.Add(int.MaxValue.ToString());

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

		protected virtual void RegisterCastTypes()
		{
			// According to the MySql documentation (http://dev.mysql.com/doc/refman/4.1/en/cast-functions.html)
			// only a few values are supported for the cast target type: BINARY, CHAR, DATE, DATETIME,
			// SIGNED, TIME, and UNSIGNED. So we must limit our possible cast types to these

			// The Dialect.GetCastTypeName() method uses the default length, precision, and
			// scale values, so there's no need to consider those values here either, just use the defaults
			RegisterCastType(DbType.AnsiString, "CHAR");
			RegisterCastType(DbType.AnsiStringFixedLength, "CHAR");
			RegisterCastType(DbType.String, "CHAR");
			RegisterCastType(DbType.StringFixedLength, "CHAR");
			RegisterCastType(DbType.Binary, "BINARY");
			RegisterCastType(DbType.Int16, "SIGNED");
			RegisterCastType(DbType.Int32, "SIGNED");
			RegisterCastType(DbType.Int64, "SIGNED");
			RegisterCastType(DbType.UInt16, "UNSIGNED");
			RegisterCastType(DbType.UInt32, "UNSIGNED");
			RegisterCastType(DbType.UInt64, "UNSIGNED");
			RegisterCastType(DbType.Guid, "CHAR(40)");
			RegisterCastType(DbType.Time, "TIME");
			RegisterCastType(DbType.Date, "DATE");
			RegisterCastType(DbType.DateTime, "DATETIME");
		}

		/// <summary>
		/// Subclasses register a typename for the given type code, to be used in CAST()
		/// statements.
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="name">The database type name</param>
		protected void RegisterCastType(DbType code, string name)
		{
			castTypeNames.Put(code, name);
		}

		/// <summary>
		/// Subclasses register a typename for the given type code, to be used in CAST()
		/// statements.
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="capacity"></param>
		/// <param name="name">The database type name</param>
		protected void RegisterCastType(DbType code, int capacity, string name)
		{
			castTypeNames.Put(code, capacity, name);
		}

		/// <summary> 
		/// Get the name of the database type appropriate for casting operations
		/// (via the CAST() SQL function) for the given <see cref="SqlType"/> typecode.
		/// </summary>
		/// <param name="sqlType">The <see cref="SqlType"/> typecode </param>
		/// <returns> The database type name </returns>
		public override string GetCastTypeName(SqlType sqlType)
		{
			string result = castTypeNames.Get(sqlType.DbType, Column.DefaultLength, Column.DefaultPrecision, Column.DefaultScale);
			if (result == null)
			{
				throw new HibernateException(string.Format("No CAST() type mapping for SqlType {0}", sqlType));
			}
			return result;
		}

		public override long TimestampResolutionInTicks
		{
			get
			{
				// MySQL before version 5.6.4 does not support fractional seconds.
				return TimeSpan.TicksPerSecond;
			}
		}
	}
}