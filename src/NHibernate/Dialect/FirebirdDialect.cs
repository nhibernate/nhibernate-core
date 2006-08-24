using System.Data;

using NHibernate.Cfg;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for FirebirdDialect.
	/// </summary>
	/// <remarks>
	/// The FirebirdDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.FirebirdDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class FirebirdDialect : Dialect
	{
		public FirebirdDialect()
			: base()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($1)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($1)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "BLOB"); // should use the IType.ClobType
			RegisterColumnType(DbType.Binary, "BLOB SUB_TYPE 0");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB SUB_TYPE 0"); // should use the IType.BlobType
			RegisterColumnType(DbType.Boolean, "SMALLINT");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(18,4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(18,5)"); // NUMERIC(18,5) is equivalent to DECIMAL(18,5)
			RegisterColumnType(DbType.Decimal, 18, "DECIMAL(18, $1)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "CHAR($1)");
			RegisterColumnType(DbType.String, "VARCHAR(255)");
			RegisterColumnType(DbType.String, 4000, "VARCHAR($1)");
			RegisterColumnType(DbType.String, 1073741823, "BLOB SUB_TYPE 1"); // should use the IType.ClobType
			RegisterColumnType(DbType.Time, "TIME");

			// Firebird server embedded functions
			RegisterFunction("upper", new StandardSQLFunction());
			RegisterFunction("substring", new StandardSQLFunction());
			RegisterFunction("today", new NoArgSQLFunction(NHibernateUtil.Date, false));
			RegisterFunction("yesterday", new NoArgSQLFunction(NHibernateUtil.Date, false));
			RegisterFunction("tomorrow", new NoArgSQLFunction(NHibernateUtil.Date, false));
			RegisterFunction("now", new NoArgSQLFunction(NHibernateUtil.DateTime, false));
			RegisterFunction("nullif", new StandardSQLFunction());

			// External Firebird and Interbase standard UDFs
			//Conditional Logic Functions
			RegisterFunction("invl", new StandardSQLFunction());
			RegisterFunction("snvl", new StandardSQLFunction());
			//Mathematical Functions
			RegisterFunction("abs", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("bin_and", new StandardSQLFunction(NHibernateUtil.Int32));
			RegisterFunction("bin_or", new StandardSQLFunction(NHibernateUtil.Int32));
			RegisterFunction("bin_xor", new StandardSQLFunction(NHibernateUtil.Int32));
			RegisterFunction("ceiling", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("div", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("dpower", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("floor", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("ln", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("modulo", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("pi", new NoArgSQLFunction(NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction(NHibernateUtil.Double));
			RegisterFunction("round", new StandardSQLFunction());
			RegisterFunction("sing", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("sqtr", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("truncate", new StandardSQLFunction());
			//Date and Time Functions
			RegisterFunction("dow", new StandardSQLFunction(NHibernateUtil.String));
			RegisterFunction("sdow", new StandardSQLFunction(NHibernateUtil.String));
			RegisterFunction("addday", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addhour", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addmillisecond", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addminute", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addmonth", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addsecond", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addweek", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("addyear", new StandardSQLFunction(NHibernateUtil.DateTime));
			RegisterFunction("getexacttimestamp", new NoArgSQLFunction(NHibernateUtil.DateTime));
			//String and Character Functions
			RegisterFunction("ascii_char", new StandardSQLFunction());
			RegisterFunction("ascii_val", new StandardSQLFunction(NHibernateUtil.Int16));
			RegisterFunction("lpad", new StandardSQLFunction());
			RegisterFunction("ltrim", new StandardSQLFunction());
			RegisterFunction("sright", new StandardSQLFunction());
			RegisterFunction("rpad", new StandardSQLFunction());
			RegisterFunction("rtrim", new StandardSQLFunction());
			RegisterFunction("strlen", new StandardSQLFunction(NHibernateUtil.Int16));
			RegisterFunction("substr", new StandardSQLFunction());
			RegisterFunction("substrlen", new StandardSQLFunction(NHibernateUtil.Int16));
			//BLOB Functions
			RegisterFunction("string2blob", new StandardSQLFunction());
			//Trigonometric Functions
			RegisterFunction("acos", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("cosh", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("sinh", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction(NHibernateUtil.Double));
			RegisterFunction("tanh", new StandardSQLFunction(NHibernateUtil.Double));

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.FirebirdDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return string.Format("select gen_id({0}, 1 ) from RDB$DATABASE", sequenceName);
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return string.Format("create generator {0}", sequenceName);
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return string.Format("drop generator {0}", sequenceName);
		}

		public override bool SupportsSequences
		{
			get { return true; }
		}

		public override string ForUpdateString
		{
			get { return " with lock"; }
		}

		public override bool ForUpdateOfColumns
		{
			get { return true; }
		}

		public override string GetForUpdateString(string aliases)
		{
			return " for update of " + aliases + " with lock";
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		/// <summary>
		/// Add a <c>FIRST x [SKIP] y</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">A Query in the form of a SqlString.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offset">Offset of the first row to process in the result set</param>
		/// <returns>A new SqlString that contains the <c>FIRST</c> clause.</returns>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			/*
			 * "SELECT FIRST x [SKIP y] rest-of-sql-statement"
			 */
			querySqlString = querySqlString.Compact();
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			bool firstAdded = false;
			foreach (object sqlPart in querySqlString.SqlParts)
			{
				if (!firstAdded)
				{
					string sqlPartString = sqlPart as string;
					if (sqlPartString != null)
					{
						string sqlFragment = sqlPartString.TrimStart();
						int insertIndex = GetAfterSelectInsertPoint(sqlFragment);
						if (insertIndex > 0)
						{
							string newFragment = string.Empty;

							if (offset > 0)
							{
								newFragment = sqlFragment.Insert(insertIndex, " first " + limit.ToString() + " skip " + offset.ToString());
							}
							else
							{
								newFragment = sqlFragment.Insert(insertIndex, " first " + limit.ToString());
							}
							pagingBuilder.Add(newFragment);
							firstAdded = true;
							continue;
						}
					}
				}
				pagingBuilder.AddObject(sqlPart);
			}

			return pagingBuilder.ToSqlString();
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		private static int GetAfterSelectInsertPoint(string fragment)
		{
			string fragmentLowerCased = fragment.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			if (fragmentLowerCased.StartsWith("select distinct"))
			{
				return 15;
			}
			else if (fragmentLowerCased.StartsWith("select"))
			{
				return 6;
			}
			return 0;
		}
	}
}
