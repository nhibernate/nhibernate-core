using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Cfg
{
	/// <summary>
	/// A collection of mappings from classes and collections to relational database tables.
	/// </summary>
	/// <remarks>Represents a single <c>&lt;hibernate-mapping&gt;</c> element.</remarks>
	[Serializable]
	public class Mappings
	{
		#region Utility classes

		[Serializable]
		public class ColumnNames
		{
			public readonly IDictionary<string, string> logicalToPhysical = new Dictionary<string, string>();
			public readonly IDictionary<string, string> physicalToLogical = new Dictionary<string, string>();
		}

		[Serializable]
		public class TableDescription
		{
			public readonly string logicalName;
			public readonly Table denormalizedSupertable;

			public TableDescription(string logicalName, Table denormalizedSupertable)
			{
				this.logicalName = logicalName;
				this.denormalizedSupertable = denormalizedSupertable;
			}
		}

		[Serializable]
		public sealed class PropertyReference
		{
			public string referencedClass;
			public string propertyName;
			public bool unique;
		}

		#endregion

		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(Mappings));

		private readonly IDictionary<string, PersistentClass> classes;
		private readonly IDictionary<string, Mapping.Collection> collections;
		private readonly IDictionary<string, Table> tables;
		private readonly IDictionary<string, NamedQueryDefinition> queries;
		private readonly IDictionary<string, NamedSQLQueryDefinition> sqlqueries;
		private readonly IDictionary<string, ResultSetMappingDefinition> resultSetMappings;
		private readonly IList<SecondPassCommand> secondPasses;
		private readonly IDictionary<string, string> imports;
		private string schemaName;
		private string catalogName;
		private string defaultCascade;
		private string defaultNamespace;
		private readonly Dialect.Dialect dialect;
		private string defaultAssembly;
		private string defaultAccess;
		private bool autoImport;
		private bool defaultLazy;
		private readonly IList<PropertyReference> propertyReferences;
		private readonly IDictionary<string, FilterDefinition> filterDefinitions;
		private readonly IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects;
		private readonly Queue<FilterSecondPassArgs> filtersSecondPasses;

		private readonly INamingStrategy namingStrategy;

		protected internal IDictionary<string, TypeDef> typeDefs;
		protected internal ISet<ExtendsQueueEntry> extendsQueue;

		/// <summary> 
		/// Binding table between the logical column name and the name out of the naming strategy
		/// for each table.
		/// According that when the column name is not set, the property name is considered as such
		/// This means that while theoretically possible through the naming strategy contract, it is
		/// forbidden to have 2 real columns having the same logical name
		/// </summary>
		protected internal IDictionary<Table, ColumnNames> columnNameBindingPerTable;

		/// <summary> 
		/// Binding between logical table name and physical one (ie after the naming strategy has been applied)
		/// </summary>
		protected internal IDictionary<string, TableDescription> tableNameBinding;

		protected internal Mappings(
			IDictionary<string, PersistentClass> classes,
			IDictionary<string, Mapping.Collection> collections,
			IDictionary<string, Table> tables,
			IDictionary<string, NamedQueryDefinition> queries,
			IDictionary<string, NamedSQLQueryDefinition> sqlqueries,
			IDictionary<string, ResultSetMappingDefinition> resultSetMappings,
			IDictionary<string, string> imports,
			IList<SecondPassCommand> secondPasses,
			Queue<FilterSecondPassArgs> filtersSecondPasses,
			IList<PropertyReference> propertyReferences,
			INamingStrategy namingStrategy,
			IDictionary<string, TypeDef> typeDefs,
			IDictionary<string, FilterDefinition> filterDefinitions,
			ISet<ExtendsQueueEntry> extendsQueue,
			IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects,
			IDictionary<string, TableDescription> tableNameBinding,
			IDictionary<Table, ColumnNames> columnNameBindingPerTable,
			string defaultAssembly,
			string defaultNamespace,
			Dialect.Dialect dialect)
		{
			this.classes = classes;
			this.collections = collections;
			this.queries = queries;
			this.sqlqueries = sqlqueries;
			this.resultSetMappings = resultSetMappings;
			this.tables = tables;
			this.imports = imports;
			this.secondPasses = secondPasses;
			this.propertyReferences = propertyReferences;
			this.namingStrategy = namingStrategy;
			this.typeDefs = typeDefs;
			this.filterDefinitions = filterDefinitions;
			this.extendsQueue = extendsQueue;
			this.auxiliaryDatabaseObjects = auxiliaryDatabaseObjects;
			this.tableNameBinding = tableNameBinding;
			this.columnNameBindingPerTable = columnNameBindingPerTable;
			this.defaultAssembly = defaultAssembly;
			this.defaultNamespace = defaultNamespace;
			this.dialect = dialect;
			this.filtersSecondPasses = filtersSecondPasses;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		public void AddClass(PersistentClass persistentClass)
		{
			if (classes.ContainsKey(persistentClass.EntityName))
				throw new DuplicateMappingException("class/entity", persistentClass.EntityName);

			classes[persistentClass.EntityName] = persistentClass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		public void AddCollection(Mapping.Collection collection)
		{
			if (collections.ContainsKey(collection.Role))
			{
				throw new DuplicateMappingException("collection role", collection.Role);
			}
			collections[collection.Role] = collection;
		}

		public void AddUniquePropertyReference(string referencedClass, string propertyName)
		{
			var upr = new PropertyReference {referencedClass = referencedClass, propertyName = propertyName, unique = true};
			propertyReferences.Add(upr);
		}

		public void AddPropertyReference(string referencedClass, string propertyName)
		{
			var upr = new PropertyReference {referencedClass = referencedClass, propertyName = propertyName};
			propertyReferences.Add(upr);
		}

		public PersistentClass GetClass(string className)
		{
			PersistentClass result;
			classes.TryGetValue(className, out result);
			return result;
		}

		public Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		/// <summary>
		/// 
		/// </summary>
		public INamingStrategy NamingStrategy
		{
			get { return namingStrategy; }
		}

		/// <summary>
		/// The default namespace for persistent classes
		/// </summary>
		public string DefaultNamespace
		{
			get { return defaultNamespace; }
			set { defaultNamespace = value; }
		}

		/// <summary>
		/// The default assembly for persistent classes
		/// </summary>
		public string DefaultAssembly
		{
			get { return defaultAssembly; }
			set { defaultAssembly = value; }
		}

		public Mapping.Collection GetCollection(string role)
		{
			return collections[role];
		}

		/// <summary>
		/// Adds an import to allow for the full class name <c>Namespace.Entity (AssemblyQualifiedName)</c> 
		/// to be referenced as <c>Entity</c> or some other name in HQL.
		/// </summary>
		/// <param name="className">The name of the type that is being renamed.</param>
		/// <param name="rename">The new name to use in HQL for the type.</param>
		/// <exception cref="MappingException">Thrown when the rename already identifies another type.</exception>
		public void AddImport(string className, string rename)
		{
			if (rename == null)
			{
				throw new ArgumentNullException("rename");
			}
			// if the imports dictionary already contains the rename, then make sure 
			// the rename is not for a different className.  If it is a different className
			// then we probably have 2 classes with the same name in a different namespace.  To 
			// prevent this error one of the classes needs to have the attribute "
			string existing;
			imports.TryGetValue(rename, out existing);
			imports[rename] = className;
			if (existing != null)
			{
				if (existing.Equals(className))
				{
					log.Info("duplicate import: " + className + "->" + rename);
				}
				else
				{
					throw new DuplicateMappingException(
						"duplicate import: " + rename + " refers to both " + className + " and " + existing
						+ " (try using auto-import=\"false\")", "import", rename);
				}
			}
		}

		public Table AddTable(string schema, string catalog, string name, string subselect, bool isAbstract, string schemaAction)
		{
			string key = subselect ?? dialect.Qualify(catalog, schema, name);
			Table table;
			if (!tables.TryGetValue(key, out table))
			{
				table = new Table();
				table.IsAbstract = isAbstract;
				table.Name = name;
				table.Schema = schema;
				table.Catalog = catalog;
				table.Subselect = subselect;
				table.SchemaActions = GetSchemaActions(schemaAction);
				tables[key] = table;
			}
			else
			{
				if (!isAbstract)
					table.IsAbstract = false;
			}

			return table;
		}

		private static SchemaAction GetSchemaActions(string schemaAction)
		{
			if (string.IsNullOrEmpty(schemaAction))
			{
				return SchemaAction.All;
			}
			else
			{
				SchemaAction sa = SchemaAction.None;
				string[] acts = schemaAction.Split(new[] {',', ' '});
				foreach (var s in acts)
				{
					switch (s.ToLowerInvariant())
					{
						case "":
						case "all":
							sa |= SchemaAction.All;
							break;
						case "drop":
							sa |= SchemaAction.Drop;
							break;
						case "update":
							sa |= SchemaAction.Update;
							break;
						case "export":
							sa |= SchemaAction.Export;
							break;
						case "validate":
							sa |= SchemaAction.Validate;
							break;
						case "none":
							sa |= SchemaAction.None;
							break;
						default:
							throw new MappingException(
								string.Format("Invalid schema-export value; Expected(all drop update export validate none), Found ({0})", s));
					}
				}
				return sa;
			}
		}

		public Table AddDenormalizedTable(string schema, string catalog, string name, bool isAbstract, string subselect, Table includedTable)
		{
			string key = subselect ?? dialect.Qualify(schema, catalog, name);

			Table table = new DenormalizedTable(includedTable)
			              	{
												IsAbstract = isAbstract, 
												Name = name, 
												Catalog = catalog, 
												Schema = schema, 
												Subselect = subselect
											};

			Table existing;
			if (tables.TryGetValue(key, out existing))
			{
				if (existing.IsPhysicalTable)
				{
					throw new DuplicateMappingException("table", name);
				}
			}

			tables[key] = table;
			return table;
		}

		public void AddTableBinding(string schema, string catalog, string logicalName, string physicalName, Table denormalizedSuperTable)
		{
			string key = BuildTableNameKey(schema, catalog, physicalName);
			TableDescription tableDescription = new TableDescription(logicalName, denormalizedSuperTable);
			TableDescription oldDescriptor;
			tableNameBinding.TryGetValue(key, out oldDescriptor);
			tableNameBinding[key] = tableDescription;
			if (oldDescriptor != null && !oldDescriptor.logicalName.Equals(logicalName))
			{
				//TODO possibly relax that
				throw new MappingException("Same physical table name reference several logical table names: " + physicalName
																	 + " => " + "'" + oldDescriptor.logicalName + "' and '" + logicalName + "'");
			}
		}

		public Table GetTable(string schema, string catalog, string name)
		{
			string key = dialect.Qualify(catalog, schema, name);
			return tables[key];
		}

		public string SchemaName
		{
			get { return schemaName; }
			set { schemaName = value; }
		}

		public string CatalogName
		{
			get { return catalogName; }
			set { catalogName = value; }
		}

		/// <summary></summary>
		public string DefaultCascade
		{
			get { return defaultCascade; }
			set { defaultCascade = value; }
		}

		/// <summary></summary>
		public string DefaultAccess
		{
			get { return defaultAccess; }
			set { defaultAccess = value; }
		}

		private void CheckQueryExists(string name)
		{
			if (queries.ContainsKey(name) || sqlqueries.ContainsKey(name))
			{
				throw new DuplicateMappingException("query / sql-query", name);
			}
		}

		public void AddQuery(string name, NamedQueryDefinition query)
		{
			CheckQueryExists(name);
			queries[name] = query;
		}

		public void AddSQLQuery(string name, NamedSQLQueryDefinition query)
		{
			CheckQueryExists(name);
			sqlqueries[name] = query;
		}

		public NamedQueryDefinition GetQuery(string name)
		{
			return queries[name];
		}

		public void AddSecondPass(SecondPassCommand command)
		{
			secondPasses.Add(command);
		}

		public void AddSecondPass(SecondPassCommand command, bool onTopOfTheQueue)
		{
			if (onTopOfTheQueue)
				secondPasses.Insert(0, command);
			else
				secondPasses.Add(command);
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the Fully Qualified Type name should
		/// automatically have an import added as the class name.
		/// </summary>
		/// <value><see langword="true" /> if the class name should be used as an import.</value>
		/// <remarks>
		/// Auto-import is used to shorten the string used to refer to types to just their
		/// unqualified name.  So if the type <c>MyAssembly.MyNamespace.MyClass, MyAssembly</c> has
		/// <c>auto-import="false"</c> then all use of it in HQL would need to be the fully qualified
		/// version <c>MyAssembly.MyNamespace.MyClass</c>. If <c>auto-import="true"</c>, the type could
		/// be referred to in HQL as just <c>MyClass</c>.
		/// </remarks>
		public bool IsAutoImport
		{
			get { return autoImport; }
			set { autoImport = value; }
		}

		public bool DefaultLazy
		{
			get { return defaultLazy; }
			set { defaultLazy = value; }
		}

		public IDictionary<string, FilterDefinition> FilterDefinitions
		{
			get { return filterDefinitions; }
		}

		public void AddFilterDefinition(FilterDefinition definition)
		{
			FilterDefinition fd;
			if (filterDefinitions.TryGetValue(definition.FilterName, out fd))
			{
				if(fd!=null)
				{
					throw new MappingException("Duplicated filter-def named: " + definition.FilterName);
				}
			}
			filterDefinitions[definition.FilterName] = definition;
		}

		public FilterDefinition GetFilterDefinition(string name)
		{
			FilterDefinition result;
			filterDefinitions.TryGetValue(name, out result);
			return result;
		}

		public void AddAuxiliaryDatabaseObject(IAuxiliaryDatabaseObject auxiliaryDatabaseObject)
		{
			auxiliaryDatabaseObjects.Add(auxiliaryDatabaseObject);
		}

		public void AddResultSetMapping(ResultSetMappingDefinition sqlResultSetMapping)
		{
			string name = sqlResultSetMapping.Name;
			if (resultSetMappings.ContainsKey(name))
			{
				throw new DuplicateMappingException("resultSet", name);
			}
			resultSetMappings[name] = sqlResultSetMapping;
		}

		public void AddToExtendsQueue(ExtendsQueueEntry entry)
		{
			if (!extendsQueue.Contains(entry))
				extendsQueue.Add(entry);
		}

		public void AddTypeDef(string typeName, string typeClass, IDictionary<string, string> paramMap)
		{
			var def = new TypeDef(typeClass, paramMap);
			typeDefs[typeName] = def;
			log.Debug("Added " + typeName + " with class " + typeClass);
		}

		public TypeDef GetTypeDef(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				return null;
			TypeDef result;
			typeDefs.TryGetValue(typeName, out result);
			return result;
		}

		#region Column Name Binding

		public void AddColumnBinding(string logicalName, Column finalColumn, Table table)
		{
			ColumnNames binding;
			if (!columnNameBindingPerTable.TryGetValue(table, out binding))
			{
				binding = new ColumnNames();
				columnNameBindingPerTable[table] = binding;
			}

			string oldFinalName;
			binding.logicalToPhysical.TryGetValue(logicalName.ToLowerInvariant(), out oldFinalName);
			binding.logicalToPhysical[logicalName.ToLowerInvariant()] = finalColumn.GetQuotedName();
			if (oldFinalName != null &&
					!(finalColumn.IsQuoted
							? oldFinalName.Equals(finalColumn.GetQuotedName())
							: oldFinalName.Equals(finalColumn.GetQuotedName(), StringComparison.InvariantCultureIgnoreCase)))
			{
				//TODO possibly relax that
				throw new MappingException("Same logical column name referenced by different physical ones: " + table.Name + "."
																	 + logicalName + " => '" + oldFinalName + "' and '" + finalColumn.GetQuotedName() + "'");
			}

			string oldLogicalName;
			binding.physicalToLogical.TryGetValue(finalColumn.GetQuotedName(), out oldLogicalName);
			binding.physicalToLogical[finalColumn.GetQuotedName()] = logicalName;
			if (oldLogicalName != null && !oldLogicalName.Equals(logicalName))
			{
				//TODO possibly relax that
				throw new MappingException("Same physical column represented by different logical column names: " + table.Name + "."
																	 + finalColumn.GetQuotedName() + " => '" + oldLogicalName + "' and '" + logicalName + "'");
			}
		}

		public string GetLogicalColumnName(string physicalName, Table table)
		{
			string logical = null;
			Table currentTable = table;
			TableDescription description;
			do
			{
				ColumnNames binding;
				if (columnNameBindingPerTable.TryGetValue(currentTable, out binding))
					binding.physicalToLogical.TryGetValue(physicalName, out logical);

				string key = BuildTableNameKey(currentTable.Schema, currentTable.Catalog, currentTable.Name);
				if (tableNameBinding.TryGetValue(key, out description))
					currentTable = description.denormalizedSupertable;
			}
			while (logical == null && currentTable != null && description != null);
			if (logical == null)
			{
				throw new MappingException("Unable to find logical column name from physical name " + physicalName + " in table " + table.Name);
			}
			return logical;
		}

		public string GetPhysicalColumnName(string logicalName, Table table)
		{
			logicalName = logicalName.ToLowerInvariant();
			string finalName = null;
			Table currentTable = table;
			do
			{
				ColumnNames binding;
				if (columnNameBindingPerTable.TryGetValue(currentTable, out binding))
					finalName = binding.logicalToPhysical[logicalName];

				string key = BuildTableNameKey(currentTable.Schema, currentTable.Catalog, currentTable.Name);
				TableDescription description;
				if (tableNameBinding.TryGetValue(key, out description))
					currentTable = description.denormalizedSupertable;
			}
			while (finalName == null && currentTable != null);
			if (finalName == null)
				throw new MappingException("Unable to find column with logical name " + logicalName + " in table " + table.Name);

			return finalName;
		}

		private static string BuildTableNameKey(string schema, string catalog, string name)
		{
			var keyBuilder = new StringBuilder();
			if (schema != null)
				keyBuilder.Append(schema);
			keyBuilder.Append(".");
			if (catalog != null)
				keyBuilder.Append(catalog);
			keyBuilder.Append(".");
			keyBuilder.Append(name);
			return keyBuilder.ToString();
		}

		#endregion

		private string GetLogicalTableName(string schema, string catalog, string physicalName)
		{
			string key = BuildTableNameKey(schema, catalog, physicalName);
			TableDescription descriptor;
			if (!tableNameBinding.TryGetValue(key, out descriptor))
			{
				throw new MappingException("Unable to find physical table: " + physicalName);
			}
			return descriptor.logicalName;
		}

		public string GetLogicalTableName(Table table)
		{
			return GetLogicalTableName(table.GetQuotedSchema(), table.Catalog, table.GetQuotedName());
		}

		public ResultSetMappingDefinition GetResultSetMapping(string name)
		{
			return resultSetMappings[name];
		}

		public IEnumerable<Mapping.Collection> IterateCollections
		{
			get { return collections.Values; }
		}

		public IEnumerable<Table> IterateTables
		{
			get { return tables.Values; }
		}

		public PersistentClass LocatePersistentClassByEntityName(string entityName)
		{
			PersistentClass persistentClass;
			if (!classes.TryGetValue(entityName, out persistentClass))
			{
				string actualEntityName;
				if (imports.TryGetValue(entityName, out actualEntityName))
					classes.TryGetValue(actualEntityName, out persistentClass);
			}
			return persistentClass;
		}

		public void ExpectedFilterDefinition(IFilterable filterable, string filterName, string condition)
		{
			var fdef = GetFilterDefinition(filterName);
			if (string.IsNullOrEmpty(condition))
			{
				if (fdef != null)
				{
					// where immediately available, apply the condition
					condition = fdef.DefaultFilterCondition;
				}
			}
			if (string.IsNullOrEmpty(condition) && fdef == null)
			{
				log.Debug(string.Format("Adding filter second pass [{0}]", filterName));
				filtersSecondPasses.Enqueue(new FilterSecondPassArgs(filterable, filterName));
			}
			else if (string.IsNullOrEmpty(condition) && fdef != null)
			{
				// Both sides does not have condition
				throw new MappingException("no filter condition found for filter: " + filterName);
			}

			if (fdef == null)
			{
				// if not available add an expected filter definition
				FilterDefinitions[filterName] = null;
			}
		}
	}

	public delegate void SecondPassCommand(IDictionary<string, PersistentClass> persistentClasses);
}
