using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents a Table in a database that an object gets mapped against.
	/// </summary>
	[Serializable]
	public class Table : IRelationalModel
	{
		internal class ForeignKeyKey: IEqualityComparer<ForeignKeyKey>
		{
			internal string referencedClassName;
			internal List<Column> columns;
			internal List<Column> referencedColumns;

			internal ForeignKeyKey(IEnumerable<Column> columns, string referencedClassName, IEnumerable<Column> referencedColumns)
			{
				this.referencedClassName = referencedClassName;
				this.columns = new List<Column>(columns);
				if (referencedColumns != null)
					this.referencedColumns = new List<Column>(referencedColumns);
				else
					this.referencedColumns = new List<Column>();
			}

			public override int GetHashCode()
			{
				return GetHashCode(this);
			}

			public override bool Equals(object other)
			{
				ForeignKeyKey that = other as ForeignKeyKey;
				if (that != null)
					return Equals(this, that);
				else
					return false;
			}

			#region IEqualityComparer<ForeignKeyKey> Members

			public bool Equals(ForeignKeyKey x, ForeignKeyKey y)
			{
				// NH : Different implementation to prevent NH930 (look test)
				return //y.referencedClassName.Equals(x.referencedClassName) &&
					CollectionHelper.CollectionEquals<Column>(y.columns, x.columns) &&
					CollectionHelper.CollectionEquals<Column>(y.referencedColumns, x.referencedColumns);
			}

			public int GetHashCode(ForeignKeyKey obj)
			{
				int result = CollectionHelper.GetHashCode(obj.columns) ^ CollectionHelper.GetHashCode(obj.referencedColumns);
				return result;
			}

			#endregion
		}

		private string name;
		private string schema;
		private string catalog;

		private readonly LinkedHashMap<string, Column> columns = new LinkedHashMap<string, Column>();
		private IKeyValue idValue;
		private PrimaryKey primaryKey;
		private readonly Dictionary<string, Index> indexes = new Dictionary<string, Index>();
		private readonly Dictionary<ForeignKeyKey, ForeignKey> foreignKeys = new Dictionary<ForeignKeyKey, ForeignKey>();
		private readonly Dictionary<string, UniqueKey> uniqueKeys = new Dictionary<string, UniqueKey>();
		private readonly int uniqueInteger;
		private bool quoted;
		private static int tableCounter = 0;
		private readonly List<string> checkConstraints = new List<string>();
		private bool isAbstract;
		private bool hasDenormalizedTables = false;
		private string comment;
		private string subselect;
		private string rowId;
		private bool isSchemaQuoted;

		/// <summary>
		/// Initializes a new instance of <see cref="Table"/>.
		/// </summary>
		public Table()
		{
			uniqueInteger = tableCounter++;
		}

		public Table(string name)
			: this()
		{
			Name = name;
		}

		/// <summary>
		/// Gets the schema qualified name of the Table.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> that knows how to Quote the Table name.</param>
		/// <returns>The name of the table qualified with the schema if one is specified.</returns>
		public string GetQualifiedName(Dialect.Dialect dialect)
		{
			return GetQualifiedName(dialect, null, null);
		}

		/// <summary>
		/// Gets the schema qualified name of the Table using the specified qualifier
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> that knows how to Quote the Table name.</param>
		/// <param name="defaultQualifier">The Qualifier to use when accessing the table.</param>
		/// <returns>A String representing the Qualified name.</returns>
		/// <remarks>If this were used with MSSQL it would return a dbo.table_name.</remarks>
		public string GetQualifiedName(Dialect.Dialect dialect, string defaultQualifier)
		{
			return GetQualifiedName(dialect, null, defaultQualifier);
		}

		public virtual string GetQualifiedName(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			if (!string.IsNullOrEmpty(subselect))
			{
				return "( " + subselect + " )";
			}
			string quotedName = GetQuotedName(dialect);
			string usedSchema = schema == null ? defaultSchema : GetQuotedSchema(dialect);
			string usedCatalog = catalog ?? defaultCatalog;
			return Qualify(usedCatalog, usedSchema, quotedName);
		}

		/// <summary>
		/// Gets or sets the name of the Table in the database.
		/// </summary>
		/// <value>
		/// The name of the Table in the database.  The get does 
		/// not return a Quoted Table name.
		/// </value>
		/// <remarks>
		/// <p>
		/// If a value is passed in that is wrapped by <c>`</c> then 
		/// NHibernate will Quote the Table whenever SQL is generated
		/// for it.  How the Table is quoted depends on the Dialect.
		/// </p>
		/// <p>
		/// The value returned by the getter is not Quoted.  To get the
		/// column name in quoted form use <see cref="GetQuotedName(Dialect.Dialect)"/>.
		/// </p>
		/// </remarks>
		public string Name
		{
			get { return name; }
			set
			{
				if (value[0] == '`')
				{
					quoted = true;
					name = value.Substring(1, value.Length - 2);
				}
				else
				{
					name = value;
				}
			}
		}

		/// <summary> returns quoted name as it would be in the mapping file.</summary>
		public string GetQuotedName()
		{
			return quoted ? "`" + name + "`" : name;
		}

		/// <summary>
		/// Gets the name of this Table in quoted form if it is necessary.
		/// </summary>
		/// <param name="dialect">
		/// The <see cref="Dialect.Dialect"/> that knows how to quote the Table name.
		/// </param>
		/// <returns>
		/// The Table name in a form that is safe to use inside of a SQL statement.
		/// Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		public string GetQuotedName(Dialect.Dialect dialect)
		{
			return IsQuoted ?
			       dialect.QuoteForTableName(name) :
			       name;
		}

		/// <summary> returns quoted name as it is in the mapping file.</summary>
		public string GetQuotedSchema()
		{
			return IsSchemaQuoted ? "`" + schema + "`" : schema;
		}
		
		public string GetQuotedSchema(Dialect.Dialect dialect)
		{
			return IsSchemaQuoted ? dialect.OpenQuote + schema + dialect.CloseQuote : schema;
		}

		/// <summary>
		/// Gets the schema for this table in quoted form if it is necessary.
		/// </summary>
		/// <param name="dialect">
		/// The <see cref="Dialect.Dialect" /> that knows how to quote the table name.
		/// </param>
		/// <returns>
		/// The schema name for this table in a form that is safe to use inside
		/// of a SQL statement. Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		public string GetQuotedSchemaName(Dialect.Dialect dialect)
		{
			if (schema == null)
			{
				return null;
			}

			if (schema.StartsWith("`"))
			{
				return dialect.QuoteForSchemaName(schema.Substring(1, schema.Length - 2));
			}

			return schema;
		}

		/// <summary>
		/// Gets the <see cref="Column"/> at the specified index.
		/// </summary>
		/// <param name="n">The index of the Column to get.</param>
		/// <returns> 
		/// The <see cref="Column"/> at the specified index.
		/// </returns>
		public Column GetColumn(int n)
		{
			IEnumerator<Column> iter = columns.Values.GetEnumerator();
			for (int i = 0; i < n; i++)
			{
				iter.MoveNext();
			}
			return iter.Current;
		}

		/// <summary>
		/// Adds the <see cref="Column"/> to the <see cref="ICollection"/> of 
		/// Columns that are part of the Table.
		/// </summary>
		/// <param name="column">The <see cref="Column"/> to include in the Table.</param>
		public void AddColumn(Column column)
		{
			Column old = GetColumn(column);
			if (old == null)
			{
				columns[column.CanonicalName] = column;
				column.uniqueInteger = columns.Count;
			}
			else
			{
				column.uniqueInteger = old.uniqueInteger;
			}
		}

		/// <summary>
		/// Gets the number of columns that this Table contains.
		/// </summary>
		/// <value>
		/// The number of columns that this Table contains.
		/// </value>
		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable"/> of <see cref="Column"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="IEnumerable"/> of <see cref="Column"/> objects that are 
		/// part of the Table.
		/// </value>
		public virtual IEnumerable<Column> ColumnIterator
		{
			get { return columns.Values; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Index"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Index"/> objects that are 
		/// part of the Table.
		/// </value>
		public virtual IEnumerable<Index> IndexIterator
		{
			get { return indexes.Values; }
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable"/> of <see cref="ForeignKey"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="IEnumerable"/> of <see cref="ForeignKey"/> objects that are 
		/// part of the Table.
		/// </value>
		public IEnumerable<ForeignKey> ForeignKeyIterator
		{
			get { return foreignKeys.Values; }
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable"/> of <see cref="UniqueKey"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="IEnumerable"/> of <see cref="UniqueKey"/> objects that are 
		/// part of the Table.
		/// </value>
		public virtual IEnumerable<UniqueKey> UniqueKeyIterator
		{
			get { return uniqueKeys.Values; }
		}

		public string[] SqlAlterStrings(Dialect.Dialect dialect, IMapping p, TableMetadata tableInfo, string defaultSchema)
		{
			StringBuilder root = new StringBuilder("alter table ")
				.Append(GetQualifiedName(dialect, defaultSchema))
				.Append(" ")
				.Append(dialect.AddColumnString);

			ArrayList results = new ArrayList(ColumnSpan);

			foreach (Column col in ColumnIterator)
			{
				if (tableInfo.GetColumnMetadata(col.Name) != null)
					continue;

				StringBuilder alter = new StringBuilder(root.ToString())
					.Append(" ")
					.Append(col.GetQuotedName(dialect))
					.Append(" ")
					.Append(col.GetSqlType(dialect, p));
				if (string.IsNullOrEmpty(col.DefaultValue) == false)
				{
					alter.Append(" default ").Append(col.DefaultValue).Append(" ");
					if (col.IsNullable)
						alter.Append(dialect.NullColumnString);
					else
						alter.Append(" not null ");
				}
				if (col.IsUnique && dialect.SupportsUnique)
				{
					alter.Append(" unique");
				}

				if (col.HasCheckConstraint && dialect.SupportsColumnCheck)
				{
					alter.Append(" check(")
						.Append(col.CheckConstraint)
						.Append(") ");
				}

				results.Add(alter.ToString());
			}

			return (string[]) results.ToArray(typeof (string));
		}

		/// <summary>
		/// Generates the SQL string to create this Table in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create this Table, Primary Key Constraints
		/// , and Unique Key Constraints.
		/// </returns>
		public string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			StringBuilder buf = new StringBuilder(HasPrimaryKey
			                                      	?
			                                      		dialect.CreateTableString
			                                      	: dialect.CreateMultisetTableString)
				.Append(' ')
				.Append(GetQualifiedName(dialect, defaultSchema))
				.Append(" (");

			bool identityColumn = idValue != null && idValue.IsIdentityColumn(dialect);

			// try to find out the name of the pk to create it as identity if the 
			// identitygenerator is used
			string pkname = null;
			if (HasPrimaryKey && identityColumn)
			{
				foreach (Column col in PrimaryKey.ColumnIterator)
				{
					pkname = col.GetQuotedName(dialect); //should only go through this loop once
				}
			}

			bool commaNeeded = false;
			foreach (Column col in ColumnIterator)
			{
				if (commaNeeded)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				commaNeeded = true;

				buf.Append(col.GetQuotedName(dialect))
					.Append(' ');

				if (identityColumn && col.GetQuotedName(dialect).Equals(pkname))
				{
					// to support dialects that have their own identity data type
					if (dialect.HasDataTypeInIdentityColumn)
					{
						buf.Append(col.GetSqlType(dialect, p));
					}
					buf.Append(' ')
						.Append(dialect.IdentityColumnString);
				}
				else
				{
					buf.Append(col.GetSqlType(dialect, p));

					if(string.IsNullOrEmpty(col.DefaultValue)==false)
					{
						buf.Append(" default ").Append(col.DefaultValue).Append(" ");
					}

					if (col.IsNullable)
					{
						buf.Append(dialect.NullColumnString);
					}
					else
					{
						buf.Append(" not null");
					}
				}

				if (col.IsUnique)
				{
					if (dialect.SupportsUnique)
					{
						buf.Append(" unique");
					}
					else
					{
						UniqueKey uk = GetUniqueKey(col.GetQuotedName(dialect) + "_");
						uk.AddColumn(col);
					}
				}

				if(col.HasCheckConstraint && dialect.SupportsColumnCheck)
				{
					buf.Append(" check( ")
						.Append(col.CheckConstraint)
						.Append(") ");
				}

				if(string.IsNullOrEmpty(col.Comment)==false)
				{
					buf.Append(dialect.getColumnComment(col.Comment));
				}
			}
			if (HasPrimaryKey)
			{
				buf.Append(StringHelper.CommaSpace).Append(PrimaryKey.SqlConstraintString(dialect, defaultSchema));
			}

			foreach (UniqueKey uk in UniqueKeyIterator)
			{
				buf.Append(',').Append(uk.SqlConstraintString(dialect));
			}
			
			if(dialect.SupportsTableCheck)
			{
				foreach (string checkConstraint in checkConstraints)
				{
					buf.Append(", check (")
						.Append(checkConstraint)
						.Append(") ");
				}
			}

			buf.Append(StringHelper.ClosedParen);

			if(string.IsNullOrEmpty(comment)==false)
			{
				buf.Append(dialect.GetTableComment(comment));
			}

			return buf.ToString();
		}

		/// <summary>
		/// Generates the SQL string to drop this Table in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Table and to cascade the drop to 
		/// the constraints if the database supports it.
		/// </returns>
		public string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			return dialect.GetDropTableString(GetQualifiedName(dialect, defaultCatalog, defaultSchema));
		}

		/// <summary>
		/// Gets or sets the <see cref="PrimaryKey"/> of the Table.
		/// </summary>
		/// <value>The <see cref="PrimaryKey"/> of the Table.</value>
		public virtual PrimaryKey PrimaryKey
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		/// <summary>
		/// Gets the <see cref="Index"/> identified by the name.
		/// </summary>
		/// <param name="name">The name of the <see cref="Index"/> to get.</param>
		/// <returns>
		/// The <see cref="Index"/> identified by the name.  If the <see cref="Index"/>
		/// identified by the name does not exist then it is created.
		/// </returns>
		public Index GetIndex(string name)
		{
			Index result;
			indexes.TryGetValue(name, out result);
			return result;
		}

		public Index AddIndex(Index index)
		{
			Index current = GetIndex(index.Name);
			if (current != null)
			{
				throw new MappingException("Index " + index.Name + " already exists!");
			}
			indexes[index.Name] = index;
			return index;
		}

		public Index GetOrCreateIndex(string indexName)
		{
			Index index = GetIndex(indexName);
			if (index == null)
			{
				index = new Index();
				index.Name = indexName;
				index.Table = this;
				indexes[indexName] = index;
			}
			return index;
		}

		/// <summary>
		/// Gets the <see cref="UniqueKey"/> identified by the name.
		/// </summary>
		/// <param name="name">The name of the <see cref="UniqueKey"/> to get.</param>
		/// <returns>
		/// The <see cref="UniqueKey"/> identified by the name.  If the <see cref="UniqueKey"/>
		/// identified by the name does not exist then it is created.
		/// </returns>
		public UniqueKey GetUniqueKey(string name)
		{
			UniqueKey result;
			uniqueKeys.TryGetValue(name, out result);
			return result;
		}

		public UniqueKey AddUniqueKey(UniqueKey uniqueKey)
		{
			UniqueKey current = GetUniqueKey(uniqueKey.Name);
			if (current != null)
			{
				throw new MappingException("UniqueKey " + uniqueKey.Name + " already exists!");
			}
			uniqueKeys[uniqueKey.Name] = uniqueKey;
			return uniqueKey;
		}

		public UniqueKey GetOrCreateUniqueKey(string keyName)
		{
			UniqueKey uk = GetUniqueKey(keyName);

			if (uk == null)
			{
				uk = new UniqueKey();
				uk.Name = keyName;
				uk.Table = this;
				uniqueKeys[keyName] = uk;
			}
			return uk;
		}

		public virtual void CreateForeignKeys()
		{
		}

		public virtual ForeignKey CreateForeignKey(string keyName, IEnumerable<Column> keyColumns, string referencedEntityName)
		{
			return CreateForeignKey(keyName, keyColumns, referencedEntityName, null);
		}

		/// <summary>
		/// Create a <see cref="ForeignKey"/> for the columns in the Table.
		/// </summary>
		/// <param name="keyName"></param>
		/// <param name="keyColumns">An <see cref="IList"/> of <see cref="Column"/> objects.</param>
		/// <param name="referencedEntityName"></param>
		/// <param name="referencedColumns"></param>
		/// <returns>
		/// A <see cref="ForeignKey"/> for the columns in the Table.  
		/// </returns>
		/// <remarks>
		/// This does not necessarily create a <see cref="ForeignKey"/>, if
		/// one already exists for the columns then it will return an 
		/// existing <see cref="ForeignKey"/>.
		/// </remarks>
		public virtual ForeignKey CreateForeignKey(string keyName, IEnumerable<Column> keyColumns,
			string referencedEntityName, IEnumerable<Column> referencedColumns)
		{
			IEnumerable<Column> kCols = keyColumns;
			IEnumerable<Column> refCols = referencedColumns;

			ForeignKeyKey key = new ForeignKeyKey(kCols, referencedEntityName, refCols);

			ForeignKey fk;
			foreignKeys.TryGetValue(key, out fk);

			if (fk == null)
			{
				fk = new ForeignKey();
				if (!string.IsNullOrEmpty(keyName))
				{
					fk.Name = keyName;
				}
				else
				{
					fk.Name = "FK" + UniqueColumnString(kCols, referencedEntityName);
					//TODO: add referencedClass to disambiguate to FKs on the same columns, pointing to different tables
				}
				fk.Table = this;
				foreignKeys.Add(key, fk);
				fk.ReferencedEntityName = referencedEntityName;
				fk.AddColumns(kCols);
				if (referencedColumns != null)
				{
					fk.AddReferencedColumns(refCols);
				}
			}

			if (!string.IsNullOrEmpty(keyName))
			{
				fk.Name = keyName;
			}

			return fk;
		}

		public virtual UniqueKey CreateUniqueKey(IList<Column> keyColumns)
		{
			string keyName = "UK" + UniqueColumnString(keyColumns);
			UniqueKey uk = GetOrCreateUniqueKey(keyName);
			uk.AddColumns(keyColumns);
			return uk;
		}

		/// <summary>
		/// Generates a unique string for an <see cref="ICollection"/> of 
		/// <see cref="Column"/> objects.
		/// </summary>
		/// <param name="columns">An <see cref="ICollection"/> of <see cref="Column"/> objects.</param>
		/// <returns>
		/// An unique string for the <see cref="Column"/> objects.
		/// </returns>
		public string UniqueColumnString(IEnumerable columns)
		{
			return UniqueColumnString(columns, null);
		}

		public string UniqueColumnString(IEnumerable iterator, string referencedEntityName)
		{
			int result = 0;
			if (referencedEntityName != null)
				result += referencedEntityName.GetHashCode();

			foreach (object o in iterator)
			{
				// this is marked as unchecked because the GetHashCode could potentially
				// cause an integer overflow.  This way if there is an overflow it will
				// just roll back over - since we are not doing any computations based
				// on this number then a rollover is no big deal.
				unchecked
				{
					result += o.GetHashCode();
				}
			}
			return (name.GetHashCode().ToString("X") + result.GetHashCode().ToString("X"));
		}

		/// <summary>
		/// Gets or sets the schema the table is in.
		/// </summary>
		/// <value>
		/// The schema the table is in or <see langword="null" /> if no schema is specified.
		/// </value>
		public string Schema
		{
			get{return schema;}
			set
			{
				if (value != null && value[0] == '`')
				{
					isSchemaQuoted = true;
					schema = value.Substring(1, value.Length - 2);
				}
				else
				{
					schema = value;
				}
			}
		}

		/// <summary>
		/// Gets the unique number of the Table.
		/// </summary>
		/// <value>The unique number of the Table.</value>
		public int UniqueInteger
		{
			get { return uniqueInteger; }
		}

		/// <summary>
		/// Sets the Identifier of the Table.
		/// </summary>
		/// <param name="idValue">The <see cref="SimpleValue"/> that represents the Identifier.</param>
		public void SetIdentifierValue(SimpleValue idValue)
		{
			this.idValue = idValue;
		}

		/// <summary>
		/// Gets or sets if the column needs to be quoted in SQL statements.
		/// </summary>
		/// <value><see langword="true" /> if the column is quoted.</value>
		public bool IsQuoted
		{
			get { return quoted; }
			set { quoted = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="constraint"></param>
		public void AddCheckConstraint(string constraint)
		{
			checkConstraints.Add(constraint);
		}

		public IEnumerable<string> CheckConstraintsIterator
		{
			get { return checkConstraints; }
		}

		public bool IsAbstractUnionTable
		{
			get { return HasDenormalizedTables && isAbstract; }
		}

		public bool HasDenormalizedTables
		{
			get { return hasDenormalizedTables; }
		}

		internal void SetHasDenormalizedTables()
		{
			hasDenormalizedTables = true;
		}

		public bool IsAbstract
		{
			get { return isAbstract; }
			set { isAbstract = value; }
		}

		public virtual bool ContainsColumn(Column column)
		{
			return columns.ContainsValue(column);
		}

		/// <summary> Return the column which is identified by column provided as argument. </summary>
		/// <param name="column">column with atleast a name. </param>
		/// <returns> 
		/// The underlying column or null if not inside this table.
		/// Note: the instance *can* be different than the input parameter, but the name will be the same.
		/// </returns>
		public virtual Column GetColumn(Column column)
		{
			if (column == null)
				return null;

			Column result;
			columns.TryGetValue(column.CanonicalName, out result);

			return column.Equals(result) ? result : null;
		}

		internal IDictionary<string, UniqueKey> UniqueKeys
		{
			get
			{
				if (uniqueKeys.Count > 1)
				{
					//deduplicate unique constraints sharing the same columns
					//this is needed by Hibernate Annotations since it creates automagically
					// unique constraints for the user
					Dictionary<string, UniqueKey> finalUniqueKeys = new Dictionary<string, UniqueKey>(uniqueKeys.Count);
					foreach (KeyValuePair<string, UniqueKey> entry in uniqueKeys)
					{
						UniqueKey uk = entry.Value;
						IList<Column> _columns = uk.Columns;
						bool skip = false;
						Dictionary<string, UniqueKey> tempUks = new Dictionary<string, UniqueKey>(finalUniqueKeys);
						foreach (KeyValuePair<string, UniqueKey> tUk in tempUks)
						{
							UniqueKey currentUk = tUk.Value;
							if (AreSameColumns(currentUk.Columns, _columns))
							{
								skip = true;
								break;
							}
						}
						if (!skip)
							finalUniqueKeys[entry.Key] = uk;
					}
					return finalUniqueKeys;
				}
				else
				{
					return uniqueKeys;
				}
			}
		}

		private static bool AreSameColumns(ICollection<Column> col1, ICollection<Column> col2)
		{
			if(col1.Count!=col2.Count)
				return false;
			bool result= true;
			foreach (Column column in col1)
			{
				if(!col2.Contains(column))
					return false;
			}
			foreach (Column column in col2)
			{
				if (!col1.Contains(column))
					return false;
			}
			return result;
		}

		public static string Qualify(string catalog, string schema, string table)
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

		public bool HasPrimaryKey
		{
			get { return PrimaryKey != null; }
		}

		public string Catalog
		{
			get { return catalog; }
			set { catalog = value; }
		}

		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		public string Subselect
		{
			get { return subselect; }
			set { subselect = value; }
		}

		public IKeyValue IdentifierValue
		{
			get { return idValue; }
			set { idValue = value; }
		}

		public bool IsSubselect
		{
			get { return !string.IsNullOrEmpty(subselect); }
		}

		public bool IsPhysicalTable
		{
			get { return !IsSubselect && !IsAbstractUnionTable; }
		}

		public string RowId
		{
			get { return rowId; }
			set { rowId = value; }
		}

		public bool IsSchemaQuoted
		{
			get { return isSchemaQuoted; }
		}

		public virtual string[] SqlCommentStrings(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			List<string> comments = new List<string>();
			if (dialect.SupportsCommentOn)
			{
				string tableName = GetQualifiedName(dialect, defaultCatalog, defaultSchema);
				if (!string.IsNullOrEmpty(comment))
				{
					StringBuilder buf = new StringBuilder()
						.Append("comment on table ")
						.Append(tableName)
						.Append(" is '")
						.Append(comment)
						.Append("'");
					comments.Add(buf.ToString());
				}
				foreach (Column column in ColumnIterator)
				{
					string columnComment = column.Comment;
					if (columnComment != null)
					{
						StringBuilder buf = new StringBuilder()
							.Append("comment on column ")
							.Append(tableName)
							.Append('.')
							.Append(column.GetQuotedName(dialect))
							.Append(" is '")
							.Append(columnComment).Append("'");
						comments.Add(buf.ToString());
					}
				}
			}
			return comments.ToArray();
		}

		public virtual string SqlTemporaryTableCreateString(Dialect.Dialect dialect, IMapping mapping)
		{
			StringBuilder buffer = new StringBuilder(dialect.CreateTemporaryTableString)
				.Append(' ')
				.Append(name)
				.Append(" (");
			bool commaNeeded = false;
			foreach (Column column in ColumnIterator)
			{
				buffer.Append(column.GetQuotedName(dialect))
					.Append(' ');
				buffer.Append(column.GetSqlType(dialect, mapping));

				if (commaNeeded)
					buffer.Append(StringHelper.CommaSpace);
				commaNeeded = true;

				if (column.IsNullable)
				{
					buffer.Append(dialect.NullColumnString);
				}
				else
				{
					buffer.Append(" not null");
				}
			}

			buffer.Append(") ");
			buffer.Append(dialect.CreateTemporaryTablePostfix);
			return buffer.ToString();
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder()
				.Append(GetType().FullName)
				.Append('(');
			if (Catalog != null)
			{
				buf.Append(Catalog + ".");
			}
			if (Schema != null)
			{
				buf.Append(Schema + ".");
			}
			buf.Append(Name).Append(')');
			return buf.ToString();
		}

		public void ValidateColumns(Dialect.Dialect dialect, IMapping mapping, DataTable tableInfo)
		{
			throw new NotSupportedException();
		}
	}
}