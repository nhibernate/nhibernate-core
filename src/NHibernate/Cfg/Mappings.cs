using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Mapping;

namespace NHibernate.Cfg 
{
	/// <summary>
	/// A collection of mappings from classes and collections to relational database tables.
	/// </summary>
	/// <remarks>Represents a single <c>&lt;hibernate-mapping&gt;</c> element.</remarks>
	public class Mappings 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Mappings));

		private IDictionary classes;
		private IDictionary collections;
		private IDictionary tables;
		private IDictionary queries;
		private IList secondPasses;
		private IDictionary imports;
		private string schemaName;
		private string defaultCascade;
		private bool autoImport;
		private string defaultAccess;
		private IDictionary caches;

		internal Mappings(IDictionary classes, IDictionary collections, IDictionary tables, IDictionary queries, IDictionary imports, IDictionary caches, IList secondPasses) 
		{
			this.classes = classes;
			this.collections = collections;
			this.queries = queries;
			this.tables = tables;
			this.imports = imports;
			this.caches = caches;
			this.secondPasses = secondPasses;
		}

		/// <summary>
		/// Associates the class name with the cache strategy.
		/// </summary>
		/// <param name="name">The classname of the class to cache.</param>
		/// <param name="cache">The <see cref="ICacheConcurrencyStrategy"/> to use for caching.</param>
		/// <exception cref="MappingException">Thrown when <c>name</c> already has a <c>cache</c> associated with it.</exception>
		public void AddCache(string name, ICacheConcurrencyStrategy cache) 
		{
			object old = caches[ name ];
			if( old!=null ) 
			{
				throw new MappingException( "duplicate cache region" );
			}
			caches[ name ] = cache;
		}

		public void AddClass(PersistentClass persistentClass) 
		{
			object old = classes[persistentClass.PersistentClazz];
			if (old!=null) log.Warn ( "duplicate class mapping: " + persistentClass.PersistentClazz.Name );
			classes[persistentClass.PersistentClazz] = persistentClass;
		}

		public void AddCollection(Mapping.Collection collection) 
		{
			object old = collections[collection.Role];
			if (old!=null) log.Warn ( "duplicate collection role: " + collection.Role );
			collections[collection.Role] = collection;
		}

		public PersistentClass GetClass(System.Type type) 
		{
			return (PersistentClass) classes[type];
		}

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
			if( imports.Contains(rename) && (string)imports[rename]!=className ) 
			{
				throw new MappingException("duplicate import: " + rename);
			}
			imports[rename] = className; 
		}

		public Table AddTable(string schema, string name) 
		{
			string key = schema != null ? schema + "." + name : name;
			Table table = (Table) tables[key];

			if (table==null) 
			{
				table = new Table();
				table.Name = name;
				table.Schema = schema;
				tables[key] = table;
			}
			return table;
		}

		public Table GetTable(string schema, string name) 
		{
			string key = schema != null ? schema + "." + name : name;
			return (Table) tables[key];
		}

		public string SchemaName 
		{
			get { return schemaName; }
			set { schemaName = value; }
		}

		public string DefaultCascade 
		{
			get {  return defaultCascade;  }
			set { defaultCascade = value; }
		}

		public string DefaultAccess 
		{
			get { return defaultAccess; }
			set { defaultAccess = value; }
		}
		public void AddQuery(string name, string query) 
		{
			object old = queries[name];
			if (old!=null) log.Warn("duplicate query name: " + name);
			queries[name] = query;
		}

		public string GetQuery(string name) 
		{
			return (string) queries[name];
		}

		internal void AddSecondPass(Binder.SecondPass sp) 
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
			get { return autoImport;  }
			set { autoImport = value; }
		}
	}
}
