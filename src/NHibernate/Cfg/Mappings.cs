using System;
using System.Collections;
using NHibernate.Mapping;

namespace NHibernate.Cfg {
	/// <summary>
	/// A collection of mappings from classes and collections to relational database tables.
	/// </summary>
	public class Mappings {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Mappings));

		private IDictionary classes;
		private IDictionary collections;
		private IDictionary tables;
		private IDictionary queries;
		private IList secondPasses;
		private string schemaName;
		private string defaultCascade;

		internal Mappings(IDictionary classes, IDictionary collections, IDictionary tables, IDictionary queries, IList secondPasses) {
			this.classes = classes;
			this.collections = collections;
			this.queries = queries;
			this.tables = tables;
			this.secondPasses = secondPasses;
		}

		public void AddClass(PersistentClass persistentClass) {
			object old = classes[persistentClass.PersistentClazz];
			if (old!=null) log.Warn ( "duplicate class mapping: " + persistentClass.PersistentClazz.Name );
			classes[persistentClass.PersistentClazz] = persistentClass;
		}
		//public void AddCollection(Collection collection) {
		//	object old = collections[collection.Role];
		//	if (old!=null) log.Warn ( "duplicate collection role: " + collection.Role );
		//	collections[collection.Role] = collection;
		//}
		public PersistentClass GetClass(System.Type type) {
			return (PersistentClass) classes[type];
		}
		//public Collection GetCollection(string role) {
		//	return (Collection) collections[role];
		//}
		public Table AddTable(string schema, string name) {
			string key = schema != null ? schema + "." + name : name;
			Table table = (Table) tables[key];

			if (table==null) {
				table = new Table();
				table.Name = name;
				table.Schema = schema;
				tables[key] = table;
			}
			return table;
		}

		public Table GetTable(string schema, string name) {
			string key = schema != null ? schema + "." + name : name;
			return (Table) tables[key];
		}
		public string SchemaName {
			get { return schemaName; }
			set { schemaName = value; }
		}
		public string DefaultCascade {
			get { return defaultCascade; }
			set { defaultCascade = value; }
		}

		public void AddQuery(string name, string query) {
			object old = queries[name];
			if (old!=null) log.Warn("duplicate query name: " + name);
			queries[name] = query;
		}
		public string GetQuery(string name) {
			return (string) queries[name];
		}
		//internal void AddSecondPass(Binder.SecondPass sp) {
		//	secondPasses.Add(sp);
		//}
	}
}
