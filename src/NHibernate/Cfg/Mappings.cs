using System;
using System.Collections;
using NHibernate.Mapping;

namespace NHibernate.Cfg 
{
	/// <summary>
	/// A collection of mappings from classes and collections to relational database tables.
	/// </summary>
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

		internal Mappings(IDictionary classes, IDictionary collections, IDictionary tables, IDictionary queries, IDictionary imports, IList secondPasses) 
		{
			this.classes = classes;
			this.collections = collections;
			this.queries = queries;
			this.tables = tables;
			this.imports = imports;
			this.secondPasses = secondPasses;
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
		public void AddImport(string className, string rename) 
		{
			if ( imports.Contains(rename) && (string)imports[rename] != className) 
			{
				throw new MappingException("duplicate import: " + rename);
			}
			imports.Add(rename, className); 
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

		public bool IsAutoImport 
		{
			get 
			{ 
				return autoImport; 
			}
			set 
			{ 
				autoImport = value; 
			}
		}
	}
}
