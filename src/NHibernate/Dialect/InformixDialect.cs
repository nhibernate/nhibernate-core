using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Util;

//using NHibernate.Dialect.Schema;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for InformixDialect.
	/// This dialect is intended to work with IDS version 7.31 
	/// However I can test only version 10.00 as I have only this version at work
	/// </summary>
	/// <remarks>
	/// The InformixDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>ConnectionDriver</term>
	///			<description>NHibernate.Driver.OdbcDriver</description>
	///			<term>PrepareSql</term>
	///			<description>true</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.OdbcDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public partial class InformixDialect : Dialect
	{
		/// <summary></summary>
		public InformixDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 32739, "LVARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "TEXT");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.Binary, 2147483647, "BYTE");
			RegisterColumnType(DbType.Binary, "BYTE");
			RegisterColumnType(DbType.Boolean, "BOOLEAN");
			RegisterColumnType(DbType.Currency, "DECIMAL(16,4)");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "datetime year to fraction(5)");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19, 5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL($p, $s)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "SmallFloat");
			RegisterColumnType(DbType.Time, "datetime hour to second");
			RegisterColumnType(DbType.StringFixedLength, "CHAR($l)");
			RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 32739, "LVARCHAR($l)");
			RegisterColumnType(DbType.String, 2147483647, "TEXT");
			RegisterColumnType(DbType.String, "VARCHAR(255)");

			RegisterFunction("substr", new StandardSQLFunction("substr"));
			//			RegisterFunction("trim", new AnsiTrimFunction()); // defined in base class
			//			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));  // defined in base class
			RegisterFunction("coalesce", new NvlFunction()); // base class override
			//			RegisterFunction("abs", new StandardSQLFunction("abs")); 
			//			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
			//			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			//			RegisterFunction("upper", new StandardSQLFunction("upper"));
			//			RegisterFunction("lower", new StandardSQLFunction("lower"));
			//			RegisterFunction("cast", new CastFunction());
			//			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "||", ")"));

			RegisterFunction("current_timestamp", new NoArgSQLFunction("current", NHibernateUtil.DateTime, false));
			RegisterFunction("sysdate", new NoArgSQLFunction("today", NHibernateUtil.DateTime, false));
			RegisterFunction("current", new NoArgSQLFunction("current", NHibernateUtil.DateTime, false));
			RegisterFunction("today", new NoArgSQLFunction("today", NHibernateUtil.DateTime, false));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
			RegisterFunction("date", new StandardSQLFunction("date", NHibernateUtil.DateTime));
			RegisterFunction("mdy", new SQLFunctionTemplate(NHibernateUtil.DateTime, "mdy(?1, ?2, ?3)"));
			RegisterFunction("to_char", new StandardSQLFunction("to_char", NHibernateUtil.String));
			RegisterFunction("to_date", new StandardSQLFunction("to_date", NHibernateUtil.Timestamp));
			RegisterFunction("instr", new StandardSQLFunction("instr", NHibernateUtil.String));
			// actually there is no Instr (or equivalent) in Informix; you have to write your own SPL or UDR

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.OdbcDriver";
			DefaultProperties[Environment.PrepareSql] = "true";
		}

		/// <summary>
		/// The keyword used to insert a generated value into an identity column (or null).
		/// Need if the dialect does not support inserts that specify no column values.
		/// </summary>
		public override string IdentityInsertString
		{
			get { return "0"; }
		}

		/// <summary> Command used to create a temporary table. </summary>
		public override string CreateTemporaryTableString
		{
			get { return "create temp table"; }
		}

		/// <summary> 
		/// Get any fragments needing to be postfixed to the command for
		/// temporary table creation. 
		/// </summary>
		public override string CreateTemporaryTablePostfix
		{
			get { return "with no log"; }
		}

		/// <summary> 
		/// Should the value returned by <see cref="CurrentTimestampSelectString"/>
		/// be treated as callable.  Typically this indicates that JDBC escape
		/// sytnax is being used...
		/// </summary>
		public override bool IsCurrentTimestampSelectStringCallable
		{
			get { return true; }
		}

		/// <summary> 
		/// Retrieve the command used to retrieve the current timestamp from the database. 
		/// </summary>
		public override string CurrentTimestampSelectString
		{
			get { return "select current from systables where tabid=1"; }
		}

		/// <summary> 
		/// The name of the database-specific SQL function for retrieving the
		/// current timestamp. 
		/// </summary>
		public override string CurrentTimestampSQLFunctionName
		{
			get { return "current"; }
		}

		public override IViolatedConstraintNameExtracter ViolatedConstraintNameExtracter
		{
			get { return new IfxViolatedConstraintExtracter(); }
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		//public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		//{
		//    if (connection.ToString()=="IBM.Data.Informix.IfxConnection") {
		//        // this driver doesn't have working IfxConnection.GetSchema 
		//        // and probably will never have
		//        throw new NotSupportedException();
		//    }
		//    if (connection.ToString()=="System.Data.Odbc.OdbcConnection") {
		//        // waiting for somebody implementing OdbcBaseSchema
		//        // return new OdbcBaseSchema(connection);
		//    }
		//    if (connection.ToString()=="IBM.Data.DB2.IfxConnection") {
		//        // waiting for somebody implementing DB2BaseSchema
		//        return new DB2BaseSchema(connection);
		//    }
		//    throw new NotSupportedException();
		//}

		/// <summary> Is <tt>FOR UPDATE OF</tt> syntax supported? </summary>
		/// <value> True if the database supports <tt>FOR UPDATE OF</tt> syntax; false otherwise. </value>
		public override bool ForUpdateOfColumns
		{
			get { return true; }
		}

		/// <summary> 
		/// Does this dialect support <tt>FOR UPDATE</tt> in conjunction with outer joined rows?
		/// </summary>
		/// <value> True if outer joined rows can be locked via <tt>FOR UPDATE</tt>. </value>
		public override bool SupportsOuterJoinForUpdate
		{
			get { return false; }
		}

		/// <summary> 
		/// Get the <tt>FOR UPDATE OF column_list</tt> fragment appropriate for this
		/// dialect given the aliases of the columns to be write locked.
		///  </summary>
		/// <param name="aliases">The columns to be write locked. </param>
		/// <returns> The appropriate <tt>FOR UPDATE OF column_list</tt> clause string. </returns>
		public override string GetForUpdateString(string aliases)
		{
			return ForUpdateString + " of " + aliases;
		}

		/// <summary> Does this dialect support temporary tables? </summary>
		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		/// <summary> 
		/// Does the dialect require that temporary table DDL statements occur in
		/// isolation from other statements?  This would be the case if the creation
		/// would cause any current transaction to get committed implicitly.
		///  </summary>
		/// <returns> see the result matrix above. </returns>
		/// <remarks>
		/// JDBC defines a standard way to query for this information via the
		/// {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}
		/// method.  However, that does not distinguish between temporary table
		/// DDL and other forms of DDL; MySQL, for example, reports DDL causing a
		/// transaction commit via its driver, even though that is not the case for
		/// temporary table DDL.
		/// <p/>
		/// Possible return values and their meanings:<ul>
		/// <li>{@link Boolean#TRUE} - Unequivocally, perform the temporary table DDL in isolation.</li>
		/// <li>{@link Boolean#FALSE} - Unequivocally, do <b>not</b> perform the temporary table DDL in isolation.</li>
		/// <li><i>null</i> - defer to the JDBC driver response in regards to {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}</li>
		/// </ul>
		/// </remarks>
		public override bool? PerformTemporaryTableDDLInIsolation()
		{
			return false;
		}

		public override int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			return position;
		}

		public override DbDataReader GetResultSet(DbCommand statement)
		{
			return statement.ExecuteReader(CommandBehavior.SingleResult);
		}

		/// <summary> Does this dialect support a way to retrieve the database's current timestamp value? </summary>
		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override long TimestampResolutionInTicks
		{
			get { return 100L; } // Maximum precision (one tick)
		}

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary>
		/// Whether this dialect have an Identity clause added to the data type or a
		/// completely separate identity data type
		/// </summary>
		public override bool HasDataTypeInIdentityColumn
		{
			get { return false; }
		}

		/// <summary> 
		/// Get the select command to use to retrieve the last generated IDENTITY
		/// value for a particular table 
		/// </summary>
		/// <param name="tableName">The table into which the insert was done </param>
		/// <param name="identityColumn">The PK column. </param>
		/// <param name="type">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate select command </returns>
		public override string GetIdentitySelectString(string identityColumn, string tableName, DbType type)
		{
			return type == DbType.Int64
			       	? "select dbinfo('serial8') from systables where tabid=1"
			       	: "select dbinfo('sqlca.sqlerrd1') from systables where tabid=1";
		}

		/// <summary>
		/// The syntax that returns the identity value of the last insert, if native
		/// key generation is supported
		/// </summary>
		public override string IdentitySelectString
		{
			//			return type==Types.BIGINT ?
			//				"select dbinfo('serial8') from systables where tabid=1" :
			//				"select dbinfo('sqlca.sqlerrd1') from systables where tabid=1";
			get { return "select dbinfo('sqlca.sqlerrd1') from systables where tabid=1"; }
		}

		/// <summary> 
		/// The syntax used during DDL to define a column as being an IDENTITY of
		/// a particular type. 
		/// </summary>
		/// <param name="type">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate DDL fragment. </returns>
		public override string GetIdentityColumnString(DbType type)
		{
			return type == DbType.Int64 ? "serial8 not null" : "serial not null";
		}

		/// <summary>
		/// The keyword used to specify an identity column, if native key generation is supported
		/// </summary>
		public override string IdentityColumnString
		{
			get { return "serial not null"; }
		}

		/// <summary>
		/// Does this dialect support sequences?
		/// </summary>
		public override bool SupportsSequences
		{
			get { return false; }
		}

		/// <summary> 
		/// Create a <see cref="JoinFragment"/> strategy responsible
		/// for handling this dialect's variations in how joins are handled. 
		/// </summary>
		/// <returns> This dialect's <see cref="JoinFragment"/> strategy. </returns>
		public override JoinFragment CreateOuterJoinFragment()
		{
			return new InformixJoinFragment();
		}

		/// <summary> The SQL literal value to which this database maps boolean values. </summary>
		/// <param name="value">The boolean value </param>
		/// <returns> The appropriate SQL literal. </returns>
		public override string ToBooleanValueString(bool value)
		{
			return value ? "t" : "f";
		}

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>False, unless overridden.</value>
		public override bool SupportsLimit
		{
			// select first * is supported since 7.31
			// but it works with unions since 10.00
			// so it is safer to regard that it is not supported
			get { return false; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

        public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			/*
             * "SELECT [SKIP x] FIRST y  rest-of-sql-statement"
             */

            // TODO - Check support for cases where only the offset is specified, but the limit is not.  Might need to use int.MaxValue.

			int insertIndex = GetAfterSelectInsertPoint(queryString);

            SqlStringBuilder limitFragment = new SqlStringBuilder();
            if (offset != null)
            {
                limitFragment.Add(" skip ");
                limitFragment.Add(offset);
            }
			if (limit != null)
			{
			    limitFragment.Add(" first ");
			    limitFragment.Add(limit);
			}

			return queryString.Insert(insertIndex, limitFragment.ToSqlString());
		}

		/// <summary> 
		/// Does this dialect support UNION ALL, which is generally a faster variant of UNION? 
		/// True if UNION ALL is supported; false otherwise.
		/// </summary>
		public override bool SupportsUnionAll
		{
			get { return true; }
		}

		public override bool SupportsEmptyInList
		{
			get { return false; }
		}

		public override bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor
		{
			get { return false; }
		}

		public override bool DoesRepeatableReadCauseReadersToBlockWriters
		{
			get { return true; }
		}

		public override ISQLExceptionConverter BuildSQLExceptionConverter()
		{
			// The default SQLExceptionConverter for all dialects is based on SQLState
			// since SQLErrorCode is extremely vendor-specific.  Specific Dialects
			// may override to return whatever is most appropriate for that vendor.
			return new SQLStateConverter(ViolatedConstraintNameExtracter);
		}

		private static int GetAfterSelectInsertPoint(SqlString text)
		{
			if (text.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}

			return -1;
		}

		public override string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey, bool referencesPrimaryKey)
		{
			// NH-2026
			var res = new StringBuilder(200);

			res.Append(" add constraint foreign key (")
				.Append(StringHelper.Join(StringHelper.CommaSpace, foreignKey))
				.Append(") references ")
				.Append(referencedTable);

			if (!referencesPrimaryKey)
			{
				res.Append(" (")
					.Append(StringHelper.Join(StringHelper.CommaSpace, primaryKey))
					.Append(')');
			}

			res.Append(" constraint ")
				.Append(constraintName);

			return res.ToString();
		}
	}

	public class IfxViolatedConstraintExtracter : TemplatedViolatedConstraintNameExtracter
	{
		/// <summary> 
		/// Extract the name of the violated constraint from the given DbException.
		/// </summary>
		/// <param name="sqle">The exception that was the result of the constraint violation.</param> 
		/// <returns>The extracted constraint name.</returns> 
		public override string ExtractConstraintName(DbException sqle)
		{
			string constraintName = null;

			var extracter = new ReflectionBasedSqlStateExtracter();

			int errorCode = extracter.ExtractErrorCode(sqle);
			if (errorCode == -268)
			{
				constraintName = ExtractUsingTemplate("Unique constraint (", ") violated.", sqle.Message);
			}
			else if (errorCode == -691)
			{
				constraintName = ExtractUsingTemplate("Missing key in referenced table for referential constraint (", ").",
				                                      sqle.Message);
			}
			else if (errorCode == -692)
			{
				constraintName = ExtractUsingTemplate("Key value for constraint (", ") is still being referenced.", sqle.Message);
			}

			if (constraintName != null)
			{
				// strip table-owner because Informix always returns constraint names as "<table-owner>.<constraint-name>"
				int i = constraintName.IndexOf('.');
				if (i != -1)
				{
					constraintName = constraintName.Substring(i + 1);
				}
			}
			return constraintName;
		}
	} ;
}