using System;
using System.IO;
using System.Xml;
using System.Collections;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Util;
using NHibernate.Type;
using NHibernate.Mapping;
using NHibernate.Cache;
using NHibernate.Dialect;
using NHibernate.Engine;

namespace NHibernate.Cfg {
	/// <summary>
	/// An instance of <c>Configuration</c> allows the application to specify properties
	/// and mapping documents to be used when creating a <c>ISessionFactory</c>.
	/// </summary>
	/// <remarks>
	/// Usually an application will create a single <c>Configuration</c>, build a single instance
	/// of <c>ISessionFactory</c>, and then instanciate <c>ISession</c>s in threads servicing
	/// client requests.
	/// <para>
	/// The <c>Configuration</c> is meant only as an initialization-time object. <c>ISessionFactory</c>s
	/// are immutable and do not retain any assoication back to the <c>Configuration</c>
	/// </para>
	/// </remarks>
	public class Configuration : IMapping {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Configuration));

		private Hashtable classes = new Hashtable();
		private Hashtable collections = new Hashtable();
		private Hashtable tables = new Hashtable();
		private Hashtable namedQueries = new Hashtable();
		private ArrayList secondPasses = new ArrayList();
		private IInterceptor interceptor = EmptyInterceptor;
		private IDictionary properties = Environment.Properties;

		protected void Reset() {
			classes = new Hashtable();
			collections = new Hashtable();
			tables = new Hashtable();
			namedQueries = new Hashtable();
			secondPasses = new ArrayList();
			interceptor = EmptyInterceptor;
			properties = Environment.Properties;
		}

		public Configuration() {
			Reset();
		}

		/// <summary>
		/// Returns the identifier type of a mapped class
		/// </summary>
		public IType GetIdentifierType(System.Type persistentClass) {
			return ( (PersistentClass) classes[persistentClass] ).Identifier.Type;
		}

		/// <summary>
		/// The class mappings 
		/// </summary>
		public ICollection ClassMappings {
			get { return classes.Values; }
		}

		/// <summary>
		/// The collection mappings
		/// </summary>
		public ICollection CollectionMappings { 
			get { return collections.Values; }
		}

		/// <summary>
		/// The table mappings
		/// </summary>
		private ICollection TableMappings {
			get { return tables.Values; }
		}

		/// <summary>
		/// Get the mapping for a particular class
		/// </summary>
		public PersistentClass GetClassMapping(System.Type persistentClass) {
			return (PersistentClass) classes[persistentClass];
		}

		/// <summary>
		/// Get the mapping for a particular collection role
		/// </summary>
		public Mapping.Collection GetCollectionMapping(string role) {
			return (Mapping.Collection) collections[role];
		}

		public Configuration AddFile(string xmlFile) {
			log.Debug("Mapping file: " + xmlFile);
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlFile);
				Add ( doc );
			} catch (Exception e) {
				log.Error("Could not configure datastore from file: " + xmlFile, e);
				throw new MappingException(e);
			}
			return this;
		}

		/// <summary>
		/// Read mappings from a <c>String</c>
		/// </summary>
		public Configuration AddXML(string xml) {
			if ( log.IsDebugEnabled ) log.Debug("Mapping XML:\n" + xml);
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				Add ( doc );
			} catch (Exception e) {
				log.Error("Could not configure datastore from XML", e);
			}
			return this;
		}

		/// <summary>
		/// Read mappings from an XML Document
		/// </summary>
		public Configuration AddDocument(XmlDocument doc) {
			if ( log.IsDebugEnabled ) log.Debug("Mapping XML:\n" + doc.OuterXml);
			try {
				Add ( doc );
			} catch (Exception e) {
				log.Error("Could not configure datastore from XML document", e);
				throw new MappingException(e);
			}
			return this;
		}

		private void Add(XmlDocument doc) {
			try {
				Binder.BindRoot( doc, CreateMappings() );
			} catch (MappingException me) {
				log.Error("Could not compile the mapping document", me);
				throw me;
			}
		}

		/// <summary>
		/// Create a new <c>Mappings</c> to add classes and collection mappings to
		/// </summary>
		/// <returns></returns>
		public Mappings CreateMappings() {
			return new Mappings(classes, collections, tables, namedQueries, secondPasses);
		}

		public Configuration AddInputStream(Stream xmlInputStream) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlInputStream);
				Add( doc );
				return this;
			} catch (MappingException me) {
				throw me;
			} catch (Exception e) {
				log.Error("Could not configure datastore from input stream", e);
				throw new MappingException(e);
			}
		}
		

		public Configuration AddClass(System.Type persistentClass) {
			//TODO: get the xml config from the assembly
			return this;
		}

		private ICollection CollectionGenerators(Dialect.Dialect dialect) {
			Hashtable generators = new Hashtable();
			foreach(PersistentClass clazz in generators) {
				IIdentifierGenerator ig = clazz.Identifier.CreateIdentifierGenerator(dialect);
				//TODO: figure out PersistentIdentifierGenerator
			}
			return generators.Values;
		}

		private void SecondPassCompile() {
			//TODO: finish
		}

		/// <summary>
		/// The named queries
		/// </summary>
		public IDictionary NamedQueries { 
			get { return namedQueries; }
		}

		private static readonly IInterceptor EmptyInterceptor = new EmptyInterceptorClass();

		[Serializable]
		private class EmptyInterceptorClass : IInterceptor {
			
			public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
			}

			public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types) {
				return false;
			}

			public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
				return false;
			}

			public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
				return false;
			}

			public void OnPostFlush(object entity, object id, object[] currentState, string[] propertyNames, IType[] types) {
			}

			public void PostFlush(ICollection entities) {
			}

			public void PreFlush(ICollection entitites) {
			}

			public object IsUnsaved(object entity) {
				return null;
			}

			public object Instantiate(System.Type clazz, object id) {
				return null;
			}

			public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types) {
				return null;
			}
		}

		/// <summary>
		/// Instantitate a new <c>ISessionFactory</c>, using the properties and mappings in this
		/// configuration. The <c>ISessionFactory</c> will be immutable, so changes made to the
		/// <c>Configuration</c> after building the <c>ISessionFactory</c> will not affect it.
		/// </summary>
		/// <returns></returns>
		public ISessionFactory BuildSessionFactory() {
			SecondPassCompile();
			Hashtable copy = new Hashtable();
			foreach(DictionaryEntry de in properties) {
				copy.Add(de.Key, de.Value);
			}
			return new SessionFactoryImpl(this, copy, interceptor);
		}

		public IInterceptor Interceptor {
			get { return interceptor; }
			set { this.interceptor = value; }
		}

		public IDictionary Properties {
			get { return properties; }
			set { this.properties = value; }
		}

		public Configuration AddProperties(IDictionary properties) {
			foreach(DictionaryEntry de in properties) {
				this.properties.Add(de.Key, de.Value);
			}
			return this;
		}

		public void SetProperty(string name, string value) {
			properties[name] = value;
		}

		public string GetProperty(string name) {
			return properties[name] as string;
		}

		public Configuration Configure() {
			Configure("???"); //TODO: Figure this out. THis should probably be an app settings reader, and just use that
			return this;
		}

		public Configuration Configure(string resource) {
			return this; //TODO: finish this
		}
	}
}
