using System;
using System.Data;
using System.Text;
using System.Collections;

using NHibernate.Util;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect {

	/// <summary>
	/// Represents a dialect of SQL implemented by a particular RDBMS. Sublcasses
	/// implement Hibernate compatibility with differen systems
	/// </summary>
	/// <remarks>
	/// Subclasses should provide a public default constructor that <c>Register()</c>
	/// a set of type mappings and default Hibernate properties.
	/// </remarks>
	public abstract class Dialect {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Dialect));

		const string DefaultBatchSize = "15";
		const string NoBatch = "0";

		protected Dialect() {
			log.Info( "Using dialect: " + this );
		}

		private TypeNames typeNames = new TypeNames("$1");
		private IDictionary properties = new Hashtable();
		
		/// <summary>
		/// Characters used for quoting sql identifiers
		/// </summary>
		public const string Quote = "'\"[";
		public const string ClosedQuote = "'\"]";

		/// <summary>
		/// Get the name of the database type associated with the given typecode
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <returns>The database type name</returns>
		public string GetTypeName(DbType code) {
			string result = typeNames.Get(code);
			if (result == null)
				throw new HibernateException("No default type mapping for " + code);
			return result;
		}

		/// <summary>
		/// Get the name of the database type associated with the given typecode
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="length">the length of the column</param>
		/// <returns>the database type name</returns>
		public string GetTypeName(DbType code, int length) {
			string result = typeNames.Get(code, length);
			if (result == null)
				throw new HibernateException("No type mapping for " + code + " of length " + length);
			return result;
		}

		/// <summary>
		/// Subclasses register a typename for the given type code and maximum
		/// column length. <c>$1</c> in the type name will be replaced by the column
		/// length (if appropriate
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="capacity">Maximum length of database type</param>
		/// <param name="name">The database type name</param>
		protected void Register(DbType code, int capacity, string name) {
			typeNames.Put(code, capacity, name);
		}

		/// <summary>
		/// Suclasses register a typename for the given type code. <c>$1</c> in the 
		/// typename will be replaced by the column length (if appropriate).
		/// </summary>
		/// <param name="code">The typecode</param>
		/// <param name="name">The database type name</param>
		protected void Register(DbType code, string name) {
			typeNames.Put(code, name);
		}

		/// <summary>
		/// Does this dialect support the <c>ALTER TABLE</c> syntax?
		/// </summary>
		public virtual bool HasAlterTable {
			get { return true; }
		}

		/// <summary>
		/// Do we need to drop constraints before dropping tables in the dialect?
		/// </summary>
		public virtual bool DropConstraints {
			get { return true; }
		}

		/// <summary>
		/// Do we need to qualify index names with the schema name?
		/// </summary>
		public virtual bool QualifyIndexName {
			get { return true; }
		}

		/// <summary>
		/// Does this dialect support the <c>FOR UDPATE</c> syntax?
		/// </summary>
		public virtual bool SupportsForUpdate {
			get { return true; }
		}

		/// <summary>
		/// Does this dialect support the Oracle-style <c>FOR UPDATE NOWAIT</c> syntax?
		/// </summary>
		public virtual bool SupportsForUpdateNoWait {
			get { return false; }
		}

		/// <summary>
		/// Does this dialect support the <c>UNIQUE</c> column syntax?
		/// </summary>
		public virtual bool SupportsUnique {
			get { return true; }
		}

		/// <summary>
		/// The syntax used to add a column to a table. Note this is deprecated
		/// </summary>
		public abstract string AddColumnString { get; }

		public virtual string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey) {
			return new StringBuilder(30)
				.Append(" add constraint ")
				.Append(constraintName)
				.Append(" foreign key (")
				.Append( string.Join(StringHelper.CommaSpace, foreignKey) )
				.Append(") references ")
				.Append(referencedTable)
				.ToString();
		}

		/// <summary>
		/// The syntax used to add a primary key constraint to a table
		/// </summary>
		/// <param name="constraintName"></param>
		public virtual string GetAddPrimaryKeyConstraintString(string constraintName) {
			return " add constraint " + constraintName + " primary key ";
		}

		/// <summary>
		/// The keyword used to specify a nullable column
		/// </summary>
		public virtual string NullColumnString {
			get { return String.Empty; }
		}

		/// <summary>
		/// Does this dialect support identity column key generation?
		/// </summary>
		public virtual bool SupportsIdentityColumns {
			get { return false; }
		}

		/// <summary>
		/// Does this dialect support sequences?
		/// </summary>
		public virtual bool SupportsSequences {
			get { return false; }
		}

		/// <summary>
		/// The syntax that returns the identity value of the last insert, if native
		/// key generation is supported
		/// </summary>
		public virtual string IdentitySelectString {
			get { throw new MappingException("Dialect does not support native key generation"); }
		}

		/// <summary>
		/// The keyword used to specify an identity column, if native key generation is supported
		/// </summary>
		public virtual string IdentityColumnString {
			get { throw new MappingException("Dialect does not support native key generation"); }
		}

		/// <summary>
		/// The keyword used to insert a generated value into an identity column (or null)
		/// </summary>
		public virtual string IdentityInsertString {
			get { return null; }
		}

		/// <summary>
		/// The keyword used to insert a row without specifying any column values
		/// </summary>
		public virtual string NoColumnsInsertString {
			get { return "values ( )"; }
		}

		/// <summary>
		/// The syntax that fetches the next value of a sequence, if sequences are supported.
		/// </summary>
		/// <param name="sequenceName">The name of the sequence</param>
		/// <returns></returns>
		public virtual string GetSequenceNextValString(string sequenceName) {
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary>
		/// The syntax used to create a sequence, if sequences are supported
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public virtual string GetCreateSequenceString(string sequenceName) {
			throw new MappingException("Dialect does not support sequences");
		}

		/// <summary>
		/// The syntax used to drop a sequence, if sequences are supported
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public virtual string GetDropSequenceString(string sequenceName) {
			throw new MappingException("Dialect does not support sequences");
		}

		public virtual string QuerySequenceString {
			get { return null; }
		}

		public static Dialect GetDialect() {
			string dialectName = Cfg.Environment.Properties[Cfg.Environment.Dialect] as string;
            if (dialectName==null) throw new HibernateException("The dialect was not set. Set the property hibernate.dialect.");
			try {
				return (Dialect) Activator.CreateInstance(ReflectHelper.ClassForName(dialectName));
			} catch (Exception e) {
				throw new HibernateException("Could not instanciate dialect class", e);
			}
		}

		public static Dialect GetDialect(IDictionary props) {
			if (props==null) return GetDialect();
			string dialectName = (string) props[Cfg.Environment.Dialect];
			if (dialectName==null) return GetDialect();
			try {
				return (Dialect) Activator.CreateInstance(ReflectHelper.ClassForName(dialectName));
			} catch (Exception e) {
				throw new HibernateException("could not instantiate dialect class", e);
			}
		}

		/// <summary>
		/// Retrieve a set of default Hibernate properties for this database.
		/// </summary>
		public IDictionary DefaultProperties {
			get { return properties; }
		}

		/// <summary>
		/// Completely optional cascading drop clause
		/// </summary>
		public virtual string CascadeConstraintsString {
			get { return String.Empty; }
		}

		/// <summary>
		/// Create an <c>JoinFragment</c> for this dialect
		/// </summary>
		/// <returns></returns>
		public virtual JoinFragment CreateOuterJoinFragment() {
			return new ANSIJoinFragment();
		}

		/// <summary>
		/// Create an <c>CaseFragment</c> for this dialect
		/// </summary>
		/// <returns></returns>
		public virtual SqlCommand.CaseFragment CreateCaseFragment() {
			return new SqlCommand.ANSICaseFragment();
		}

		/// <summary>
		/// The name of the SQL function that transforms a string to lowercase
		/// </summary>
		public virtual string LowercaseFunction {
			get {
				return "lower";
			}
		}

		/// <summary>
		/// Does this dialect use named parameters?
		/// </summary>
		/// <remarks>
		/// Do NOT use this Property. It will be removed once the class ADOHack has been removed.  Instead
		/// the Driver should be used because that is what determines how the CommandText and Parameters
		/// have to be built.
		/// </remarks>
		[Obsolete("This method will be removed once the class ADOHack is gone.  The Driver should be used instead.")]
		public virtual bool UseNamedParameters {
			get {
				return false;
			}
		}

		/// <summary>
		/// The prefix to use with named parameter.
		/// If UseNamedParameters return false this property will not be used.
		/// </summary>
		/// <remarks>
		/// Do NOT use this Property. It will be removed once the class ADOHack has been removed.  Instead
		/// the Driver should be used because that is what determines how the CommandText and Parameters
		/// have to be built.
		/// </remarks>
		[Obsolete("This method will be removed once ADOHack is gone.  The Driver should be used instead.")]
		public virtual string NamedParametersPrefix {
			get {
				return String.Empty;
			}
		}

		/// <summary>
		/// Converts the SqlType to the Dialect specific column type string used when
		/// <c>CREATE</c>ing the table.
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string</param>
		/// <returns>A string that can be used in the table CREATE statement.</returns>
		/// <remarks>
		/// <para>
		/// This method uses SqlType.DbType to call the appropriate protected method of SqlTypeToString().  
		/// All Dialects should override the SqlTypeToString methods because that is where the underlying
		/// SqlType is converted to a string.
		/// </para>
		/// </remarks>
		public string SqlTypeToString(SqlType sqlType) 
		{
			switch(sqlType.DbType) 
			{
				case DbType.AnsiStringFixedLength: 
					return SqlTypeToString((AnsiStringFixedLengthSqlType)sqlType);
				case DbType.Binary :
					return SqlTypeToString((BinarySqlType)sqlType);
				case DbType.Boolean :
					return SqlTypeToString((BooleanSqlType)sqlType);
				case DbType.Byte:
					return SqlTypeToString((ByteSqlType)sqlType);
				case DbType.Currency:
					return SqlTypeToString((CurrencySqlType)sqlType);
				case DbType.Date:
					return SqlTypeToString((DateSqlType)sqlType);
				case DbType.DateTime:
					return SqlTypeToString((DateTimeSqlType)sqlType);
				case DbType.Decimal:
					return SqlTypeToString((DecimalSqlType)sqlType);
				case DbType.Double:
					return SqlTypeToString((DoubleSqlType)sqlType);
				case DbType.Int16:
					return SqlTypeToString((Int16SqlType)sqlType);
				case DbType.Int32:
					return SqlTypeToString((Int32SqlType)sqlType);
				case DbType.Int64:
					return SqlTypeToString((Int64SqlType)sqlType);
				case DbType.Single:
					return SqlTypeToString((SingleSqlType)sqlType);
				case DbType.StringFixedLength:
					return SqlTypeToString((StringFixedLengthSqlType)sqlType);
				case DbType.String:
					return SqlTypeToString((StringSqlType)sqlType);
				default:
					throw new ApplicationException("Unmapped DBType");
					//break;
			}

		}


		/// <summary>
		/// Converts an AnsiStringSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an BinarySqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(BinarySqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an BooleanSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(BooleanSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an ByteSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(ByteSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an CurrencySqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(CurrencySqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an DateSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(DateSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an DateTimeSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(DateTimeSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an DecimalSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(DecimalSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an DoubleSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(DoubleSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an Int16SqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(Int16SqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an Int32SqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(Int32SqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an Int64SqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(Int64SqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an SingleSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(SingleSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		/// <summary>
		/// Converts an StringFixedLengthSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(StringFixedLengthSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}
		
		
		/// <summary>
		/// Converts an StringSqlType to the Database specific column type. 
		/// </summary>
		/// <param name="sqlType">The SqlType to convert to a string.</param>
		/// <returns>A string that can be used for the column type when creating the table.</returns>
		protected virtual string SqlTypeToString(StringSqlType sqlType)
		{
			throw new NotImplementedException("should be implemented by subclass - this will be converted to abstract");
		}

	}
}
