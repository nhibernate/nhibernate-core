using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Lock;
using NHibernate.Dialect.Schema;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Represents a dialect of SQL implemented by a particular RDBMS. Subclasses
	/// implement NHibernate compatibility with different systems.
	/// </summary>
	/// <remarks>
	/// Subclasses should provide a public default constructor that <c>Register()</c>
	/// a set of type mappings and default Hibernate properties.
	/// </remarks>
	public abstract class Dialect
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(Dialect));

		protected const string DefaultBatchSize = "15";
		protected const string NoBatch = "0";

		/// <summary> Characters used for quoting sql identifiers </summary>
		public const string PossibleQuoteChars = "`'\"[";

		/// <summary> Characters used for closing quoted sql identifiers </summary>
		public const string PossibleClosedQuoteChars = "`'\"]";
		
		private readonly TypeNames _typeNames = new TypeNames();
		private readonly TypeNames _hibernateTypeNames = new TypeNames();
		private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();
		private readonly IDictionary<string, ISQLFunction> _sqlFunctions;
		private readonly HashSet<string> _sqlKeywords = new HashSet<string>();

		private static readonly IDictionary<string, ISQLFunction> StandardAggregateFunctions = CollectionHelper.CreateCaseInsensitiveHashtable<ISQLFunction>();

		private static readonly IViolatedConstraintNameExtracter Extracter;

		#region Constructors and factory methods

		static Dialect()
		{
			StandardAggregateFunctions["count"] = new CountQueryFunctionInfo();
			StandardAggregateFunctions["avg"] = new AvgQueryFunctionInfo();
			StandardAggregateFunctions["max"] = new ClassicAggregateFunction("max", false);
			StandardAggregateFunctions["min"] = new ClassicAggregateFunction("min", false);
			StandardAggregateFunctions["sum"] = new SumQueryFunctionInfo();

			Extracter = new NoOpViolatedConstraintNameExtracter();
		}

		/// <summary>
		/// The base constructor for Dialect.
		/// </summary>
		/// <remarks>
		/// Every subclass should override this and call Register() with every <see cref="DbType"/> except
		/// <see cref="DbType.Object"/>, <see cref="DbType.SByte"/>, <see cref="DbType.UInt16"/>, <see cref="DbType.UInt32"/>, 
		/// <see cref="DbType.UInt64"/>, <see cref="DbType.VarNumeric"/>.
		/// 
		/// <para>
		/// The Default properties for this Dialect should also be set - such as whether or not to use outer-joins
		/// and what the batch size should be.
		/// </para>
		/// </remarks>
		protected Dialect()
		{
			Log.Info("Using dialect: " + this);

			_sqlFunctions = CollectionHelper.CreateCaseInsensitiveHashtable(StandardAggregateFunctions);
			
			// standard sql92 functions (can be overridden by subclasses)
			RegisterFunction("substring", new AnsiSubstringFunction());
			RegisterFunction("locate", new StandardSQLFunction("locate", NHibernateUtil.Int32));
			RegisterFunction("trim", new AnsiTrimFunction());
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));
			RegisterFunction("bit_length", new StandardSQLFunction("bit_length", NHibernateUtil.Int32));
			RegisterFunction("coalesce", new StandardSQLFunction("coalesce"));
			RegisterFunction("nullif", new StandardSQLFunction("nullif"));
			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("cast", new CastFunction());
			RegisterFunction("extract", new AnsiExtractFunction());
			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "||", ")"));

			// the syntax of current_timestamp is extracted from H3.2 tests 
			// - test\hql\ASTParserLoadingTest.java
			// - test\org\hibernate\test\hql\HQLTest.java
			RegisterFunction("current_timestamp", new NoArgSQLFunction("current_timestamp", NHibernateUtil.DateTime, true));
			RegisterFunction("sysdate", new NoArgSQLFunction("sysdate", NHibernateUtil.DateTime, false));

			//map second/minute/hour/day/month/year to ANSI extract(), override on subclasses
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(second from ?1)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(minute from ?1)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(hour from ?1)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(day from ?1)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(month from ?1)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(year from ?1)"));

			// Bitwise operations
			RegisterFunction("band", new BitwiseNativeOperation("&"));
			RegisterFunction("bor", new BitwiseNativeOperation("|"));
			RegisterFunction("bxor", new BitwiseNativeOperation("^"));
			RegisterFunction("bnot", new BitwiseNativeOperation("~", true));

			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as char)"));

			// register hibernate types for default use in scalar sqlquery type auto detection
			RegisterHibernateType(DbType.Int64, NHibernateUtil.Int64.Name);
			RegisterHibernateType(DbType.Binary, NHibernateUtil.Binary.Name);
			RegisterHibernateType(DbType.Boolean, NHibernateUtil.Boolean.Name);
			RegisterHibernateType(DbType.AnsiString, NHibernateUtil.Character.Name);
			RegisterHibernateType(DbType.Date, NHibernateUtil.Date.Name);
			RegisterHibernateType(DbType.Double, NHibernateUtil.Double.Name);
			RegisterHibernateType(DbType.Single, NHibernateUtil.Single.Name);
			RegisterHibernateType(DbType.Int32, NHibernateUtil.Int32.Name);
			RegisterHibernateType(DbType.Int16, NHibernateUtil.Int16.Name);
			RegisterHibernateType(DbType.SByte, NHibernateUtil.SByte.Name);
			RegisterHibernateType(DbType.Time, NHibernateUtil.Time.Name);
			RegisterHibernateType(DbType.DateTime, NHibernateUtil.Timestamp.Name);
			RegisterHibernateType(DbType.String, NHibernateUtil.String.Name);
			RegisterHibernateType(DbType.VarNumeric, NHibernateUtil.Decimal.Name);
			RegisterHibernateType(DbType.Decimal, NHibernateUtil.Decimal.Name);
		}

		/// <summary> Get an instance of the dialect specified by the current <see cref="Cfg.Environment"/> properties. </summary>
		/// <returns> The specified Dialect </returns>
		public static Dialect GetDialect()
		{
			string dialectName;
			try
			{
				dialectName = Environment.Properties[Environment.Dialect];
			}
			catch (Exception e)
			{
				throw new HibernateException("The dialect was not set. Set the property 'dialect'.", e);
			}
			return InstantiateDialect(dialectName);
		}

		/// <summary>
		/// Get <see cref="Dialect"/> from a property bag (prop name <see cref="Cfg.Environment.Dialect"/>)
		/// </summary>
		/// <param name="props">The property bag.</param>
		/// <returns>An instance of <see cref="Dialect"/>.</returns>
		/// <exception cref="System.ArgumentNullException">When <paramref name="props"/> is null.</exception>
		/// <exception cref="HibernateException">When the property bag don't contains de property <see cref="Cfg.Environment.Dialect"/>.</exception>
		public static Dialect GetDialect(IDictionary<string, string> props)
		{
			if (props == null)
				throw new ArgumentNullException("props");
			string dialectName;
			if (props.TryGetValue(Environment.Dialect, out dialectName) == false)
				throw new InvalidOperationException("Could not find the dialect in the configuration");
			if (dialectName == null)
			{
				return GetDialect();
			}

			return InstantiateDialect(dialectName);
		}

		private static Dialect InstantiateDialect(string dialectName)
		{
			try
			{
				return (Dialect)Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(dialectName));
			}
			catch (Exception e)
			{
				throw new HibernateException("Could not instantiate dialect class " + dialectName, e);
			}
		}

		#endregion

		#region Database type mapping support

		/// <summary>
		/// Get the name of the database type associated with the given 
		/// <see cref="SqlTypes.SqlType"/>,
		/// </summary>
		/// <param name="sqlType">The SqlType</param>
		/// <returns>The database type name used by ddl.</returns>
		public virtual string GetTypeName(SqlType sqlType)
		{
			if (sqlType.LengthDefined || sqlType.PrecisionDefined)
			{
				string resultWithLength = _typeNames.Get(sqlType.DbType, sqlType.Length, sqlType.Precision, sqlType.Scale);
				if (resultWithLength != null) return resultWithLength;
			}

			string result = _typeNames.Get(sqlType.DbType);
			if (result == null)
			{
				throw new HibernateException(string.Format("No default type mapping for SqlType {0}", sqlType));
			}

			return result;
		}

		/// <summary>
		/// Get the name of the database type associated with the given
		/// <see cref="SqlType"/>.
		/// </summary>
		/// <param name="sqlType">The SqlType </param>
		/// <param name="length">The datatype length </param>
		/// <param name="precision">The datatype precision </param>
		/// <param name="scale">The datatype scale </param>
		/// <returns>The database type name used by ddl.</returns>
		public virtual string GetTypeName(SqlType sqlType, int length, int precision, int scale)
		{
			string result = _typeNames.Get(sqlType.DbType, length, precision, scale);
			if (result == null)
			{
				throw new HibernateException(string.Format("No type mapping for SqlType {0} of length {1}", sqlType, length));
			}
			return result;
		}

		/// <summary>
		/// Gets the name of the longest registered type for a particular DbType.
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public virtual string GetLongestTypeName(DbType dbType)
		{
			return _typeNames.GetLongest(dbType);
		}

		/// <summary> 
		/// Get the name of the database type appropriate for casting operations
		/// (via the CAST() SQL function) for the given <see cref="SqlType"/> typecode.
		/// </summary>
		/// <param name="sqlType">The <see cref="SqlType"/> typecode </param>
		/// <returns> The database type name </returns>
		public virtual string GetCastTypeName(SqlType sqlType)
		{
			return GetTypeName(sqlType, Column.DefaultLength, Column.DefaultPrecision, Column.DefaultScale);
		}

		/// <summary>
		/// Subclasses register a typename for the given type code and maximum
		/// column length. <c>$l</c> in the type name will be replaced by the column
		/// length (if appropriate)
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="capacity">Maximum length of database type</param>
		/// <param name="name">The database type name</param>
		protected void RegisterColumnType(DbType code, int capacity, string name)
		{
			_typeNames.Put(code, capacity, name);
		}

		/// <summary>
		/// Subclasses register a typename for the given type code. <c>$l</c> in the 
		/// typename will be replaced by the column length (if appropriate).
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="name">The database type name</param>
		protected void RegisterColumnType(DbType code, string name)
		{
			_typeNames.Put(code, name);
		}

		#endregion

		#region DDL support

		/// <summary>
		/// Do we need to drop constraints before dropping tables in the dialect?
		/// </summary>
		public virtual bool DropConstraints
		{
			get { return true; }
		}

		/// <summary>
		/// Do we need to qualify index names with the schema name?
		/// </summary>
		public virtual bool QualifyIndexName
		{
			get { return true; }
		}

		/// <summary>
		/// Does this dialect support the <c>UNIQUE</c> column syntax?
		/// </summary>
		public virtual bool SupportsUnique
		{
			get { return true; }
		}

		/// <summary> Does this dialect support adding Unique constraints via create and alter table ?</summary>
		public virtual bool SupportsUniqueConstraintInCreateAlterTable
		{
			get { return true; }
		}

		/// <summary>
		/// Does this dialect support adding foreign key constraints via alter table?  If not, it's assumed they can only be added through create table.
		/// </summary>
		public virtual bool SupportsForeignKeyConstraintInAlterTable
		{
			get { return true; }
		}

		/// <summary> 
		/// The syntax used to add a foreign key constraint to a table.  If SupportsForeignKeyConstraintInAlterTable is false, the returned string will be added to the create table statement instead.  In this case, extra strings, like "add", that apply when using alter table should be omitted.
		/// </summary>
		/// <param name="constraintName">The FK constraint name. </param>
		/// <param name="foreignKey">The names of the columns comprising the FK </param>
		/// <param name="referencedTable">The table referenced by the FK </param>
		/// <param name="primaryKey">The explicit columns in the referencedTable referenced by this FK. </param>
		/// <param name="referencesPrimaryKey">
		/// if false, constraint should be explicit about which column names the constraint refers to 
		/// </param>
		/// <returns> the "add FK" fragment </returns>
		public virtual string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey, bool referencesPrimaryKey)
		{
			var res = new StringBuilder(200);

			if (SupportsForeignKeyConstraintInAlterTable)
				res.Append(" add");

			res.Append(" constraint ")
				.Append(constraintName)
				.Append(" foreign key (")
				.Append(StringHelper.Join(StringHelper.CommaSpace, foreignKey))
				.Append(") references ")
				.Append(referencedTable);

			if (!referencesPrimaryKey)
			{
				res.Append(" (")
					.Append(StringHelper.Join(StringHelper.CommaSpace, primaryKey))
					.Append(')');
			}

			return res.ToString();
		}

		/// <summary>
		/// The syntax used to add a primary key constraint to a table
		/// </summary>
		/// <param name="constraintName"></param>
		public virtual string GetAddPrimaryKeyConstraintString(string constraintName)
		{
			return " add constraint " + constraintName + " primary key ";
		}

		public virtual bool HasSelfReferentialForeignKeyBug
		{
			get { return false; }
		}

		public virtual bool SupportsCommentOn
		{
			get { return false; }
		}

		public virtual string GetTableComment(string comment)
		{
			return string.Empty;
		}

		public virtual string GetColumnComment(string comment)
		{
			return string.Empty;
		}

		/// <summary>
		/// Does the dialect support the syntax 'drop table if exists NAME'
		/// </summary>
		public virtual bool SupportsIfExistsBeforeTableName
		{
			get { return false; }
		}

		/// <summary>
		/// Does the dialect support the syntax 'drop table NAME if exists'
		/// </summary>
		public virtual bool SupportsIfExistsAfterTableName
		{
			get { return false; }
		}

		/// <summary> Does this dialect support column-level check constraints? </summary>
		/// <returns> True if column-level CHECK constraints are supported; false otherwise. </returns>
		public virtual bool SupportsColumnCheck
		{
			get { return true; }
		}

		/// <summary> Does this dialect support table-level check constraints? </summary>
		/// <returns> True if table-level CHECK constraints are supported; false otherwise. </returns>
		public virtual bool SupportsTableCheck
		{
			get { return true; }
		}

		public virtual bool SupportsCascadeDelete
		{
			get { return true; }
		}

		public virtual bool SupportsNotNullUnique
		{
			get { return true; }
		}

		public virtual IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Lock acquisition support

		/// <summary> 
		/// Get a strategy instance which knows how to acquire a database-level lock
		/// of the specified mode for this dialect. 
		/// </summary>
		/// <param name="lockable">The persister for the entity to be locked. </param>
		/// <param name="lockMode">The type of lock to be acquired. </param>
		/// <returns> The appropriate locking strategy. </returns>
		public virtual ILockingStrategy GetLockingStrategy(ILockable lockable, LockMode lockMode)
		{
			return new SelectLockingStrategy(lockable, lockMode);
		}

		/// <summary> 
		/// Given a lock mode, determine the appropriate for update fragment to use. 
		/// </summary>
		/// <param name="lockMode">The lock mode to apply. </param>
		/// <returns> The appropriate for update fragment. </returns>
		public virtual string GetForUpdateString(LockMode lockMode)
		{
			if (lockMode == LockMode.Upgrade)
			{
				return ForUpdateString;
			}
			if (lockMode == LockMode.UpgradeNoWait)
			{
				return ForUpdateNowaitString;
			}
			if (lockMode == LockMode.Force)
			{
				return ForUpdateNowaitString;
			}
			return string.Empty;
		}

		/// <summary>
		/// Get the string to append to SELECT statements to acquire locks
		/// for this dialect.
		/// </summary>
		/// <value>The appropriate <c>FOR UPDATE</c> clause string.</value>
		public virtual string ForUpdateString
		{
			get { return " for update"; }
		}

		/// <summary> Is <tt>FOR UPDATE OF</tt> syntax supported? </summary>
		/// <value> True if the database supports <tt>FOR UPDATE OF</tt> syntax; false otherwise. </value>
		public virtual bool ForUpdateOfColumns
		{
			// by default we report no support
			get { return false; }
		}

		/// <summary> 
		/// Does this dialect support <tt>FOR UPDATE</tt> in conjunction with outer joined rows?
		/// </summary>
		/// <value> True if outer joined rows can be locked via <tt>FOR UPDATE</tt>. </value>
		public virtual bool SupportsOuterJoinForUpdate
		{
			get { return true; }
		}

		/// <summary> 
		/// Get the <tt>FOR UPDATE OF column_list</tt> fragment appropriate for this
		/// dialect given the aliases of the columns to be write locked.
		///  </summary>
		/// <param name="aliases">The columns to be write locked. </param>
		/// <returns> The appropriate <tt>FOR UPDATE OF column_list</tt> clause string. </returns>
		public virtual string GetForUpdateString(string aliases)
		{
			// by default we simply return the ForUpdateString result since
			// the default is to say no support for "FOR UPDATE OF ..."
			return ForUpdateString;
		}

		/// <summary>
		/// Retrieves the <c>FOR UPDATE NOWAIT</c> syntax specific to this dialect
		/// </summary>
		/// <value>The appropriate <c>FOR UPDATE NOWAIT</c> clause string.</value>
		public virtual string ForUpdateNowaitString
		{
			// by default we report no support for NOWAIT lock semantics
			get { return ForUpdateString; }
		}

		/// <summary> 
		/// Get the <tt>FOR UPDATE OF column_list NOWAIT</tt> fragment appropriate
		/// for this dialect given the aliases of the columns to be write locked.
		/// </summary>
		/// <param name="aliases">The columns to be write locked. </param>
		/// <returns> The appropriate <tt>FOR UPDATE colunm_list NOWAIT</tt> clause string. </returns>
		public virtual string GetForUpdateNowaitString(string aliases)
		{
			return GetForUpdateString(aliases);
		}

		/// <summary> 
		/// Modifies the given SQL by applying the appropriate updates for the specified
		/// lock modes and key columns.
		/// </summary>
		/// <param name="sql">the SQL string to modify </param>
		/// <param name="aliasedLockModes">a map of lock modes indexed by aliased table names. </param>
		/// <param name="keyColumnNames">a map of key columns indexed by aliased table names. </param>
		/// <returns> the modified SQL string. </returns>
		/// <remarks>
		/// The behavior here is that of an ANSI SQL <tt>SELECT FOR UPDATE</tt>.  This
		/// method is really intended to allow dialects which do not support
		/// <tt>SELECT FOR UPDATE</tt> to achieve this in their own fashion.
		/// </remarks>
		public virtual SqlString ApplyLocksToSql(SqlString sql, IDictionary<string, LockMode> aliasedLockModes, IDictionary<string, string[]> keyColumnNames)
		{
			return sql.Append(new ForUpdateFragment(this, aliasedLockModes, keyColumnNames).ToSqlStringFragment());
		}

		/// <summary> 
		/// Some dialects support an alternative means to <tt>SELECT FOR UPDATE</tt>,
		/// whereby a "lock hint" is appends to the table name in the from clause.
		///  </summary>
		/// <param name="lockMode">The lock mode to apply </param>
		/// <param name="tableName">The name of the table to which to apply the lock hint. </param>
		/// <returns> The table with any required lock hints. </returns>
		public virtual string AppendLockHint(LockMode lockMode, string tableName)
		{
			return tableName;
		}

		#endregion

		#region Table support

		/// <summary>
		/// Return SQL needed to drop the named table. May (and should) use
		/// some form of "if exists" clause, and cascade constraints.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string GetDropTableString(string tableName)
		{
			var buf = new StringBuilder("drop table ");
			if (SupportsIfExistsBeforeTableName)
			{
				buf.Append("if exists ");
			}

			buf.Append(tableName).Append(CascadeConstraintsString);

			if (SupportsIfExistsAfterTableName)
			{
				buf.Append(" if exists");
			}
			return buf.ToString();
		}

		#region Temporary table support

		/// <summary> Does this dialect support temporary tables? </summary>
		public virtual bool SupportsTemporaryTables
		{
			get { return false; }
		}

		/// <summary> Generate a temporary table name given the bas table. </summary>
		/// <param name="baseTableName">The table name from which to base the temp table name. </param>
		/// <returns> The generated temp table name. </returns>
		public virtual string GenerateTemporaryTableName(string baseTableName)
		{
			return "HT_" + baseTableName;
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
		public virtual bool? PerformTemporaryTableDDLInIsolation()
		{
			return null;
		}

		/// <summary> Do we need to drop the temporary table after use? </summary>
		public virtual bool DropTemporaryTableAfterUse()
		{
			return true;
		}

		#endregion

		#endregion

		#region Callable statement support

		/// <summary> 
		/// Registers an OUT parameter which will be returning a
		/// <see cref="DbDataReader"/>.  How this is accomplished varies greatly
		/// from DB to DB, hence its inclusion (along with {@link #getResultSet}) here.
		///  </summary>
		/// <param name="statement">The callable statement. </param>
		/// <param name="position">The bind position at which to register the OUT param. </param>
		/// <returns> The number of (contiguous) bind positions used. </returns>
		public virtual int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			throw new NotSupportedException(GetType().FullName + " does not support resultsets via stored procedures");
		}

		/// <summary> 
		/// Given a callable statement previously processed by <see cref="RegisterResultSetOutParameter"/>,
		/// extract the <see cref="DbDataReader"/> from the OUT parameter. 
		/// </summary>
		/// <param name="statement">The callable statement. </param>
		/// <returns> The extracted result set. </returns>
		/// <throws>  SQLException Indicates problems extracting the result set. </throws>
		public virtual DbDataReader GetResultSet(DbCommand statement)
		{
			throw new NotSupportedException(GetType().FullName + " does not support resultsets via stored procedures");
		}

		#endregion

		#region Current timestamp support

		/// <summary> Does this dialect support a way to retrieve the database's current timestamp value? </summary>
		public virtual bool SupportsCurrentTimestampSelection
		{
			get { return false; }
		}

		/// <summary>
		/// Gives the best resolution that the database can use for storing
		/// date/time values, in ticks.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For example, if the database can store values with 100-nanosecond
		/// precision, this property is equal to 1L. If the database can only
		/// store values with 1-millisecond precision, this property is equal
		/// to 10000L (number of ticks in a millisecond).
		/// </para>
		/// <para>
		/// Used in TimestampType.
		/// </para>
		/// </remarks>
		public virtual long TimestampResolutionInTicks
		{
			get { return 1L; } // Maximum precision (one tick)
		}

		#endregion

		#region Constraint support

		/// <summary>
		/// The syntax used to drop a foreign key constraint from a table.
		/// </summary>
		/// <param name="constraintName">The name of the foreign key constraint to drop.</param>
		/// <returns>
		/// The SQL string to drop the foreign key constraint.
		/// </returns>
		public virtual string GetDropForeignKeyConstraintString(string constraintName)
		{
			return " drop constraint " + constraintName;
		}

		/// <summary>
		/// The syntax that is used to check if a constraint does not exists before creating it
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual string GetIfNotExistsCreateConstraint(Table table, string name)
		{
			return "";
		}

		/// <summary>
		/// The syntax that is used to close the if for a constraint exists check, used
		/// for dialects that requires begin/end for ifs
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual string GetIfNotExistsCreateConstraintEnd(Table table, string name)
		{
			return "";
		}

		/// <summary>
		/// The syntax that is used to check if a constraint exists before dropping it
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual string GetIfExistsDropConstraint(Table table, string name)
		{
			return "";
		}

		/// <summary>
		/// The syntax that is used to close the if for a constraint exists check, used
		/// for dialects that requires begin/end for ifs
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual string GetIfExistsDropConstraintEnd(Table table, string name)
		{
			return "";
		}

		/// <summary>
		/// The syntax used to drop a primary key constraint from a table.
		/// </summary>
		/// <param name="constraintName">The name of the primary key constraint to drop.</param>
		/// <returns>
		/// The SQL string to drop the primary key constraint.
		/// </returns>
		public virtual string GetDropPrimaryKeyConstraintString(string constraintName)
		{
			return " drop constraint " + constraintName;
		}

		/// <summary>
		/// The syntax used to drop an index constraint from a table.
		/// </summary>
		/// <param name="constraintName">The name of the index constraint to drop.</param>
		/// <returns>
		/// The SQL string to drop the primary key constraint.
		/// </returns>
		public virtual string GetDropIndexConstraintString(string constraintName)
		{
			return " drop constraint " + constraintName;
		}

		/// <summary>
		/// Completely optional cascading drop clause
		/// </summary>
		public virtual string CascadeConstraintsString
		{
			get { return String.Empty; }
		}

		/// <summary> Only needed if the Dialect does not have SupportsForeignKeyConstraintInAlterTable. </summary>
		public virtual string DisableForeignKeyConstraintsString
		{
			get { return null; }
		}

		/// <summary> Only needed if the Dialect does not have SupportsForeignKeyConstraintInAlterTable. </summary>
		public virtual string EnableForeignKeyConstraintsString
		{
			get { return null; }
		}

		#endregion

		#region Native identifier generation

		#region IDENTITY support

		/// <summary>
		/// Does this dialect support identity column key generation?
		/// </summary>
		public virtual bool SupportsIdentityColumns
		{
			get { return false; }
		}

		/// <summary> 
		/// Does the dialect support some form of inserting and selecting
		/// the generated IDENTITY value all in the same statement.
		///  </summary>
		public virtual bool SupportsInsertSelectIdentity
		{
			get { return false; }
		}

		/// <summary>
		/// Whether this dialect has an identity clause added to the data type or a
		/// completely separate identity data type.
		/// </summary>
		public virtual bool HasDataTypeInIdentityColumn
		{
			get { return true; }
		}

		/// <summary> 
		/// Provided we <see cref="SupportsInsertSelectIdentity"/>, then attch the
		/// "select identity" clause to the  insert statement.
		/// </summary>
		/// <param name="insertString">The insert command </param>
		/// <returns> 
		/// The insert command with any necessary identity select clause attached.
		/// Note, if <see cref="SupportsInsertSelectIdentity"/> == false then
		/// the insert-string should be returned without modification.
		/// </returns>
		public virtual SqlString AppendIdentitySelectToInsert(SqlString insertString)
		{
			return insertString;
		}

		/// <summary> 
		/// Get the select command to use to retrieve the last generated IDENTITY
		/// value for a particular table 
		/// </summary>
		/// <param name="tableName">The table into which the insert was done </param>
		/// <param name="identityColumn">The PK column. </param>
		/// <param name="type">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate select command </returns>
		public virtual string GetIdentitySelectString(string identityColumn, string tableName, DbType type)
		{
			return IdentitySelectString;
		}

		/// <summary> 
		/// Get the select command to use to retrieve the last generated IDENTITY value.
		/// </summary>
		/// <returns> The appropriate select command </returns>
		public virtual string IdentitySelectString
		{
			get { throw new MappingException("Dialect does not support identity key generation"); }
		}

		/// <summary> 
		/// The syntax used during DDL to define a column as being an IDENTITY of
		/// a particular type. 
		/// </summary>
		/// <param name="type">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate DDL fragment. </returns>
		public virtual string GetIdentityColumnString(DbType type)
		{
			return IdentityColumnString;
		}

		/// <summary>
		/// The keyword used to specify an identity column, if native key generation is supported
		/// </summary>
		public virtual string IdentityColumnString
		{
			get { throw new MappingException("Dialect does not support identity key generation"); }
		}

		/// <summary>
		/// Set this to false if no table-level primary key constraint should be generated when an identity column has been specified for the table.
		/// This is used as a work-around for SQLite so it doesn't tell us we have "more than one primary key".
		/// </summary>
		public virtual bool GenerateTablePrimaryKeyConstraintForIdentityColumn
		{
			get { return true; }
		}

		public virtual SqlString AddIdentifierOutParameterToInsert(SqlString insertString, string identifierColumnName, string parameterName)
		{
			return insertString;
		}

		public virtual InsertGeneratedIdentifierRetrievalMethod InsertGeneratedIdentifierRetrievalMethod
		{
			get { return InsertGeneratedIdentifierRetrievalMethod.ReturnValueParameter; }
		}

		/// <summary>
		/// The keyword used to insert a generated value into an identity column (or null).
		/// Need if the dialect does not support inserts that specify no column values.
		/// </summary>
		public virtual string IdentityInsertString
		{
			get { return null; }
		}

		#endregion

		#region SEQUENCE support

		/// <summary>
		/// Does this dialect support sequences?
		/// </summary>
		public virtual bool SupportsSequences
		{
			get { return false; }
		}

		/// <summary> 
		/// Does this dialect support "pooled" sequences?
		/// </summary>
		/// <returns> True if such "pooled" sequences are supported; false otherwise. </returns>
		/// <remarks>
		/// A pooled sequence is one that has a configurable initial size and increment 
		/// size. It enables NHibernate to be allocated a pool/block/range of IDs,
		/// which can reduce the frequency of round trips to the database during ID
		/// generation.
		/// </remarks>
		/// <seealso cref="GetCreateSequenceStrings(string, int, int)"> </seealso>
		/// <seealso cref="GetCreateSequenceString(string, int, int)"> </seealso>
		public virtual bool SupportsPooledSequences
		{
			get { return false; }
		}

		/// <summary> 
		/// Generate the appropriate select statement to to retreive the next value
		/// of a sequence.
		/// </summary>
		/// <param name="sequenceName">the name of the sequence </param>
		/// <returns> String The "nextval" select string. </returns>
		/// <remarks>This should be a "stand alone" select statement.</remarks>
		public virtual string GetSequenceNextValString(string sequenceName)
		{
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary> 
		/// Typically dialects which support sequences can drop a sequence
		/// with a single command.  
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <returns> The sequence drop commands </returns>
		/// <remarks>
		/// This is convenience form of <see cref="GetDropSequenceStrings"/>
		/// to help facilitate that.
		/// 
		/// Dialects which support sequences and can drop a sequence in a
		/// single command need *only* override this method.  Dialects
		/// which support sequences but require multiple commands to drop
		/// a sequence should instead override <see cref="GetDropSequenceStrings"/>. 
		/// </remarks>
		public virtual string GetDropSequenceString(string sequenceName)
		{
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary> 
		/// The multiline script used to drop a sequence. 
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <returns> The sequence drop commands </returns>
		public virtual string[] GetDropSequenceStrings(string sequenceName)
		{
			return new string[] { GetDropSequenceString(sequenceName) };
		}

		/// <summary> 
		/// Generate the select expression fragment that will retrieve the next
		/// value of a sequence as part of another (typically DML) statement.
		/// </summary>
		/// <param name="sequenceName">the name of the sequence </param>
		/// <returns> The "nextval" fragment. </returns>
		/// <remarks>
		/// This differs from <see cref="GetSequenceNextValString"/> in that this
		/// should return an expression usable within another statement.
		/// </remarks>
		public virtual string GetSelectSequenceNextValString(string sequenceName)
		{
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary> 
		/// Typically dialects which support sequences can create a sequence
		/// with a single command.
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <returns> The sequence creation command </returns>
		/// <remarks>
		/// This is convenience form of <see cref="GetCreateSequenceStrings(string,int,int)"/> to help facilitate that.
		/// Dialects which support sequences and can create a sequence in a
		/// single command need *only* override this method.  Dialects
		/// which support sequences but require multiple commands to create
		/// a sequence should instead override <see cref="GetCreateSequenceStrings(string,int,int)"/>.
		/// </remarks>
		public virtual string GetCreateSequenceString(string sequenceName)
		{
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary> 
		/// An optional multi-line form for databases which <see cref="SupportsPooledSequences"/>. 
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <param name="initialValue">The initial value to apply to 'create sequence' statement </param>
		/// <param name="incrementSize">The increment value to apply to 'create sequence' statement </param>
		/// <returns> The sequence creation commands </returns>
		public virtual string[] GetCreateSequenceStrings(string sequenceName, int initialValue, int incrementSize)
		{
			return new string[] { GetCreateSequenceString(sequenceName, initialValue, incrementSize) };
		}

		/// <summary> 
		/// Overloaded form of <see cref="GetCreateSequenceString(string)"/>, additionally
		/// taking the initial value and increment size to be applied to the sequence
		/// definition.
		///  </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <param name="initialValue">The initial value to apply to 'create sequence' statement </param>
		/// <param name="incrementSize">The increment value to apply to 'create sequence' statement </param>
		/// <returns> The sequence creation command </returns>
		/// <remarks>
		/// The default definition is to suffix <see cref="GetCreateSequenceString(string,int,int)"/>
		/// with the string: " start with {initialValue} increment by {incrementSize}" where
		/// {initialValue} and {incrementSize} are replacement placeholders.  Generally
		/// dialects should only need to override this method if different key phrases
		/// are used to apply the allocation information.
		/// </remarks>
		protected virtual string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			if (SupportsPooledSequences)
			{
				return GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
			}
			throw new MappingException("Dialect does not support pooled sequences");
		}

		/// <summary> Get the select command used retrieve the names of all sequences.</summary>
		/// <returns> The select command; or null if sequences are not supported. </returns>
		public virtual string QuerySequencesString
		{
			get { return null; }
		}

		#endregion

		/// <summary> 
		/// The class (which implements <see cref="NHibernate.Id.IIdentifierGenerator"/>)
		/// which acts as this dialects identity-style generation strategy.
		/// </summary>
		/// <returns> The native generator class. </returns>
		/// <remarks>
		/// Comes into play whenever the user specifies the "identity" generator.
		/// </remarks>
		public virtual System.Type IdentityStyleIdentifierGeneratorClass
		{
			get
			{
				if (SupportsIdentityColumns)
				{
					return typeof(IdentityGenerator);
				}
				if (SupportsSequences)
				{
					return typeof(SequenceIdentityGenerator);
				}
				return typeof(TriggerIdentityGenerator);
			}
		}

		/// <summary> 
		/// The class (which implements <see cref="NHibernate.Id.IIdentifierGenerator"/>)
		/// which acts as this dialects native generation strategy.
		/// </summary>
		/// <returns> The native generator class. </returns>
		/// <remarks>
		/// Comes into play whenever the user specifies the native generator.
		/// </remarks>
		public virtual System.Type NativeIdentifierGeneratorClass
		{
			get
			{
				if (SupportsIdentityColumns)
				{
					return typeof(IdentityGenerator);
				}
				if (SupportsSequences)
				{
					return typeof(SequenceGenerator);
				}
				return typeof(TableHiLoGenerator);
			}
		}

		#endregion

		#region Miscellaneous support

		/// <summary> 
		/// Create a <see cref="JoinFragment"/> strategy responsible
		/// for handling this dialect's variations in how joins are handled. 
		/// </summary>
		/// <returns> This dialect's <see cref="JoinFragment"/> strategy. </returns>
		public virtual JoinFragment CreateOuterJoinFragment()
		{
			return new ANSIJoinFragment();
		}

		/// <summary> 
		/// Create a <see cref="CaseFragment"/> strategy responsible
		/// for handling this dialect's variations in how CASE statements are
		/// handled. 
		/// </summary>
		/// <returns> This dialect's <see cref="CaseFragment"/> strategy. </returns>
		public virtual CaseFragment CreateCaseFragment()
		{
			return new ANSICaseFragment(this);
		}

		/// <summary> The SQL literal value to which this database maps boolean values. </summary>
		/// <param name="value">The boolean value </param>
		/// <returns> The appropriate SQL literal. </returns>
		public virtual string ToBooleanValueString(bool value)
		{
			return value ? "1" : "0";
		}

		internal static void ExtractColumnOrAliasNames(SqlString select, out List<SqlString> columnsOrAliases, out Dictionary<SqlString, SqlString> aliasToColumn, out Dictionary<SqlString, SqlString> columnToAlias)
		{
			columnsOrAliases = new List<SqlString>();
			aliasToColumn = new Dictionary<SqlString, SqlString>();
			columnToAlias = new Dictionary<SqlString, SqlString>();

			var tokens = new QuotedAndParenthesisStringTokenizer(select).GetTokens();
			int index = 0;
			while (index < tokens.Count)
			{
				var token = tokens[index];

				int nextTokenIndex = index += 1;

				if (token.EqualsCaseInsensitive("select"))
					continue;

				if (token.EqualsCaseInsensitive("distinct"))
					continue;

				if (token.EqualsCaseInsensitive(","))
					continue;

				if (token.EqualsCaseInsensitive("from"))
					break;

				// handle composite expressions like "2 * 4 as foo"
				while ((nextTokenIndex < tokens.Count)
					&& (tokens[nextTokenIndex].EqualsCaseInsensitive("as") == false
					&& tokens[nextTokenIndex].EqualsCaseInsensitive("from") == false
					&& tokens[nextTokenIndex].EqualsCaseInsensitive(",") == false))
				{
					SqlString nextToken = tokens[nextTokenIndex];
					token = token.Append(nextToken);
					nextTokenIndex = index += 1;
				}

				// if there is no alias, the token and the alias will be the same
				SqlString alias = token;

				bool isFunctionCallOrQuotedString = token.IndexOfCaseInsensitive("'") >= 0 || token.IndexOfCaseInsensitive("(") >= 0;

				// this is heuristic guess, if the expression contains ' or (, it is probably
				// not appropriate to just slice parts off of it
				if (isFunctionCallOrQuotedString == false)
				{
					// its a simple column reference, so lets set the alias to the
					// column name minus the table qualifier if it exists
					int dot = token.IndexOfCaseInsensitive(".");
					if (dot != -1)
						alias = token.Substring(dot + 1);
				}

				// notice! we are checking here the existence of "as" "alias", two
				// tokens from the current one
				if (nextTokenIndex + 1 < tokens.Count)
				{
					SqlString nextToken = tokens[nextTokenIndex];
					if (nextToken.IndexOfCaseInsensitive("as") >= 0)
					{
						SqlString tokenAfterNext = tokens[nextTokenIndex + 1];
						alias = tokenAfterNext;
						index += 2; //skip the "as" and the alias
					}
				}

				columnsOrAliases.Add(alias);
				aliasToColumn[alias] = token;
				columnToAlias[token] = alias;
			}
		}

		/// <summary>
		/// This specialized string tokenizier will break a string to tokens, taking
		/// into account single quotes, parenthesis and commas and [ ]
		/// Notice that we aren't differentiating between [ ) and ( ] on purpose, it would complicate
		/// the code and it is not legal at any rate.
		/// </summary>
		public class QuotedAndParenthesisStringTokenizer : IEnumerable<SqlString>
		{
			private readonly SqlString _original;

			public QuotedAndParenthesisStringTokenizer(SqlString original)
			{
				_original = original;
			}

			IEnumerator<SqlString> IEnumerable<SqlString>.GetEnumerator()
			{
				TokenizerState state = TokenizerState.WhiteSpace;
				int parenthesisCount = 0;
				bool escapeQuote = false;
				char quoteType = '\'';
				int tokenStart = 0;
				int tokenLength = 0;
				string originalString = _original.ToString();

				for (int i = 0; i < originalString.Length; i++)
				{
					char ch = originalString[i];
					switch (state)
					{
						case TokenizerState.WhiteSpace:
							if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								quoteType = '\'';
								tokenLength += 1;
							}
							else if (ch == '"')
							{
								state = TokenizerState.Quoted;
								quoteType = '"';
								tokenLength += 1;
							}
							else if (ch == ',')
							{
								yield return new SqlString(",");
								tokenStart += 1;
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								tokenLength += 1;
								parenthesisCount = 1;
							}
							else if (char.IsWhiteSpace(ch) == false)
							{
								state = TokenizerState.Token;
								tokenLength += 1;
							}
							else
							{
								tokenStart += 1;
							}
							break;
						case TokenizerState.Quoted:
							if (escapeQuote)
							{
								escapeQuote = false;
								tokenLength += 1;
							}
							// handle escaping of ' by using '' or \'
							else if (ch == '\\' || (ch == quoteType && i + 1 < originalString.Length && originalString[i + 1] == quoteType))
							{
								escapeQuote = true;
								tokenLength += 1;
							}
							else if (ch == quoteType)
							{
								yield return _original.Substring(tokenStart, tokenLength + 1);
								tokenStart += tokenLength + 1;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						case TokenizerState.InParenthesis:
							if (ch == ')' || ch == ']')
							{
								tokenLength += 1;
								parenthesisCount -= 1;
								if (parenthesisCount == 0)
								{
									yield return _original.Substring(tokenStart, tokenLength);
									tokenStart += tokenLength;
									tokenLength = 0;
									state = TokenizerState.WhiteSpace;
								}
							}
							else if (ch == '(' || ch == '[')
							{
								tokenLength += 1;
								parenthesisCount += 1;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						case TokenizerState.Token:
							if (char.IsWhiteSpace(ch))
							{
								yield return _original.Substring(tokenStart, tokenLength);
								tokenStart += tokenLength + 1;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == ',') // stop current token, and send the , as well
							{
								yield return _original.Substring(tokenStart, tokenLength);
								yield return new SqlString(",");
								tokenStart += tokenLength + 1;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								parenthesisCount = 1;
								tokenLength += 1;
							}
							else if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								quoteType = '\'';
								tokenLength += 1;
							}
							else if (ch == '"')
							{
								state = TokenizerState.Quoted;
								quoteType = '"';
								tokenLength += 1;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						default:
							throw new InvalidExpressionException("Could not understand the string " + _original);
					}
				}
				if (tokenLength > 0)
				{
					yield return _original.Substring(tokenStart, tokenLength);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return ((IEnumerable<SqlString>)this).GetEnumerator();
			}

			public enum TokenizerState
			{
				WhiteSpace,
				Quoted,
				InParenthesis,
				Token
			}

			public IList<SqlString> GetTokens()
			{
				return new List<SqlString>(this);
			}
		}

		#endregion

		#region Limit/offset support

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>False, unless overridden.</value>
		public virtual bool SupportsLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public virtual bool SupportsLimitOffset
		{
			get { return SupportsLimit; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public virtual bool SupportsVariableLimit
		{
			get { return SupportsLimit; }
		}

		/// <summary> 
		/// Does the <tt>LIMIT</tt> clause take a "maximum" row number instead
		/// of a total number of returned rows?
		/// </summary>
		/// <returns> True if limit is relative from offset; false otherwise. </returns>
		/// <remarks>
		/// This is easiest understood via an example.  Consider you have a table
		/// with 20 rows, but you only want to retrieve rows number 11 through 20.
		/// Generally, a limit with offset would say that the offset = 11 and the
		/// limit = 10 (we only want 10 rows at a time); this is specifying the
		/// total number of returned rows.  Some dialects require that we instead
		/// specify offset = 11 and limit = 20, where 20 is the "last" row we want
		/// relative to offset (i.e. total number of rows = 20 - 11 = 9)
		/// So essentially, is limit relative from offset?  Or is limit absolute?
		/// </remarks>
		public virtual bool UseMaxForLimit
		{
			get { return false; }
		}

		/// <summary>
		/// For limit clauses, indicates whether to use 0 or 1 as the offset that returns the first row.  Should be true if the first row is at offset 1.
		/// </summary>
		public virtual bool OffsetStartsAtOne
		{
			get { return false; }
		}

		/// <summary>
		/// Attempts to add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>.
		/// Expects any database-specific offset and limit adjustments to have already been performed (ex. UseMaxForLimit, OffsetStartsAtOne).
		/// </summary>
		/// <param name="queryString">The <see cref="SqlString"/> to base the limit query off.</param>
		/// <param name="offset">Offset of the first row to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no limit is requested.  This should have already been adjusted to account for OffsetStartsAtOne.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no offset is requested.  This should have already been adjusted to account for UseMaxForLimit.</param>
		/// <returns>A new <see cref="SqlString"/> that contains the <c>LIMIT</c> clause. Returns <c>null</c> 
		/// if <paramref name="queryString"/> represents a SQL statement to which a limit clause cannot be added, 
		/// for example when the query string is custom SQL invoking a stored procedure.</returns>
		public virtual SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			throw new NotSupportedException("Dialect does not have support for limit strings.");
		}

		/// <summary>
		/// Attempts to generate a string to limit the result set to a number of maximum results with a specified offset into the results.
		/// Expects any database-specific offset and limit adjustments to have already been performed (ex. UseMaxForLimit, OffsetStartsAtOne).
		/// Performs error checking based on the various dialect limit support options.  If both parameters and fixed valeus are
		/// specified, this will use the parameter option if possible.  Otherwise, it will fall back to a fixed string.
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="offsetParameter"></param>
		/// <param name="limitParameter"></param>
		/// <returns></returns>
		public SqlString GetLimitString(SqlString queryString, int? offset, int? limit, Parameter offsetParameter, Parameter limitParameter)
		{
			if (offset == null && limit == null && offsetParameter == null && limitParameter == null)
				return queryString;

			if (!SupportsLimit)
				throw new NotSupportedException("Dialect does not support limits.");

			if (!SupportsVariableLimit && offsetParameter != null && offset == null)
				throw new NotSupportedException("Dialect does not support variable limits.");

			if (!SupportsVariableLimit && limitParameter != null && limit == null)
				throw new NotSupportedException("Dialect does not support variable limits.");

			if (!SupportsLimitOffset && (offset != null || offsetParameter != null))
				throw new NotSupportedException("Dialect does not support limits with offsets.");

			SqlString o =
				SupportsVariableLimit && offsetParameter != null ? new SqlString(offsetParameter) :
				offset.HasValue ? new SqlString(offset.ToString()) :
				null;

			SqlString l =
				SupportsVariableLimit && limitParameter != null ? new SqlString(limitParameter) :
				limit.HasValue ? new SqlString(limit.ToString()) :
				null;

			return GetLimitString(queryString, o, l);
		}

		/// <summary>
		/// Some databases require that a limit statement contain the maximum row number
		/// instead of the number of rows to retrieve.  This method adjusts source
		/// limit and offset values to account for this.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		public int GetLimitValue(int offset, int limit)
		{
			if (limit == int.MaxValue)
				return int.MaxValue;

			if (UseMaxForLimit)
				return GetOffsetValue(offset) + limit;

			return limit;
		}

		/// <summary>
		/// Some databases use limit row offsets that start at one instead of zero.
		/// This method adjusts a desired offset using the OffsetStartsAtOne flag.
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		public int GetOffsetValue(int offset)
		{
			if (OffsetStartsAtOne)
				return offset + 1;

			return offset;
		}

		#endregion

		#region Identifier quoting support

		/// <summary>
		/// The opening quote for a quoted identifier.
		/// </summary>
		public virtual char OpenQuote
		{
			get { return '"'; }
		}

		/// <summary>
		/// The closing quote for a quoted identifier.
		/// </summary>
		public virtual char CloseQuote
		{
			get { return '"'; }
		}

		/// <summary>
		/// Checks to see if the name has been quoted.
		/// </summary>
		/// <param name="name">The name to check if it is quoted</param>
		/// <returns>true if name is already quoted.</returns>
		/// <remarks>
		/// The default implementation is to compare the first character
		/// to Dialect.OpenQuote and the last char to Dialect.CloseQuote
		/// </remarks>
		public virtual bool IsQuoted(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			return (name[0] == OpenQuote && name[name.Length - 1] == CloseQuote);
		}

		public virtual string Qualify(string catalog, string schema, string table)
		{
			StringBuilder qualifiedName = new StringBuilder();

			if (!string.IsNullOrEmpty(catalog))
			{
				qualifiedName.Append(catalog).Append(StringHelper.Dot);
			}
			if (!string.IsNullOrEmpty(schema))
			{
				qualifiedName.Append(schema).Append(StringHelper.Dot);
			}
			return qualifiedName.Append(table).ToString();
		}

		/// <summary>
		/// Quotes a name.
		/// </summary>
		/// <param name="name">The string that needs to be Quoted.</param>
		/// <returns>A QuotedName </returns>
		/// <remarks>
		/// <p>
		/// This method assumes that the name is not already Quoted.  So if the name passed
		/// in is <c>"name</c> then it will return <c>"""name"</c>.  It escapes the first char
		/// - the " with "" and encloses the escaped string with OpenQuote and CloseQuote. 
		/// </p>
		/// </remarks>
		protected virtual string Quote(string name)
		{
			string quotedName = name.Replace(OpenQuote.ToString(), new string(OpenQuote, 2));

			// in some dbs the Open and Close Quote are the same chars - if they are 
			// then we don't have to escape the Close Quote char because we already
			// got it.
			if (OpenQuote != CloseQuote)
			{
				quotedName = name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2));
			}

			return OpenQuote + quotedName + CloseQuote;
		}

		/// <summary>
		/// Quotes a name for being used as a aliasname
		/// </summary>
		/// <remarks>Original implementation calls <see cref="QuoteForTableName"/></remarks>
		/// <param name="aliasName">Name of the alias</param>
		/// <returns>A Quoted name in the format of OpenQuote + aliasName + CloseQuote</returns>
		/// <remarks>
		/// <p>
		/// If the aliasName is already enclosed in the OpenQuote and CloseQuote then this 
		/// method will return the aliasName that was passed in without going through any
		/// Quoting process.  So if aliasName is passed in already Quoted make sure that 
		/// you have escaped all of the chars according to your DataBase's specifications.
		/// </p>
		/// </remarks>
		public virtual string QuoteForAliasName(string aliasName)
		{
			return IsQuoted(aliasName) ? aliasName : Quote(aliasName);
		}

		/// <summary>
		/// Quotes a name for being used as a columnname
		/// </summary>
		/// <remarks>Original implementation calls <see cref="QuoteForTableName"/></remarks>
		/// <param name="columnName">Name of the column</param>
		/// <returns>A Quoted name in the format of OpenQuote + columnName + CloseQuote</returns>
		/// <remarks>
		/// <p>
		/// If the columnName is already enclosed in the OpenQuote and CloseQuote then this 
		/// method will return the columnName that was passed in without going through any
		/// Quoting process.  So if columnName is passed in already Quoted make sure that 
		/// you have escaped all of the chars according to your DataBase's specifications.
		/// </p>
		/// </remarks>
		public virtual string QuoteForColumnName(string columnName)
		{
			return IsQuoted(columnName) ? columnName : Quote(columnName);
		}

		/// <summary>
		/// Quotes a name for being used as a tablename
		/// </summary>
		/// <param name="tableName">Name of the table</param>
		/// <returns>A Quoted name in the format of OpenQuote + tableName + CloseQuote</returns>
		/// <remarks>
		/// <p>
		/// If the tableName is already enclosed in the OpenQuote and CloseQuote then this 
		/// method will return the tableName that was passed in without going through any
		/// Quoting process.  So if tableName is passed in already Quoted make sure that 
		/// you have escaped all of the chars according to your DataBase's specifications.
		/// </p>
		/// </remarks>
		public virtual string QuoteForTableName(string tableName)
		{
			return IsQuoted(tableName) ? tableName : Quote(tableName);
		}

		/// <summary>
		/// Quotes a name for being used as a schemaname
		/// </summary>
		/// <param name="schemaName">Name of the schema</param>
		/// <returns>A Quoted name in the format of OpenQuote + schemaName + CloseQuote</returns>
		/// <remarks>
		/// <p>
		/// If the schemaName is already enclosed in the OpenQuote and CloseQuote then this 
		/// method will return the schemaName that was passed in without going through any
		/// Quoting process.  So if schemaName is passed in already Quoted make sure that 
		/// you have escaped all of the chars according to your DataBase's specifications.
		/// </p>
		/// </remarks>
		public virtual string QuoteForSchemaName(string schemaName)
		{
			return IsQuoted(schemaName) ? schemaName : Quote(schemaName);
		}

		/// <summary>
		/// Unquotes and unescapes an already quoted name
		/// </summary>
		/// <param name="quoted">Quoted string</param>
		/// <returns>Unquoted string</returns>
		/// <remarks>
		/// <p>
		/// This method checks the string <c>quoted</c> to see if it is 
		/// quoted.  If the string <c>quoted</c> is already enclosed in the OpenQuote
		/// and CloseQuote then those chars are removed.
		/// </p>
		/// <p>
		/// After the OpenQuote and CloseQuote have been cleaned from the string <c>quoted</c>
		/// then any chars in the string <c>quoted</c> that have been escaped by doubling them
		/// up are changed back to a single version.
		/// </p>
		/// <p>
		/// The following quoted values return these results
		/// "quoted" = quoted
		/// "quote""d" = quote"d
		/// quote""d = quote"d 
		/// </p>
		/// <p>
		/// If this implementation is not sufficient for your Dialect then it needs to be overridden.
		/// MsSql2000Dialect is an example of where UnQuoting rules are different.
		/// </p>
		/// </remarks>
		public virtual string UnQuote(string quoted)
		{
			string unquoted;

			if (IsQuoted(quoted))
			{
				unquoted = quoted.Substring(1, quoted.Length - 2);
			}
			else
			{
				unquoted = quoted;
			}

			unquoted = unquoted.Replace(new string(OpenQuote, 2), OpenQuote.ToString());

			if (OpenQuote != CloseQuote)
			{
				unquoted = unquoted.Replace(new string(CloseQuote, 2), CloseQuote.ToString());
			}

			return unquoted;
		}

		/// <summary>
		/// Unquotes an array of Quoted Names.
		/// </summary>
		/// <param name="quoted">strings to Unquote</param>
		/// <returns>an array of unquoted strings.</returns>
		/// <remarks>
		/// This use UnQuote(string) for each string in the quoted array so
		/// it should not need to be overridden - only UnQuote(string) needs
		/// to be overridden unless this implementation is not sufficient.
		/// </remarks>
		public virtual string[] UnQuote(string[] quoted)
		{
			var unquoted = new string[quoted.Length];

			for (int i = 0; i < quoted.Length; i++)
			{
				unquoted[i] = UnQuote(quoted[i]);
			}

			return unquoted;
		}

		#endregion

		#region Union subclass support

		/// <summary> 
		/// Given a <see cref="DbType"/> type code, determine an appropriate
		/// null value to use in a select clause.
		/// </summary>
		/// <param name="sqlType">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate select clause value fragment. </returns>
		/// <remarks>
		/// One thing to consider here is that certain databases might
		/// require proper casting for the nulls here since the select here
		/// will be part of a UNION/UNION ALL.
		/// </remarks>
		public virtual string GetSelectClauseNullString(SqlType sqlType)
		{
			return "null";
		}

		/// <summary> 
		/// Does this dialect support UNION ALL, which is generally a faster variant of UNION? 
		/// True if UNION ALL is supported; false otherwise.
		/// </summary>
		public virtual bool SupportsUnionAll
		{
			get { return false; }
		}

		#endregion

		#region Informational metadata

		/// <summary> 
		/// Does this dialect support empty IN lists?
		/// For example, is [where XYZ in ()] a supported construct?
		/// </summary>
		/// <returns> True if empty in lists are supported; false otherwise. </returns>
		public virtual bool SupportsEmptyInList
		{
			get { return true; }
		}

		/// <summary> 
		/// Are string comparisons implicitly case insensitive.
		/// In other words, does [where 'XYZ' = 'xyz'] resolve to true? 
		/// </summary>
		/// <returns> True if comparisons are case insensitive. </returns>
		public virtual bool AreStringComparisonsCaseInsensitive
		{
			get { return false; }
		}

		/// <summary> 
		/// Is this dialect known to support what ANSI-SQL terms "row value
		/// constructor" syntax; sometimes called tuple syntax.
		/// <p/>
		/// Basically, does it support syntax like
		/// "... where (FIRST_NAME, LAST_NAME) = ('Steve', 'Ebersole') ...". 
		/// </summary>
		/// <returns> 
		/// True if this SQL dialect is known to support "row value
		/// constructor" syntax; false otherwise.
		/// </returns>
		public virtual bool SupportsRowValueConstructorSyntax
		{
			// return false here, as most databases do not properly support this construct...
			get { return false; }
		}

		/// <summary> 
		/// If the dialect supports {@link #supportsRowValueConstructorSyntax() row values},
		/// does it offer such support in IN lists as well?
		/// <p/>
		/// For example, "... where (FIRST_NAME, LAST_NAME) IN ( (?, ?), (?, ?) ) ..." 
		/// </summary>
		/// <returns> 
		/// True if this SQL dialect is known to support "row value
		/// constructor" syntax in the IN list; false otherwise.
		/// </returns>
		public virtual bool SupportsRowValueConstructorSyntaxInInList
		{
			get { return false; }
		}

		/// <summary> 
		/// Should LOBs (both BLOB and CLOB) be bound using stream operations (i.e.
		/// {@link java.sql.PreparedStatement#setBinaryStream}). 
		/// </summary>
		/// <returns> True if BLOBs and CLOBs should be bound using stream operations. </returns>
		public virtual bool UseInputStreamToInsertBlob
		{
			get { return true; }
		}

		/// <summary> 
		/// Does this dialect support parameters within the select clause of
		/// INSERT ... SELECT ... statements? 
		/// </summary>
		/// <returns> True if this is supported; false otherwise. </returns>
		public virtual bool SupportsParametersInInsertSelect
		{
			get { return true; }
		}

		/// <summary> 
		/// Does this dialect require that references to result variables
		/// (i.e, select expresssion aliases) in an ORDER BY clause be
		/// replaced by column positions (1-origin) as defined by the select clause?
		/// </summary>
		/// <returns> 
		/// true if result variable references in the ORDER BY clause should 
		/// be replaced by column positions; false otherwise. 
		/// </returns>
		public virtual bool ReplaceResultVariableInOrderByClauseWithPosition
		{
			get { return false; }
		}

		/// <summary> 
		/// Does this dialect support asking the result set its positioning
		/// information on forward only cursors.  Specifically, in the case of
		/// scrolling fetches, Hibernate needs to use
		/// {@link java.sql.ResultSet#isAfterLast} and
		/// {@link java.sql.ResultSet#isBeforeFirst}.  Certain drivers do not
		/// allow access to these methods for forward only cursors.
		/// <p/>
		/// NOTE : this is highly driver dependent! 
		/// </summary>
		/// <returns> 
		/// True if methods like {@link java.sql.ResultSet#isAfterLast} and
		/// {@link java.sql.ResultSet#isBeforeFirst} are supported for forward
		/// only cursors; false otherwise.
		/// </returns>
		public virtual bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor
		{
			get { return true; }
		}

		/// <summary> 
		/// Does this dialect support definition of cascade delete constraints
		/// which can cause circular chains? 
		/// </summary>
		/// <returns> True if circular cascade delete constraints are supported; false otherwise. </returns>
		public virtual bool SupportsCircularCascadeDeleteConstraints
		{
			get { return true; }
		}

		/// <summary> 
		/// Are subselects supported as the left-hand-side (LHS) of
		/// IN-predicates.
		/// <para/>
		/// In other words, is syntax like "... {subquery} IN (1, 2, 3) ..." supported? 
		/// </summary>
		/// <returns> True if subselects can appear as the LHS of an in-predicate;false otherwise. </returns>
		public virtual bool SupportsSubselectAsInPredicateLHS
		{
			get { return true; }
		}

		/// <summary> 
		/// Expected LOB usage pattern is such that I can perform an insert
		/// via prepared statement with a parameter binding for a LOB value
		/// without crazy casting to JDBC driver implementation-specific classes...
		/// <p/>
		/// Part of the trickiness here is the fact that this is largely
		/// driver dependent.  For example, Oracle (which is notoriously bad with
		/// LOB support in their drivers historically) actually does a pretty good
		/// job with LOB support as of the 10.2.x versions of their drivers... 
		/// </summary>
		/// <returns> 
		/// True if normal LOB usage patterns can be used with this driver;
		/// false if driver-specific hookiness needs to be applied.
		/// </returns>
		public virtual bool SupportsExpectedLobUsagePattern
		{
			get { return true; }
		}

		/// <summary> Does the dialect support propagating changes to LOB
		/// values back to the database?  Talking about mutating the
		/// internal value of the locator as opposed to supplying a new
		/// locator instance...
		/// <p/>
		/// For BLOBs, the internal value might be changed by:
		/// {@link java.sql.Blob#setBinaryStream},
		/// {@link java.sql.Blob#setBytes(long, byte[])},
		/// {@link java.sql.Blob#setBytes(long, byte[], int, int)},
		/// or {@link java.sql.Blob#truncate(long)}.
		/// <p/>
		/// For CLOBs, the internal value might be changed by:
		/// {@link java.sql.Clob#setAsciiStream(long)},
		/// {@link java.sql.Clob#setCharacterStream(long)},
		/// {@link java.sql.Clob#setString(long, String)},
		/// {@link java.sql.Clob#setString(long, String, int, int)},
		/// or {@link java.sql.Clob#truncate(long)}.
		/// <p/>
		/// NOTE : I do not know the correct answer currently for
		/// databases which (1) are not part of the cruise control process
		/// or (2) do not {@link #supportsExpectedLobUsagePattern}. 
		/// </summary>
		/// <returns> True if the changes are propagated back to the database; false otherwise. </returns>
		public virtual bool SupportsLobValueChangePropogation
		{
			get { return true; }
		}

		/// <summary> 
		/// Is it supported to materialize a LOB locator outside the transaction in
		/// which it was created?
		/// <p/>
		/// Again, part of the trickiness here is the fact that this is largely
		/// driver dependent.
		/// <p/>
		/// NOTE: all database I have tested which {@link #supportsExpectedLobUsagePattern()}
		/// also support the ability to materialize a LOB outside the owning transaction... 
		/// </summary>
		/// <returns> True if unbounded materialization is supported; false otherwise. </returns>
		public virtual bool SupportsUnboundedLobLocatorMaterialization
		{
			get { return true; }
		}

		/// <summary> 
		/// Does this dialect support referencing the table being mutated in
		/// a subquery.  The "table being mutated" is the table referenced in
		/// an UPDATE or a DELETE query.  And so can that table then be
		/// referenced in a subquery of said UPDATE/DELETE query.
		/// <p/>
		/// For example, would the following two syntaxes be supported:<ul>
		/// <li>delete from TABLE_A where ID not in ( select ID from TABLE_A )</li>
		/// <li>update TABLE_A set NON_ID = 'something' where ID in ( select ID from TABLE_A)</li>
		/// </ul>
		///  </summary>
		/// <returns> True if this dialect allows references the mutating table from a subquery. </returns>
		public virtual bool SupportsSubqueryOnMutatingTable
		{
			get { return true; }
		}

		/// <summary> Does the dialect support an exists statement in the select clause? </summary>
		/// <returns> True if exists checks are allowed in the select clause; false otherwise. </returns>
		public virtual bool SupportsExistsInSelect
		{
			get { return true; }
		}

		/// <summary> 
		/// For the underlying database, is READ_COMMITTED isolation implemented by
		/// forcing readers to wait for write locks to be released? 
		/// </summary>
		/// <returns> True if writers block readers to achieve READ_COMMITTED; false otherwise. </returns>
		public virtual bool DoesReadCommittedCauseWritersToBlockReaders
		{
			get { return false; }
		}

		/// <summary> 
		/// For the underlying database, is REPEATABLE_READ isolation implemented by
		/// forcing writers to wait for read locks to be released? 
		/// </summary>
		/// <returns> True if readers block writers to achieve REPEATABLE_READ; false otherwise. </returns>
		public virtual bool DoesRepeatableReadCauseReadersToBlockWriters
		{
			get { return false; }
		}

		/// <summary> 
		/// Does this dialect support using a JDBC bind parameter as an argument
		/// to a function or procedure call? 
		/// </summary>
		/// <returns> True if the database supports accepting bind params as args; false otherwise. </returns>
		public virtual bool SupportsBindAsCallableArgument
		{
			get { return true; }
		}

		#endregion

		/// <summary>
		/// Does this dialect support subselects?
		/// </summary>
		public virtual bool SupportsSubSelects
		{
			get { return true; }
		}

		/// <summary>
		/// Retrieve a set of default Hibernate properties for this database.
		/// </summary>
		public IDictionary<string, string> DefaultProperties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Aggregate SQL functions as defined in general. This is
		/// a case-insensitive hashtable!
		/// </summary>
		/// <remarks>
		/// The results of this method should be integrated with the 
		/// specialization's data.
		/// </remarks>
		public virtual IDictionary<string, ISQLFunction> Functions
		{
			get { return _sqlFunctions; }
		}

		public HashSet<string> Keywords
		{
			get { return _sqlKeywords; }
		}

		/// <summary> 
		/// Get the command used to select a GUID from the underlying database.
		/// (Optional operation.)
		///  </summary>
		/// <returns> The appropriate command. </returns>
		public virtual string SelectGUIDString
		{
			get { throw new NotSupportedException("dialect does not support server side GUIDs generation."); }
		}

		/// <summary> Command used to create a table. </summary>
		public virtual string CreateTableString
		{
			get { return "create table"; }
		}

		/// <summary> 
		/// Slight variation on <see cref="CreateTableString"/>.
		/// The command used to create a multiset table. 
		/// </summary>
		/// <remarks>
		/// Here, we have the command used to create a table when there is no primary key and
		/// duplicate rows are expected.
		/// <p/>
		/// Most databases do not care about the distinction; originally added for
		/// Teradata support which does care.
		/// </remarks>
		public virtual string CreateMultisetTableString
		{
			get { return CreateTableString; }
		}

		/// <summary> Command used to create a temporary table. </summary>
		public virtual string CreateTemporaryTableString
		{
			get { return "create table"; }
		}

		/// <summary> 
		/// Get any fragments needing to be postfixed to the command for
		/// temporary table creation. 
		/// </summary>
		public virtual string CreateTemporaryTablePostfix
		{
			get { return string.Empty; }
		}

		/// <summary> 
		/// Should the value returned by <see cref="CurrentTimestampSelectString"/>
		/// be treated as callable.  Typically this indicates that JDBC escape
		/// sytnax is being used...
		/// </summary>
		public virtual bool IsCurrentTimestampSelectStringCallable
		{
			get { throw new NotSupportedException("Database not known to define a current timestamp function"); }
		}

		/// <summary> 
		/// Retrieve the command used to retrieve the current timestammp from the database. 
		/// </summary>
		public virtual string CurrentTimestampSelectString
		{
			get { throw new NotSupportedException("Database not known to define a current timestamp function"); }
		}

		/// <summary> 
		/// The name of the database-specific SQL function for retrieving the
		/// current timestamp. 
		/// </summary>
		public virtual string CurrentTimestampSQLFunctionName
		{
			get { return "current_timestamp"; }
		}

		public virtual IViolatedConstraintNameExtracter ViolatedConstraintNameExtracter
		{
			get { return Extracter; }
		}

		/// <summary>
		/// The keyword used to insert a row without specifying any column values
		/// </summary>
		public virtual string NoColumnsInsertString
		{
			get { return "values ( )"; }
		}

		/// <summary>
		/// The name of the SQL function that transforms a string to lowercase
		/// </summary>
		public virtual string LowercaseFunction
		{
			get { return "lower"; }
		}

		public virtual int MaxAliasLength
		{
			get { return 10; }
		}

		/// <summary>
		/// The syntax used to add a column to a table. Note this is deprecated
		/// </summary>
		public virtual string AddColumnString
		{
			get { throw new NotSupportedException("No add column syntax supported by Dialect"); }
		}

		public virtual string DropForeignKeyString
		{
			get { return " drop constraint "; }
		}

		public virtual string TableTypeString
		{
			get { return String.Empty; } // for differentiation of mysql storage engines
		}

		/// <summary>
		/// The keyword used to specify a nullable column
		/// </summary>
		public virtual string NullColumnString
		{
			get { return String.Empty; }
		}

		/// <summary>
		/// The keyword used to create a primary key constraint
		/// </summary>
		public virtual string PrimaryKeyString
		{
			get { return "primary key"; }
		}

		/// <summary>
		/// Supports splitting batches using GO T-SQL command
		/// </summary>
		/// <remarks>
		/// Batches http://msdn.microsoft.com/en-us/library/ms175502.aspx
		/// </remarks>
		public virtual bool SupportsSqlBatches
		{
			get { return false; }
		}

		public virtual bool IsKnownToken(string currentToken, string nextToken)
		{
			return false;
		}

		protected void RegisterKeyword(string word)
		{
			Keywords.Add(word);
		}

		protected void RegisterFunction(string name, ISQLFunction function)
		{
			_sqlFunctions[name] = function;
		}

		/// <summary> 
		/// Registers a NHibernate <see cref="IType"/> name for the given <see cref="DbType"/> type code. 
		/// </summary>
		/// <param name="code">The <see cref="DbType"/> typecode </param>
		/// <param name="name">The NHibernate <see cref="DbType"/> name </param>
		private void RegisterHibernateType(DbType code, string name)
		{
			_hibernateTypeNames.Put(code, name);
		}

		/// <summary> 
		/// Build an instance of the <see cref="ISQLExceptionConverter"/> preferred by this dialect for
		/// converting <see cref="System.Data.Common.DbException"/> into NHibernate's ADOException hierarchy.  
		/// </summary>
		/// <returns> The Dialect's preferred <see cref="ISQLExceptionConverter"/>. </returns>
		/// <remarks>
		/// The default Dialect implementation simply returns a converter based on X/Open SQLState codes.
		/// <para/>
		/// It is strongly recommended that specific Dialect implementations override this
		/// method, since interpretation of a SQL error is much more accurate when based on
		/// the ErrorCode rather than the SQLState. Unfortunately, the ErrorCode is a vendor-specific approach. 
		/// </remarks>
		public virtual ISQLExceptionConverter BuildSQLExceptionConverter()
		{
			// The default SQLExceptionConverter for all dialects is based on SQLState
			// since SQLErrorCode is extremely vendor-specific.  Specific Dialects
			// may override to return whatever is most appropriate for that vendor.
			return new SQLStateConverter(ViolatedConstraintNameExtracter);
		}
	}
}