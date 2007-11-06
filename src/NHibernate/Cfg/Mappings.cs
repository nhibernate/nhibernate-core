using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// A collection of mappings from classes and collections to relational database tables.
	/// </summary>
	/// <remarks>Represents a single <c>&lt;hibernate-mapping&gt;</c> element.</remarks>
	public class Mappings
	{
		private readonly IDictionary<System.Type, PersistentClass> classes;
		private readonly IDictionary<string, Mapping.Collection> collections;
		private readonly IDictionary<string, Table> tables;
		private readonly IDictionary<string, NamedQueryDefinition> queries;
		private readonly IDictionary<string, NamedSQLQueryDefinition> sqlqueries;
		private readonly IDictionary<string, ResultSetMappingDefinition> resultSetMappings;
		private readonly IList<SecondPassCommand> secondPasses;
		private readonly IDictionary<string, string> imports;
		private string schemaName;
		private string defaultCascade;
		private string defaultNamespace;
		private string defaultAssembly;
		private string defaultAccess;
		private bool autoImport;
		private bool defaultLazy;
		private readonly IList<UniquePropertyReference> propertyReferences;
		private readonly IDictionary<string, FilterDefinition> filterDefinitions;
		private readonly IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects;

		private readonly INamingStrategy namingStrategy;

		internal class UniquePropertyReference
		{
			public System.Type ReferencedClass;
			public string PropertyName;
		}

		internal Mappings(
			IDictionary<System.Type, PersistentClass> classes,
			IDictionary<string, Mapping.Collection> collections,
			IDictionary<string, Table> tables,
			IDictionary<string, NamedQueryDefinition> queries,
			IDictionary<string, NamedSQLQueryDefinition> sqlqueries,
			IDictionary<string, ResultSetMappingDefinition> resultSetMappings,
			IDictionary<string, string> imports,
			IList<SecondPassCommand> secondPasses,
			IList<UniquePropertyReference> propertyReferences,
			INamingStrategy namingStrategy,
			IDictionary<string, FilterDefinition> filterDefinitions,
			IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects,
			string defaultAssembly,
			string defaultNamespace
			)
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
			this.filterDefinitions = filterDefinitions;
			this.auxiliaryDatabaseObjects = auxiliaryDatabaseObjects;
			this.defaultAssembly = defaultAssembly;
			this.defaultNamespace = defaultNamespace;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		public void AddClass(PersistentClass persistentClass)
		{
			if (classes.ContainsKey(persistentClass.MappedClass))
			{
				throw new DuplicateMappingException("class/entity", persistentClass.MappedClass.Name);
			}

			classes[persistentClass.MappedClass] = persistentClass;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="referencedClass"></param>
		/// <param name="propertyName"></param>
		public void AddUniquePropertyReference(System.Type referencedClass, string propertyName)
		{
			UniquePropertyReference upr = new UniquePropertyReference();
			upr.ReferencedClass = referencedClass;
			upr.PropertyName = propertyName;

			propertyReferences.Add(upr);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public PersistentClass GetClass(System.Type type)
		{
			return classes[type];
		}

		public PersistentClass GetClass(string className)
		{
			return classes[ReflectHelper.ClassForName(className)];
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public Mapping.Collection GetCollection(string role)
		{
			return collections[role];
		}

		/// <summary>
		/// Adds an import to allow for the full class name <c>Namespace.Entity</c> 
		/// to be referenced as <c>Entity</c> or some other name in HQL.
		/// </summary>
		/// <param name="className">The name of the type that is being renamed.</param>
		/// <param name="rename">The new name to use in HQL for the type.</param>
		/// <exception cref="MappingException">Thrown when the rename already identifies another type.</exception>
		public void AddImport(string className, string rename)
		{
			// if the imports dictionary already contains the rename, then make sure 
			// the rename is not for a different className.  If it is a different className
			// then we probably have 2 classes with the same name in a different namespace.  To 
			// prevent this error one of the classes needs to have the attribute "
			if (imports.ContainsKey(rename) && imports[rename] != className)
			{
				object existing = imports[rename];
				throw new DuplicateMappingException("duplicate import: " + rename +
				                                    " refers to both " + className +
				                                    " and " + existing +
				                                    " (try using auto-import=\"false\")",
				                                    "import",
				                                    rename);
			}
			imports[rename] = className;
		}

		public Table AddTable(string schema, string name, bool isAbstract)
		{
			string key = Table.Qualify(schema, name);
			//string key = subselect ?? Table.Qualify(schema, name);
			Table table;
			if (!tables.TryGetValue(key, out table))
			{
				table = new Table();
				table.IsAbstract = isAbstract;
				table.Name = name;
				table.Schema = schema;
				tables[key] = table;
			}
			else
			{
				if (!isAbstract)
					table.IsAbstract = false;
			}
			return table;
		}

		public Table AddDenormalizedTable(string schema, System.String name, bool isAbstract, Table includedTable)
		{
			string key = Table.Qualify(schema, name);
			//string key = subselect ?? Table.Qualify(schema, name);
			if (tables.ContainsKey(key))
			{
				throw new DuplicateMappingException("table", name);
			}

			Table table = new DenormalizedTable(includedTable);
			table.IsAbstract = isAbstract;
			table.Name = name;
			table.Schema = schema;
			//table.Subselect= subselect;
			tables[key] = table;
			return table;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public Table GetTable(string schema, string name)
		{
			string key = schema != null ? schema + "." + name : name;
			return tables[key];
		}

		/// <summary></summary>
		public string SchemaName
		{
			get { return schemaName; }
			set { schemaName = value; }
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

		internal void AddSecondPass(SecondPassCommand command)
		{
			secondPasses.Add(command);
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the Fully Qualified Type name should
		/// automattically have an import added as the class name.
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
			filterDefinitions.Add(definition.FilterName, definition);
		}

		public FilterDefinition GetFilterDefinition(string name)
		{
			return filterDefinitions[name];
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
	}

	internal delegate void SecondPassCommand(IDictionary<System.Type, PersistentClass> persistentClasses);
}
