using System;
using System.IO;
using System.Xml;
using System.Reflection;
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
		private Hashtable imports = new Hashtable();
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
			return new Mappings(classes, collections, tables, namedQueries, imports, secondPasses);
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
		
		public Configuration AddResource(string path, Assembly assembly) {
			log.Info("mapping resource: " + path);
			Stream rsrc = assembly.GetManifestResourceStream(path);
			if (rsrc==null) throw new MappingException("Resource: " + path + " not found");
			return AddInputStream(rsrc);
		}

		public Configuration AddClass(System.Type persistentClass) {
			string fileName = persistentClass.FullName + ".hbm.xml";
			log.Info("Mapping resource: " + fileName);
			Stream rsrc = persistentClass.Assembly.GetManifestResourceStream(fileName);
			if (rsrc==null) throw new MappingException("Resource: " + fileName + " not found");
			return AddInputStream(rsrc);
		}

		public Configuration AddAssembly(string assemblyName) {
			log.Info("searching for mapped documents in jar: " + assemblyName);

			Assembly assembly = null;
			try {
				assembly = Assembly.Load(assemblyName);
			} catch (Exception e) {
				log.Error("Could not configure datastore from assembly", e);
				throw new MappingException(e);
			}
			foreach(string fileName in assembly.GetManifestResourceNames() ) {
				if ( fileName.EndsWith(".hbm.xml") ) {
					log.Info( "Found mapping documents in assembly: " + fileName );
					try {
						AddInputStream( assembly.GetManifestResourceStream(fileName) );
					} catch (MappingException me) {
						throw me;
					} catch (Exception e) {
						log.Error("Could not configure datastore from assembly", e);
						throw new MappingException(e);
					}
				}
			}
			return this;
		}

		private ICollection CollectionGenerators(Dialect.Dialect dialect) {
			Hashtable generators = new Hashtable();
			foreach(PersistentClass clazz in generators) {
				IIdentifierGenerator ig = clazz.Identifier.CreateIdentifierGenerator(dialect);
				if ( ig is IPersistentIdentifierGenerator ) generators.Add(
																( (IPersistentIdentifierGenerator) ig).GeneratorKey(), ig);

			}
			return generators.Values;
		}

		/// <summary>
		/// Generate DDL for droping tables
		/// </summary>
		public string[] GenerateDropSchemaScript(Dialect.Dialect dialect) {
			SecondPassCompile();

			ArrayList script = new ArrayList(50);

			if ( dialect.DropConstraints ) {
				foreach(Table table in TableMappings) {
					foreach(ForeignKey fk in table.ForeignKeyCollection) {
						script.Add(fk.SqlDropString(dialect));
					}
				}
			}

			foreach(Table table in TableMappings) {
				script.Add( table.SqlDropString(dialect) );
			}

			foreach(IPersistentIdentifierGenerator idGen in CollectionGenerators(dialect) ) {
				string dropString = idGen.SqlDropString(dialect);
				if (dropString!=null) script.Add( dropString );
			}

			return ArrayHelper.ToStringArray(script);
		}

		/// <summary>
		/// Generate DDL for creating tables
		/// </summary>
		public string[] GenerateSchemaCreationScript(Dialect.Dialect dialect) {
			SecondPassCompile();

			ArrayList script = new ArrayList(50);

			foreach(Table table in TableMappings) {
				script.Add( table.SqlCreateString(dialect, this) );
			}

			foreach(Table table in TableMappings) {
				foreach(ForeignKey fk in table.ForeignKeyCollection) {
					script.Add( fk.SqlCreateString(dialect, this) );
				}
				foreach(Index index in table.IndexCollection) {
					script.Add( index.SqlCreateString(dialect, this) );
				}
			}

			foreach(IPersistentIdentifierGenerator idGen in CollectionGenerators(dialect)) {
				string[] lines = idGen.SqlCreateStrings(dialect);
				for (int i=0; i<lines.Length; i++ ) script.Add( lines[i] );
			}

			return ArrayHelper.ToStringArray(script);
		}

			


		private void SecondPassCompile() {
			
			foreach(Binder.SecondPass sp in secondPasses) {
				sp.DoSecondPass(classes);
			}

			secondPasses.Clear();

			foreach(Table table in TableMappings) {
				foreach(ForeignKey fk in table.ForeignKeyCollection) {
					if ( fk.ReferencedTable == null ) {
						PersistentClass referencedClass = (PersistentClass) classes[ fk.ReferencedClass ];
						if ( referencedClass==null ) throw new MappingException(
														 "An association refers to an unmapped class: " +
														 fk.ReferencedClass.Name
														 );
						fk.ReferencedTable = referencedClass.Table;
					}
				}
			}
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

		private void AddProperties(XmlNode parent) {
			foreach(XmlNode node in parent.SelectNodes("property")) {
				string name = node.Attributes["name"].Value;
				string value = node.FirstChild.Value;
				log.Debug(name + "=" + value);
				properties.Add(name, value);
				if ( !name.StartsWith("hibernate") ) properties.Add("hibernate." + name, value);
			}
		}


		public Configuration Configure() {
			Configure("hibernate.cfg.xml");
			return this;
		}

		public Configuration Configure(string resource) {
			
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(resource);
			} catch (Exception e) {
				log.Error("Problem parsing configuraiton " + resource, e);
				throw new HibernateException("problem parsing configuration " + resource + ": " + e);
			}

			XmlNode sfNode = doc.DocumentElement.SelectSingleNode("session-factory");
			XmlAttribute name = sfNode.Attributes["name"];
			if (name!=null) properties.Add(Environment.SessionFactoryName, name.Value);
			AddProperties(sfNode);

			foreach(XmlNode mapElement in sfNode.ChildNodes) {
				string elemname = mapElement.Name;
				if ( "mapping".Equals(elemname) ) {
					XmlAttribute rsrc = mapElement.Attributes["resource"];
					XmlAttribute file = mapElement.Attributes["file"];
					XmlAttribute assembly = mapElement.Attributes["assembly"];
					if (rsrc!=null) {
						log.Debug(name + "<-" + rsrc);
						AddResource( rsrc.Value, Assembly.GetExecutingAssembly() );
					} else if ( assembly!=null) {
						log.Debug(name + "<-" + assembly);
						AddAssembly(assembly.Value);
					} else {
						if (file==null) throw new MappingException("<mapping> element in configuration specifies no attributes");
						log.Debug(name + "<-" + file);
						AddFile( file.Value );
					}
				}
			}

			log.Info("Configured SessionFactory: " + name);
			log.Debug("properties: " + properties);

			return this;
		}

		/// <summary>
		/// Get the query language imports
		/// </summary>
		/// <returns></returns>
		public IDictionary Imports {
			get { return imports; }
		}
	}
}
