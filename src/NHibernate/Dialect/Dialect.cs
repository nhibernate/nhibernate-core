using System;
using System.Data;
using System.Text;
using System.Collections;

using NHibernate.Util;
using NHibernate.Sql;

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
		public const string Quote = "'\"";

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
			get { return StringHelper.EmptyString; }
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
			string dialectName = null;
			
			//TODO: Get the class name from the environment
			dialectName = "NHibernate.Dialect.GenericDialect";

			if (dialectName==null) throw new HibernateException("The dialect was not set.");
			try {
				return (Dialect) Activator.CreateInstance(System.Type.GetType(dialectName));
			} catch (Exception e) {
				throw new HibernateException("Could not instanciate dialect class", e);
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
			get { return StringHelper.EmptyString; }
		}

		/// <summary>
		/// Create an <c>JoinFragment</c> for this dialect
		/// </summary>
		/// <returns></returns>
		public JoinFragment CreateOuterJoinFragment() {
			return new ANSIJoinFragment();
		}

		/// <summary>
		/// Create an <c>CaseFragment</c> for this dialect
		/// </summary>
		/// <returns></returns>
		public CaseFragment CreateCaseFragment() {
			return new ANSICaseFragment();
		}
	}
}
