using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Environment = NHibernate.Cfg.Environment;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect targeting Sybase Adaptive Server Enterprise (ASE) 15 and higher.
	/// </summary>
	/// <remarks>
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SybaseAseClientDriver" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SybaseASE15Dialect : Dialect
	{
		public SybaseASE15Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseAseClientDriver";
			
			RegisterColumnType(DbType.Boolean, "tinyint"); // Sybase BIT type does not support null values
			RegisterColumnType(DbType.Int16, "smallint");
			RegisterColumnType(DbType.Int16, 255, "tinyint");
			RegisterColumnType(DbType.Int32, "int");
			RegisterColumnType(DbType.Int64, "bigint");
			RegisterColumnType(DbType.Decimal, "numeric(18,0)");
			RegisterColumnType(DbType.Single, "real");
			RegisterColumnType(DbType.Double, "float");
			RegisterColumnType(DbType.AnsiStringFixedLength, "char(1)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 255, "char($l)");
			RegisterColumnType(DbType.StringFixedLength, "nchar(1)");
			RegisterColumnType(DbType.StringFixedLength, 255, "nchar($l)");
			RegisterColumnType(DbType.AnsiString, "varchar(255)");
			RegisterColumnType(DbType.AnsiString, 16384, "varchar($l)");
			RegisterColumnType(DbType.String, "nvarchar(255)");
			RegisterColumnType(DbType.String, 16384, "nvarchar($l)");
			RegisterColumnType(DbType.String, int.MaxValue, "text");
			RegisterColumnType(DbType.DateTime, "datetime");
			RegisterColumnType(DbType.Time, "time");
			RegisterColumnType(DbType.Date, "date");
			RegisterColumnType(DbType.Binary, 8000, "varbinary($l)");
			RegisterColumnType(DbType.Binary, "varbinary");

			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "datalength(?1) * 8"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("char", new StandardSQLFunction("char", NHibernateUtil.String));
			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(","+",")"));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("current_date", new NoArgSQLFunction("getdate", NHibernateUtil.Date));
			RegisterFunction("current_time", new NoArgSQLFunction("getdate", NHibernateUtil.Time));
			RegisterFunction("current_timestamp", new NoArgSQLFunction("getdate", NHibernateUtil.Timestamp));
			RegisterFunction("datename", new StandardSQLFunction("datename", NHibernateUtil.String));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("extract", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(?1, ?3)"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("getdate", new NoArgSQLFunction("getdate", NHibernateUtil.Timestamp));
			RegisterFunction("getutcdate", new NoArgSQLFunction("getutcdate", NHibernateUtil.Timestamp));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(hour, ?1)"));
			RegisterFunction("isnull", new StandardSQLFunction("isnull"));
			RegisterFunction("len", new StandardSQLFunction("len", NHibernateUtil.Int64));
			RegisterFunction("length", new StandardSQLFunction("len", NHibernateUtil.Int32));
			RegisterFunction("locate", new CharIndexFunction());
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(minute, ?1)"));
			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "?1 % ?2"));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("pi", new NoArgSQLFunction("pi", NHibernateUtil.Double));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("rand", new StandardSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("reverse", new StandardSQLFunction("reverse"));
			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(second, ?1)"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("space", new StandardSQLFunction("space", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("square", new StandardSQLFunction("square"));
			RegisterFunction("str", new StandardSQLFunction("str", NHibernateUtil.String));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			// TODO RegisterFunction("trim", new SQLFunctionTemplate(NHibernateUtil.String, "ltrim(rtrim(?1))"));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("user", new NoArgSQLFunction("user", NHibernateUtil.String));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));

			RegisterFunction("substring", new EmulatedLengthSubstringFunction());
		}
		
		public override string AddColumnString
		{
			get { return "add"; }
		}
		
		public override string NullColumnString
		{
			get { return " null"; }
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
			get { return "select @@identity"; }
		}
		
		public override string IdentityColumnString
		{
			get { return "identity not null"; } // starts with 1, implicitly
		}
		
		public override bool SupportsInsertSelectIdentity
		{
			get { return true; }
		}
		
		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}
		
		public override bool IsCurrentTimestampSelectStringCallable
		{
			get { return false; }
		}
		
		public override string CurrentTimestampSelectString
		{
			get { return "select getdate()"; }
		}
		
		/// <summary>
		/// Sybase ASE 15 temporary tables are not supported
		/// </summary>
		/// <remarks>
		/// By default, temporary tables in Sybase ASE 15 can only be created outside a transaction.
		/// This is not supported by NHibernate. Temporary tables (and other DDL) statements can only
		/// be run in a transaction if the 'ddl in tran' database option on tempdb is set to 'true'.
		/// However, Sybase does not recommend this setting due to the performance impact arising from
		/// locking and contention on tempdb system tables.
		/// </remarks>
		public override bool SupportsTemporaryTables
		{
			get { return false; }
		}
		
		public override string SelectGUIDString
		{
			get { return "select newid()"; }
		}
		
		public override bool SupportsEmptyInList
		{
			get { return false; }
		}
		
		public override bool SupportsUnionAll
		{
			get { return true; }
		}
		
		public override bool SupportsExistsInSelect
		{
			get { return false; }
		}
		
		public override bool DoesReadCommittedCauseWritersToBlockReaders
		{
			get { return true; }
		}
		
		public override bool DoesRepeatableReadCauseReadersToBlockWriters
		{
			get { return true; }
		}
		
		public override bool SupportsCascadeDelete
		{
			get { return false; }
		}
		
		public override int MaxAliasLength
		{
			get { return 30; }
		}
		
		/// <summary>
		/// This is false only by default. The database can be configured to be
		/// case-insensitive.
		/// </summary>
		public override bool AreStringComparisonsCaseInsensitive
		{
			get { return false; }
		}
		
		public override string CurrentTimestampSQLFunctionName
		{
			get { return "getdate()"; }
		}
		
		public override bool SupportsExpectedLobUsagePattern
		{
			get { return false; }
		}
		
		public override char OpenQuote
		{
			get { return '['; }
		}
				
		public override char CloseQuote
		{
			get { return ']'; }
		}
		
		public override string ForUpdateString
		{
			get { return String.Empty; }
		}
		
		public override string GenerateTemporaryTableName(string baseTableName)
		{
			return "#" + baseTableName;
		}
		
		public override bool DropTemporaryTableAfterUse()
		{
			return true;
		}
		
		public override SqlString AppendIdentitySelectToInsert(SqlString insertString)
		{
			return insertString.Append("\nselect @@identity");
		}
		
		public override string AppendLockHint(LockMode lockMode, string tableName)
		{
			if (lockMode.GreaterThan(LockMode.Read))
				return tableName + " holdlock";
			
			return tableName;
		}
		
		public override SqlString ApplyLocksToSql(SqlString sql, IDictionary<string, LockMode> aliasedLockModes, IDictionary<string, string[]> keyColumnNames)
		{
			// TODO:  merge additional lockoptions support in Dialect.applyLocksToSql

			var buffer = new StringBuilder(sql.ToString());
			int correction = 0;
			
			foreach (KeyValuePair<string, LockMode> entry in aliasedLockModes)
			{
				LockMode mode = entry.Value;
				
				if (mode.GreaterThan(LockMode.Read))
				{
					string alias = entry.Key;
					int start = -1;
					int end = -1;
					
					if (sql.EndsWith(" " + alias))
					{
						start = (sql.Length - alias.Length) + correction;
						end = start + alias.Length;
					}
					else
					{
						int position = sql.IndexOfCaseInsensitive(" " + alias + " ");
						
						if (position <= -1)
							position = sql.IndexOfCaseInsensitive(" " + alias + ",");
						
						if (position > -1)
						{
							start = position + correction + 1;
							end = start + alias.Length;
						}
					}
					
					if (start > -1)
					{
						string lockHint = AppendLockHint(mode, alias);
						buffer.Remove(start, end - start + 1);
						buffer.Insert(start, lockHint);
						correction += (lockHint.Length - alias.Length);
					}
				}
			}
			return new SqlString(buffer.ToString());
		}
		
		public override int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			return position;
		}
		
		public override DbDataReader GetResultSet(DbCommand statement)
		{
			return statement.ExecuteReader();
		}
	}
}