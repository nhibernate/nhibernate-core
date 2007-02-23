using System;
using System.Collections;
using log4net;
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
		private static readonly ILog log = LogManager.GetLogger(typeof(Mappings));

		private readonly IDictionary classes;
		private readonly IDictionary collections;
		private readonly IDictionary tables;
		private readonly IDictionary queries;
		private readonly IDictionary sqlqueries;
		private readonly IDictionary resultSetMappings;
		private readonly IList secondPasses;
		private readonly IDictionary imports;
		private string schemaName;
		private string defaultCascade;
		private string defaultNamespace;
		private string defaultAssembly;
		private string defaultAccess;
		private bool autoImport;
		private bool defaultLazy;
		private readonly IList propertyReferences;
		private readonly IDictionary filterDefinitions;
		private readonly IList auxiliaryDatabaseObjects;

		private INamingStrategy namingStrategy;

		internal class UniquePropertyReference
		{
			public System.Type ReferencedClass;
			public string PropertyName;
		}

		internal Mappings(
			IDictionary classes,
			IDictionary collections,
			IDictionary tables,
			IDictionary queries,
			IDictionary sqlqueries,
			IDictionary resultSetMappings,
			IDictionary imports,
			IList secondPasses,
			IList propertyReferences,
			INamingStrategy namingStrategy,
			IDictionary filterDefinitions,
			IList auxiliaryDatabaseObjects,
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
			object old = classes[persistentClass.MappedClass];
			if (old != null)
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
			object old = collections[collection.Role];
			if (old != null)
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
			return (PersistentClass) classes[type];
		}

		public PersistentClass GetClass(string entityName)
		{
			return (PersistentClass) classes[ReflectHelper.ClassForName(entityName)];
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
			return (Mapping.Collection) collections[role];
		}

		/// <summary>
		/// Adds an import to allow for the full class name <c>Namespace.BusClass</c> 
		/// to be referenced as <c>BusClass</c> or some other name in Hql.
		/// </summary>
		/// <param name="className">The name of the class that is being renamed.</param>
		/// <param name="rename">The new name to use in Hql for the class.</param>
		/// <exception cref="MappingException">Thrown when the rename already identifies another Class.</exception>
		public void AddImport(string className, string rename)
		{
			// if the imports dictionary already contains the rename, then make sure 
			// the rename is not for a different className.  If it is a different className
			// then we probably have 2 classes with the same name in a different namespace.  To 
			// prevent this error one of the classes needs to have the attribute "
			if (imports.Contains(rename) && (string) imports[rename] != className)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public Table AddTable(string schema, string name)
		{
			string key = schema != null ? schema + "." + name : name;
			Table table = (Table) tables[key];

			if (table == null)
			{
				table = new Table();
				table.Name = name;
				table.Schema = schema;
				tables[key] = table;
			}
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
			return (Table) tables[key];
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
			if (queries.Contains(name) || sqlqueries.Contains(name))
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
			return (NamedQueryDefinition) queries[name];
		}

		internal void AddSecondPass(ISecondPass sp)
		{
			secondPasses.Add(sp);
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the Fully Qualified Type name should
		/// automattically have an import added as the class name.
		/// </summary>
		/// <value><c>true</c> if the class name should be used as an import.</value>
		/// <remarks>
		/// AutoImport is used to shorten the string used to refer to Types to just their
		/// class.  So if the type <c>MyAssembly.MyNamespace.MyClass, MyAssembly</c> has an <c>auto-import="false"</c>
		/// then all use of in HQL would need to be the fully qualified version <c>MyAssembly.MyNamespace.MyClass</c>.
		/// If <c>auto-import="true"</c> the the Type could be referred to in hql by just the class
		/// name of <c>MyClass</c>.
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

		public IDictionary FilterDefinitions
		{
			get { return filterDefinitions; }
		}

		public void AddFilterDefinition(FilterDefinition definition)
		{
			filterDefinitions.Add(definition.FilterName, definition);
		}

		public FilterDefinition GetFilterDefinition(string name)
		{
			return (FilterDefinition) filterDefinitions[name];
		}

		public void AddAuxiliaryDatabaseObject(IAuxiliaryDatabaseObject auxiliaryDatabaseObject)
		{
			auxiliaryDatabaseObjects.Add(auxiliaryDatabaseObject);
		}

		public void AddResultSetMapping(ResultSetMappingDefinition sqlResultSetMapping)
		{
			string name = sqlResultSetMapping.Name;
			if (resultSetMappings.Contains(name))
			{
				throw new DuplicateMappingException("resultSet", name);
			}
			resultSetMappings[name] = sqlResultSetMapping;
		}
	}
}