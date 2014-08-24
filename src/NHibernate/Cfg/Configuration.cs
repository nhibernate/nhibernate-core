using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using NHibernate.Bytecode;
using NHibernate.Cfg.ConfigurationSchema;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Proxy;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using NHibernate.Util;
using Array = System.Array;
using System.Runtime.Serialization;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Allows the application to specify properties and mapping documents to be used when creating
	/// a <see cref="ISessionFactory" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Usually an application will create a single <see cref="Configuration" />, build a single instance
	/// of <see cref="ISessionFactory" />, and then instantiate <see cref="ISession"/> objects in threads
	/// servicing client requests.
	/// </para>
	/// <para>
	/// The <see cref="Configuration" /> is meant only as an initialization-time object. <see cref="ISessionFactory" />
	/// is immutable and does not retain any association back to the <see cref="Configuration" />
	/// </para>
	/// </remarks>
	[Serializable]
	public class Configuration : ISerializable
	{
		/// <summary>Default name for hibernate configuration file.</summary>
		public const string DefaultHibernateCfgFileName = "hibernate.cfg.xml";

		private string currentDocumentName;
		private bool preMappingBuildProcessed;

		protected IDictionary<string, PersistentClass> classes; // entityName, PersistentClass
		protected IDictionary<string, NHibernate.Mapping.Collection> collections;
		protected IDictionary<string, Table> tables;
		protected IList<SecondPassCommand> secondPasses;
		protected Queue<FilterSecondPassArgs> filtersSecondPasses;
		protected IList<Mappings.PropertyReference> propertyReferences;
		private IInterceptor interceptor;
		private IDictionary<string, string> properties;
		protected IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects;

		private INamingStrategy namingStrategy = DefaultNamingStrategy.Instance;
		private MappingsQueue mappingsQueue;

		private EventListeners eventListeners;
		protected IDictionary<string, TypeDef> typeDefs;
		protected ISet<ExtendsQueueEntry> extendsQueue;
		protected IDictionary<string, Mappings.TableDescription> tableNameBinding;
		protected IDictionary<Table, Mappings.ColumnNames> columnNameBindingPerTable;

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(Configuration));

		protected internal SettingsFactory settingsFactory;

		#region ISerializable Members
		public Configuration(SerializationInfo info, StreamingContext context)
		{
			Reset();

			EntityNotFoundDelegate = GetSerialedObject<IEntityNotFoundDelegate>(info, "entityNotFoundDelegate");

			auxiliaryDatabaseObjects = GetSerialedObject<IList<IAuxiliaryDatabaseObject>>(info, "auxiliaryDatabaseObjects");
			classes = GetSerialedObject<IDictionary<string, PersistentClass>>(info, "classes");
			collections = GetSerialedObject<IDictionary<string, NHibernate.Mapping.Collection>>(info, "collections");

			columnNameBindingPerTable = GetSerialedObject<IDictionary<Table, Mappings.ColumnNames>>(info,
																									"columnNameBindingPerTable");
			defaultAssembly = GetSerialedObject<string>(info, "defaultAssembly");
			defaultNamespace = GetSerialedObject<string>(info, "defaultNamespace");
			eventListeners = GetSerialedObject<EventListeners>(info, "eventListeners");
			//this.extendsQueue = GetSerialedObject<ISet<ExtendsQueueEntry>>(info, "extendsQueue");
			FilterDefinitions = GetSerialedObject<IDictionary<string, FilterDefinition>>(info, "filterDefinitions");
			Imports = GetSerialedObject<IDictionary<string, string>>(info, "imports");
			interceptor = GetSerialedObject<IInterceptor>(info, "interceptor");
			mapping = GetSerialedObject<IMapping>(info, "mapping");
			NamedQueries = GetSerialedObject<IDictionary<string, NamedQueryDefinition>>(info, "namedQueries");
			NamedSQLQueries = GetSerialedObject<IDictionary<string, NamedSQLQueryDefinition>>(info, "namedSqlQueries");
			namingStrategy = GetSerialedObject<INamingStrategy>(info, "namingStrategy");
			properties = GetSerialedObject<IDictionary<string, string>>(info, "properties");
			propertyReferences = GetSerialedObject<IList<Mappings.PropertyReference>>(info, "propertyReferences");
			settingsFactory = GetSerialedObject<SettingsFactory>(info, "settingsFactory");
			SqlFunctions = GetSerialedObject<IDictionary<string, ISQLFunction>>(info, "sqlFunctions");
			SqlResultSetMappings = GetSerialedObject<IDictionary<string, ResultSetMappingDefinition>>(info,
																									  "sqlResultSetMappings");
			tableNameBinding = GetSerialedObject<IDictionary<string, Mappings.TableDescription>>(info, "tableNameBinding");
			tables = GetSerialedObject<IDictionary<string, Table>>(info, "tables");
			typeDefs = GetSerialedObject<IDictionary<string, TypeDef>>(info, "typeDefs");
			filtersSecondPasses = GetSerialedObject<Queue<FilterSecondPassArgs>>(info, "filtersSecondPasses");
		}

		private T GetSerialedObject<T>(SerializationInfo info, string name)
		{
			return (T)info.GetValue(name, typeof(T));
		}

#if NET_4_0
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ConfigureProxyFactoryFactory();
			SecondPassCompile();
			Validate();

			info.AddValue("entityNotFoundDelegate", EntityNotFoundDelegate);

			info.AddValue("auxiliaryDatabaseObjects", auxiliaryDatabaseObjects);
			info.AddValue("classes", classes);
			info.AddValue("collections", collections);
			info.AddValue("columnNameBindingPerTable", columnNameBindingPerTable);
			info.AddValue("defaultAssembly", defaultAssembly);
			info.AddValue("defaultNamespace", defaultNamespace);
			info.AddValue("eventListeners", eventListeners);
			//info.AddValue("extendsQueue", this.extendsQueue);
			info.AddValue("filterDefinitions", FilterDefinitions);
			info.AddValue("imports", Imports);
			info.AddValue("interceptor", interceptor);
			info.AddValue("mapping", mapping);
			info.AddValue("namedQueries", NamedQueries);
			info.AddValue("namedSqlQueries", NamedSQLQueries);
			info.AddValue("namingStrategy", namingStrategy);
			info.AddValue("properties", properties);
			info.AddValue("propertyReferences", propertyReferences);
			info.AddValue("settingsFactory", settingsFactory);
			info.AddValue("sqlFunctions", SqlFunctions);
			info.AddValue("sqlResultSetMappings", SqlResultSetMappings);
			info.AddValue("tableNameBinding", tableNameBinding);
			info.AddValue("tables", tables);
			info.AddValue("typeDefs", typeDefs);
			info.AddValue("filtersSecondPasses", filtersSecondPasses);
		}
		#endregion

		/// <summary>
		/// Clear the internal state of the <see cref="Configuration"/> object.
		/// </summary>
		protected void Reset()
		{
			classes = new Dictionary<string, PersistentClass>(); //new SequencedHashMap(); - to make NH-369 bug deterministic
			Imports = new Dictionary<string, string>();
			collections = new Dictionary<string, NHibernate.Mapping.Collection>();
			tables = new Dictionary<string, Table>();
			NamedQueries = new Dictionary<string, NamedQueryDefinition>();
			NamedSQLQueries = new Dictionary<string, NamedSQLQueryDefinition>();
			SqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>();
			secondPasses = new List<SecondPassCommand>();
			propertyReferences = new List<Mappings.PropertyReference>();
			FilterDefinitions = new Dictionary<string, FilterDefinition>();
			interceptor = emptyInterceptor;
			properties = Environment.Properties;
			auxiliaryDatabaseObjects = new List<IAuxiliaryDatabaseObject>();
			SqlFunctions = new Dictionary<string, ISQLFunction>();
			mappingsQueue = new MappingsQueue();
			eventListeners = new EventListeners();
			typeDefs = new Dictionary<string, TypeDef>();
			extendsQueue = new HashSet<ExtendsQueueEntry>();
			tableNameBinding = new Dictionary<string, Mappings.TableDescription>();
			columnNameBindingPerTable = new Dictionary<Table, Mappings.ColumnNames>();
			filtersSecondPasses = new Queue<FilterSecondPassArgs>();
		}
		[Serializable]
		private class Mapping : IMapping
		{
			private readonly Configuration configuration;

			public Mapping(Configuration configuration)
			{
				this.configuration = configuration;
			}

			private PersistentClass GetPersistentClass(string className)
			{
				PersistentClass pc;
				if (!configuration.classes.TryGetValue(className, out pc))
				{
					throw new MappingException("persistent class not known: " + className);
				}
				return pc;
			}

			public IType GetIdentifierType(string className)
			{
				return GetPersistentClass(className).Identifier.Type;
			}

			public string GetIdentifierPropertyName(string className)
			{
				PersistentClass pc = GetPersistentClass(className);
				if (!pc.HasIdentifierProperty)
				{
					return null;
				}
				return pc.IdentifierProperty.Name;
			}

			public IType GetReferencedPropertyType(string className, string propertyName)
			{
				PersistentClass pc = GetPersistentClass(className);
				Property prop = pc.GetProperty(propertyName);

				if (prop == null)
				{
					throw new MappingException("property not known: " + pc.MappedClass.FullName + '.' + propertyName);
				}
				return prop.Type;
			}

			public bool HasNonIdentifierPropertyNamedId(string className)
			{
				return "id".Equals(GetIdentifierPropertyName(className));
			}
		}

		private IMapping mapping;

		protected Configuration(SettingsFactory settingsFactory)
		{
			InitBlock();
			this.settingsFactory = settingsFactory;
			Reset();
		}

		private void InitBlock()
		{
			mapping = BuildMapping();
		}

		public virtual IMapping BuildMapping()
		{
			return new Mapping(this);
		}

		/// <summary>
		/// Create a new Configuration object.
		/// </summary>
		public Configuration() : this(new SettingsFactory()) { }

		/// <summary>
		/// The class mappings 
		/// </summary>
		public ICollection<PersistentClass> ClassMappings
		{
			get { return classes.Values; }
		}

		/// <summary>
		/// The collection mappings
		/// </summary>
		public ICollection<NHibernate.Mapping.Collection> CollectionMappings
		{
			get { return collections.Values; }
		}

		/// <summary>
		/// The table mappings
		/// </summary>
		private ICollection<Table> TableMappings
		{
			get { return tables.Values; }
		}

		/// <summary>
		/// Get the mapping for a particular class
		/// </summary>
		public PersistentClass GetClassMapping(System.Type persistentClass)
		{
			// TODO NH: Remove this method
			return GetClassMapping(persistentClass.FullName);
		}

		/// <summary> Get the mapping for a particular entity </summary>
		/// <param name="entityName">An entity name. </param>
		/// <returns> the entity mapping information </returns>
		public PersistentClass GetClassMapping(string entityName)
		{
			PersistentClass result;
			classes.TryGetValue(entityName, out result);
			return result;
		}

		/// <summary>
		/// Get the mapping for a particular collection role
		/// </summary>
		/// <param name="role">a collection role</param>
		/// <returns><see cref="NHibernate.Mapping.Collection" /></returns>
		public NHibernate.Mapping.Collection GetCollectionMapping(string role)
		{
			NHibernate.Mapping.Collection result;
			collections.TryGetValue(role, out result);
			return result;
		}

		/// <summary>
		/// Read mappings from a particular XML file. This method is equivalent
		/// to <see cref="AddXmlFile(string)" />.
		/// </summary>
		/// <param name="xmlFile"></param>
		/// <returns></returns>
		public Configuration AddFile(string xmlFile)
		{
			return AddXmlFile(xmlFile);
		}

		public Configuration AddFile(FileInfo xmlFile)
		{
			return AddFile(xmlFile.FullName);
		}

		private static void LogAndThrow(Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error(exception.Message, exception);
			}

			throw exception;
		}

		/// <summary>
		/// Read mappings from a particular XML file.
		/// </summary>
		/// <param name="xmlFile">a path to a file</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXmlFile(string xmlFile)
		{
			log.Info("Mapping file: " + xmlFile);
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader(xmlFile);
				AddXmlReader(textReader, xmlFile);
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not configure datastore from file " + xmlFile, e));
			}
			finally
			{
				if (textReader != null)
				{
					textReader.Close();
				}
			}
			return this;
		}

		public Configuration AddXml(string xml)
		{
			return AddXml(xml, "(string)");
		}

		/// <summary>
		/// Read mappings from a <see cref="string" />. This method is equivalent to
		/// <see cref="AddXmlString(string)" />.
		/// </summary>
		/// <param name="xml">an XML string</param>
		/// <param name="name">The name to use in error reporting. May be <see langword="null" />.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXml(string xml, string name)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Mapping XML:\n" + xml);
			}
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(xml, XmlNodeType.Document, null);
				// make a StringReader for the string passed in - the StringReader
				// inherits from TextReader.  We can use the XmlTextReader.ctor that
				// takes the TextReader to build from a string...
				AddXmlReader(reader, name);
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not configure datastore from XML string " + name, e));
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
			return this;
		}

		/// <summary>
		/// Read mappings from a <see cref="string" />.
		/// </summary>
		/// <param name="xml">an XML string</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXmlString(string xml)
		{
			return AddXml(xml);
		}

		/// <summary>
		/// Read mappings from a URL.
		/// </summary>
		/// <param name="url">a URL</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddUrl(string url)
		{
			// AddFile works for URLs currently
			return AddFile(url);
		}

		/// <summary>
		/// Read mappings from a URL.
		/// </summary>
		/// <param name="url">a <see cref="Uri" /> to read the mappings from.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddUrl(Uri url)
		{
			return AddUrl(url.AbsolutePath);
		}

		public Configuration AddDocument(XmlDocument doc)
		{
			return AddDocument(doc, "(XmlDocument)");
		}

		/// <summary>
		/// Read mappings from an <see cref="XmlDocument" />.
		/// </summary>
		/// <param name="doc">A loaded <see cref="XmlDocument" /> that contains the mappings.</param>
		/// <param name="name">The name of the document, for error reporting purposes.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddDocument(XmlDocument doc, string name)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Mapping XML:\n" + doc.OuterXml);
			}

			try
			{
				using (var ms = new MemoryStream())
				{
					doc.Save(ms);
					ms.Position = 0;
					AddInputStream(ms, name);
				}
				return this;
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not configure datastore from XML document " + name, e));
				return this; // To please the compiler
			}
		}

		/// <summary>
		/// Takes the validated XmlDocument and has the Binder do its work of
		/// creating Mapping objects from the Mapping Xml.
		/// </summary>
		/// <param name="doc">The NamedXmlDocument that contains the <b>validated</b> mapping XML file.</param>
		private void AddValidatedDocument(NamedXmlDocument doc)
		{
			AddDeserializedMapping(doc.Document, doc.Name);
		}

		public event EventHandler<BindMappingEventArgs> BeforeBindMapping;
		public event EventHandler<BindMappingEventArgs> AfterBindMapping;

		/// <summary>
		/// Add mapping data using deserialized class.
		/// </summary>
		/// <param name="mappingDocument">Mapping metadata.</param>
		/// <param name="documentFileName">XML file's name where available; otherwise null.</param>
		public void AddDeserializedMapping(HbmMapping mappingDocument, string documentFileName)
		{
			if (mappingDocument == null)
			{
				throw new ArgumentNullException("mappingDocument");
			}
			try
			{
				Dialect.Dialect dialect = Dialect.Dialect.GetDialect(properties);
				OnBeforeBindMapping(new BindMappingEventArgs(dialect, mappingDocument, documentFileName));
				Mappings mappings = CreateMappings(dialect);

				new MappingRootBinder(mappings, dialect).Bind(mappingDocument);
				OnAfterBindMapping(new BindMappingEventArgs(dialect, mappingDocument, documentFileName));
			}
			catch (Exception e)
			{
				var message = documentFileName == null
								? "Could not compile deserialized mapping document."
								: "Could not compile the mapping document: " + documentFileName;
				LogAndThrow(new MappingException(message, e));
			}
		}

		public void AddMapping(HbmMapping mappingDocument)
		{
			AddDeserializedMapping(mappingDocument, "mapping_by_code");
		}

		private void OnAfterBindMapping(BindMappingEventArgs bindMappingEventArgs)
		{
			var handler = AfterBindMapping;
			if (handler != null)
			{
				handler(this, bindMappingEventArgs);
			}
		}

		private void OnBeforeBindMapping(BindMappingEventArgs bindMappingEventArgs)
		{
			var handler = BeforeBindMapping;
			if (handler != null)
			{
				handler(this, bindMappingEventArgs);
			}
		}

		/// <summary>
		/// Create a new <see cref="Mappings" /> to add classes and collection
		/// mappings to.
		/// </summary>
		public Mappings CreateMappings(Dialect.Dialect dialect)
		{
			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);
			string preferPooledValuesLo = PropertiesHelper.GetString(Environment.PreferPooledValuesLo, properties, null);

			ProcessPreMappingBuildProperties();
			return new Mappings(classes, collections, tables, NamedQueries, NamedSQLQueries, SqlResultSetMappings, Imports,
								secondPasses, filtersSecondPasses, propertyReferences, namingStrategy, typeDefs, FilterDefinitions, extendsQueue,
								auxiliaryDatabaseObjects, tableNameBinding, columnNameBindingPerTable, defaultAssembly,
								defaultNamespace, defaultCatalog, defaultSchema, preferPooledValuesLo, dialect);
		}

		private void ProcessPreMappingBuildProperties()
		{
			if (preMappingBuildProcessed)
			{
				return;
			}
			ConfigureCollectionTypeFactory();
			preMappingBuildProcessed = true;
		}

		private void ConfigureCollectionTypeFactory()
		{
			var ctfc = GetProperty(Environment.CollectionTypeFactoryClass);
			if (string.IsNullOrEmpty(ctfc))
			{
				return;
			}
			var ictfc = Environment.BytecodeProvider as IInjectableCollectionTypeFactoryClass;
			if (ictfc == null)
			{
				return;
			}
			ictfc.SetCollectionTypeFactoryClass(ctfc);
		}

		/// <summary>
		/// Read mappings from a <see cref="Stream" />.
		/// </summary>
		/// <param name="xmlInputStream">The stream containing XML</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The <see cref="Stream"/> passed in through the parameter <paramref name="xmlInputStream" />
		/// is not <em>guaranteed</em> to be cleaned up by this method.  It is the caller's responsiblity to
		/// ensure that <paramref name="xmlInputStream" /> is properly handled when this method
		/// completes.
		/// </remarks>
		public Configuration AddInputStream(Stream xmlInputStream)
		{
			return AddInputStream(xmlInputStream, null);
		}

		/// <summary>
		/// Read mappings from a <see cref="Stream" />.
		/// </summary>
		/// <param name="xmlInputStream">The stream containing XML</param>
		/// <param name="name">The name of the stream to use in error reporting. May be <see langword="null" />.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The <see cref="Stream"/> passed in through the parameter <paramref name="xmlInputStream" />
		/// is not <em>guaranteed</em> to be cleaned up by this method.  It is the caller's responsiblity to
		/// ensure that <paramref name="xmlInputStream" /> is properly handled when this method
		/// completes.
		/// </remarks>
		public Configuration AddInputStream(Stream xmlInputStream, string name)
		{
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader(xmlInputStream);
				AddXmlReader(textReader, name);
				return this;
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not configure datastore from input stream " + name, e));
				return this; // To please the compiler
			}
			finally
			{
				if (textReader != null)
				{
					textReader.Close();
				}
			}
		}

		/// <summary>
		/// Adds the mappings in the resource of the assembly.
		/// </summary>
		/// <param name="path">The path to the resource file in the assembly.</param>
		/// <param name="assembly">The assembly that contains the resource file.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddResource(string path, Assembly assembly)
		{
			string debugName = path;
			log.Info("Mapping resource: " + debugName);
			Stream rsrc = assembly.GetManifestResourceStream(path);
			if (rsrc == null)
			{
				LogAndThrow(new MappingException("Resource not found: " + debugName));
			}

			try
			{
				return AddInputStream(rsrc, debugName);
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not configure datastore from resource " + debugName, e));
				return this; // To please the compiler
			}
			finally
			{
				if (rsrc != null)
				{
					rsrc.Close();
				}
			}
		}

		/// <summary>
		/// Adds the mappings from embedded resources of the assembly.
		/// </summary>
		/// <param name="paths">Paths to the resource files in the assembly.</param>
		/// <param name="assembly">The assembly that contains the resource files.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddResources(IEnumerable<string> paths, Assembly assembly)
		{
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			foreach (var path in paths)
			{
				AddResource(path, assembly);
			}
			return this;
		}

		/// <summary>
		/// Read a mapping from an embedded resource, using a convention.
		/// </summary>
		/// <param name="persistentClass">The type to map.</param>
		/// <returns>This configuration object.</returns>
		/// <remarks>
		/// The convention is for class <c>Foo.Bar.Foo</c> to be mapped by
		/// the resource named <c>Foo.Bar.Foo.hbm.xml</c>, embedded in
		/// the class' assembly. If the mappings and classes are defined
		/// in different assemblies or don't follow the naming convention,
		/// this method cannot be used.
		/// </remarks>
		public Configuration AddClass(System.Type persistentClass)
		{
			return AddResource(persistentClass.FullName + ".hbm.xml", persistentClass.Assembly);
		}

		/// <summary>
		/// Adds all of the assembly's embedded resources whose names end with <c>.hbm.xml</c>.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly to load.</param>
		/// <returns>This configuration object.</returns>
		/// <remarks>
		/// The assembly must be loadable using <see cref="Assembly.Load(string)" />. If this
		/// condition is not satisfied, load the assembly manually and call
		/// <see cref="AddAssembly(Assembly)"/> instead.
		/// </remarks>
		public Configuration AddAssembly(string assemblyName)
		{
			log.Info("Searching for mapped documents in assembly: " + assemblyName);

			Assembly assembly = null;
			try
			{
				assembly = Assembly.Load(assemblyName);
			}
			catch (Exception e)
			{
				LogAndThrow(new MappingException("Could not add assembly " + assemblyName, e));
			}

			return AddAssembly(assembly);
		}

		/// <summary>
		/// Adds all of the assembly's embedded resources whose names end with <c>.hbm.xml</c>.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddAssembly(Assembly assembly)
		{
			IList<string> resourceNames = GetAllHbmXmlResourceNames(assembly);
			if (resourceNames.Count == 0)
			{
				log.Warn("No mapped documents found in assembly: " + assembly.FullName);
			}
			foreach (var name in resourceNames)
			{
				AddResource(name, assembly);
			}
			return this;
		}

		private static IList<string> GetAllHbmXmlResourceNames(Assembly assembly)
		{
			var result = new List<string>();

			foreach (var resource in assembly.GetManifestResourceNames())
			{
				if (resource.EndsWith(".hbm.xml"))
				{
					result.Add(resource);
				}
			}

			return result;
		}

		/// <summary>
		/// Read all mapping documents from a directory tree. Assume that any
		/// file named <c>*.hbm.xml</c> is a mapping document.
		/// </summary>
		/// <param name="dir">a directory</param>
		public Configuration AddDirectory(DirectoryInfo dir)
		{
			foreach (var subDirectory in dir.GetDirectories())
			{
				AddDirectory(subDirectory);
			}

			foreach (var hbmXml in dir.GetFiles("*.hbm.xml"))
			{
				AddFile(hbmXml);
			}

			return this;
		}

		/// <summary>
		/// Generate DDL for dropping tables
		/// </summary>
		/// <seealso cref="NHibernate.Tool.hbm2ddl.SchemaExport" />
		public string[] GenerateDropSchemaScript(Dialect.Dialect dialect)
		{
			SecondPassCompile();

			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			var script = new List<string>();

			if (!dialect.SupportsForeignKeyConstraintInAlterTable && !string.IsNullOrEmpty(dialect.DisableForeignKeyConstraintsString))
				script.Add(dialect.DisableForeignKeyConstraintsString);

			// drop them in reverse order in case db needs it done that way...););
			for (int i = auxiliaryDatabaseObjects.Count - 1; i >= 0; i--)
			{
				IAuxiliaryDatabaseObject auxDbObj = auxiliaryDatabaseObjects[i];
				if (auxDbObj.AppliesToDialect(dialect))
				{
					script.Add(auxDbObj.SqlDropString(dialect, defaultCatalog, defaultSchema));
				}
			}

			if (dialect.DropConstraints)
			{
				foreach (var table in TableMappings)
				{
					if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Drop))
					{
						foreach (var fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint && IncludeAction(fk.ReferencedTable.SchemaActions, SchemaAction.Drop))
							{
								script.Add(fk.SqlDropString(dialect, defaultCatalog, defaultSchema));
							}
						}
					}
				}
			}

			foreach (var table in TableMappings)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Drop))
				{
					script.Add(table.SqlDropString(dialect, defaultCatalog, defaultSchema));
				}
			}

			IEnumerable<IPersistentIdentifierGenerator> pIDg = IterateGenerators(dialect);
			foreach (var idGen in pIDg)
			{
				string[] lines = idGen.SqlDropString(dialect);
				if (lines != null)
				{
					foreach (var line in lines)
					{
						script.Add(line);
					}
				}
			}

			if (!dialect.SupportsForeignKeyConstraintInAlterTable && !string.IsNullOrEmpty(dialect.EnableForeignKeyConstraintsString))
				script.Add(dialect.EnableForeignKeyConstraintsString);

			return script.ToArray();
		}

		public static bool IncludeAction(SchemaAction actionsSource, SchemaAction includedAction)
		{
			return (actionsSource & includedAction) != SchemaAction.None;
		}

		/// <summary>
		/// Generate DDL for creating tables
		/// </summary>
		/// <param name="dialect"></param>
		public string[] GenerateSchemaCreationScript(Dialect.Dialect dialect)
		{
			SecondPassCompile();

			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			var script = new List<string>();

			foreach (var table in TableMappings)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Export))
				{
					script.Add(table.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
					script.AddRange(table.SqlCommentStrings(dialect, defaultCatalog, defaultSchema));
				}
			}

			foreach (var table in TableMappings)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Export))
				{
					if (!dialect.SupportsUniqueConstraintInCreateAlterTable)
					{
						foreach (var uk in table.UniqueKeyIterator)
						{
							string constraintString = uk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema);
							if (constraintString != null)
							{
								script.Add(constraintString);
							}
						}
					}

					foreach (var index in table.IndexIterator)
					{
						script.Add(index.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
					}

					if (dialect.SupportsForeignKeyConstraintInAlterTable)
					{
						foreach (var fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint && IncludeAction(fk.ReferencedTable.SchemaActions, SchemaAction.Export))
							{
								script.Add(fk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
							}
						}
					}
				}
			}

			IEnumerable<IPersistentIdentifierGenerator> pIDg = IterateGenerators(dialect);
			foreach (var idGen in pIDg)
			{
				script.AddRange(idGen.SqlCreateStrings(dialect));
			}

			foreach (var auxDbObj in auxiliaryDatabaseObjects)
			{
				if (auxDbObj.AppliesToDialect(dialect))
				{
					script.Add(auxDbObj.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
				}
			}

			return script.ToArray();
		}

		private void Validate()
		{
			ValidateEntities();

			ValidateCollections();

			ValidateFilterDefs();
		}

		private void ValidateFilterDefs()
		{
			var filterNames = new HashSet<string>();
			foreach (var filterDefinition in FilterDefinitions)
			{
				if (filterDefinition.Value == null)
				{
					// a class/collection has a filter but the filter-def was not added.
					filterNames.Add(filterDefinition.Key);
				}
			}
			if (filterNames.Count > 0)
			{
				var message = new StringBuilder();
				message.Append("filter-def for filter named ");
				foreach (var filterName in filterNames)
				{
					message.AppendLine(filterName);
				}
				message.AppendLine("was not found.");
				throw new MappingException(message.ToString());
			}

			// check filter-def without reference
			if (FilterDefinitions.Count > 0)
			{
				filterNames.Clear();
				foreach (var persistentClass in ClassMappings)
				{
					filterNames.UnionWith(persistentClass.FilterMap.Keys);
				}
				foreach (var collectionMapping in CollectionMappings)
				{
					filterNames.UnionWith(collectionMapping.FilterMap.Keys);
					filterNames.UnionWith(collectionMapping.ManyToManyFilterMap.Keys);
				}
				foreach (var filterName in FilterDefinitions.Keys)
				{
					if (!filterNames.Contains(filterName))
					{
						// if you are going to remove this exception at least add a log.Error
						// because the usage of filter-def, outside its scope, may cause unexpected behaviour
						// during queries.
						log.ErrorFormat("filter-def for filter named '{0}' was never used to filter classes nor collections.\r\nThis may result in unexpected behavior during queries", filterName);
					}
				}
			}
		}

		private void ValidateCollections()
		{
			foreach (var col in collections.Values)
			{
				col.Validate(mapping);
			}
		}

		private void ValidateEntities()
		{
			bool validateProxy = PropertiesHelper.GetBoolean(Environment.UseProxyValidator, properties, true);
			HashSet<string> allProxyErrors = null;
			IProxyValidator pvalidator = Environment.BytecodeProvider.ProxyFactoryFactory.ProxyValidator;

			foreach (var clazz in classes.Values)
			{
				clazz.Validate(mapping);

				if (validateProxy)
				{
					ICollection<string> errors = ValidateProxyInterface(clazz, pvalidator);
					if (errors != null)
					{
						if (allProxyErrors == null)
						{
							allProxyErrors = new HashSet<string>(errors);
						}
						else
						{
							allProxyErrors.UnionWith(errors);
						}
					}
				}
			}

			if (allProxyErrors != null)
			{
				throw new InvalidProxyTypeException(allProxyErrors);
			}
		}

		private static ICollection<string> ValidateProxyInterface(PersistentClass persistentClass, IProxyValidator validator)
		{
			if (!persistentClass.IsLazy)
			{
				// Nothing to validate
				return null;
			}

			if (persistentClass.ProxyInterface == null)
			{
				// Nothing to validate
				return null;
			}

			return validator.ValidateType(persistentClass.ProxyInterface);
		}

		/// <summary> 
		/// Call this to ensure the mappings are fully compiled/built. Usefull to ensure getting
		/// access to all information in the metamodel when calling e.g. getClassMappings().
		/// </summary>
		public virtual void BuildMappings()
		{
			SecondPassCompile();
		}

		/// <remarks>
		/// This method may be called many times!!
		/// </remarks>
		private void SecondPassCompile()
		{
			log.Info("checking mappings queue");

			mappingsQueue.CheckNoUnavailableEntries();

			log.Info("processing one-to-many association mappings");

			foreach (var command in secondPasses)
			{
				command(classes);
			}

			secondPasses.Clear();

			log.Info("processing one-to-one association property references");

			foreach (var upr in propertyReferences)
			{
				PersistentClass clazz = GetClassMapping(upr.referencedClass);
				if (clazz == null)
				{
					throw new MappingException("property-ref to unmapped class: " + upr.referencedClass);
				}

				Property prop = clazz.GetReferencedProperty(upr.propertyName);
				((SimpleValue)prop.Value).IsAlternateUniqueKey = true;
			}

			//TODO: Somehow add the newly created foreign keys to the internal collection

			log.Info("processing foreign key constraints");

			ISet<ForeignKey> done = new HashSet<ForeignKey>();
			foreach (var table in TableMappings)
			{
				SecondPassCompileForeignKeys(table, done);
			}

			log.Info("processing filters (second pass)");
			foreach (var filterSecondPassArgs in filtersSecondPasses)
			{
				FilterDefinition filterDef;
				var filterName = filterSecondPassArgs.FilterName;
				FilterDefinitions.TryGetValue(filterName, out filterDef);
				if (filterDef == null)
				{
					throw new MappingException("filter-def for filter named " + filterName + " was not found.");
				}
				if (string.IsNullOrEmpty(filterDef.DefaultFilterCondition))
				{
					throw new MappingException("no filter condition found for filter: " + filterName);
				}
				filterSecondPassArgs.Filterable.FilterMap[filterName] = filterDef.DefaultFilterCondition;
			}
		}

		private void SecondPassCompileForeignKeys(Table table, ISet<ForeignKey> done)
		{
			table.CreateForeignKeys();

			foreach (var fk in table.ForeignKeyIterator)
			{
				if (!done.Contains(fk))
				{
					done.Add(fk);

					string referencedEntityName = fk.ReferencedEntityName;
					if (string.IsNullOrEmpty(referencedEntityName))
					{
						throw new MappingException(
							string.Format("An association from the table {0} does not specify the referenced entity", fk.Table.Name));
					}

					if (log.IsDebugEnabled)
					{
						log.Debug("resolving reference to class: " + referencedEntityName);
					}

					PersistentClass referencedClass;
					if (!classes.TryGetValue(referencedEntityName, out referencedClass))
					{
						string message = string.Format("An association from the table {0} refers to an unmapped class: {1}", fk.Table.Name,
													   referencedEntityName);

						LogAndThrow(new MappingException(message));
					}
					else
					{
						if (referencedClass.IsJoinedSubclass)
						{
							SecondPassCompileForeignKeys(referencedClass.Superclass.Table, done);
						}

						try
						{
							fk.AddReferencedTable(referencedClass);
							fk.AlignColumns();
						}
						catch (MappingException me)
						{
							LogAndThrow(me);
						}
					}
				}
			}
		}

		/// <summary>
		/// The named queries
		/// </summary>
		public IDictionary<string, NamedQueryDefinition> NamedQueries { get; protected set; }

		private EventListeners GetInitializedEventListeners()
		{
			EventListeners result = eventListeners.ShallowCopy();
			result.InitializeListeners(this);
			return result;
		}

		/// <summary> 
		/// Retrieve the user-supplied delegate to handle non-existent entity scenarios.
		/// </summary>
		/// <remarks>
		/// Specify a user-supplied delegate to be used to handle scenarios where an entity could not be
		/// located by specified id.  This is mainly intended for EJB3 implementations to be able to
		/// control how proxy initialization errors should be handled...
		/// </remarks>
		public IEntityNotFoundDelegate EntityNotFoundDelegate { get; set; }

		public EventListeners EventListeners
		{
			get { return eventListeners; }
		}

		private static readonly IInterceptor emptyInterceptor = new EmptyInterceptor();
		private string defaultAssembly;
		private string defaultNamespace;

		protected virtual void ConfigureProxyFactoryFactory()
		{
			#region Way for the user to specify their own ProxyFactory

			//http://jira.nhibernate.org/browse/NH-975

			var ipff = Environment.BytecodeProvider as IInjectableProxyFactoryFactory;
			string pffClassName;
			properties.TryGetValue(Environment.ProxyFactoryFactoryClass, out pffClassName);
			if (ipff != null && !string.IsNullOrEmpty(pffClassName))
			{
				ipff.SetProxyFactoryFactory(pffClassName);
			}

			#endregion
		}
		/// <summary>
		/// Instantiate a new <see cref="ISessionFactory" />, using the properties and mappings in this
		/// configuration. The <see cref="ISessionFactory" /> will be immutable, so changes made to the
		/// configuration after building the <see cref="ISessionFactory" /> will not affect it.
		/// </summary>
		/// <returns>An <see cref="ISessionFactory" /> instance.</returns>
		public ISessionFactory BuildSessionFactory()
		{

			ConfigureProxyFactoryFactory();
			SecondPassCompile();
			Validate();
			Environment.VerifyProperties(properties);
			Settings settings = BuildSettings();

			// Ok, don't need schemas anymore, so free them
			Schemas = null;

			return new SessionFactoryImpl(this, mapping, settings, GetInitializedEventListeners());
		}

		/// <summary>
		/// Gets or sets the <see cref="IInterceptor"/> to use.
		/// </summary>
		/// <value>The <see cref="IInterceptor"/> to use.</value>
		public IInterceptor Interceptor
		{
			get { return interceptor; }
			set { interceptor = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IDictionary"/> that contains the configuration
		/// properties and their values.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> that contains the configuration
		/// properties and their values.
		/// </value>
		public IDictionary<string, string> Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>
		/// Returns the set of properties computed from the default properties in the dialect combined with the other properties in the configuration.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetDerivedProperties()
		{
			IDictionary<string, string> derivedProperties = new Dictionary<string, string>();

			if (Properties.ContainsKey(Environment.Dialect))
			{
				var dialect = Dialect.Dialect.GetDialect(Properties);
				foreach (var pair in dialect.DefaultProperties)
					derivedProperties[pair.Key] = pair.Value;
			}

			foreach (var pair in Properties)
				derivedProperties[pair.Key] = pair.Value;

			return derivedProperties;
		}

		/// <summary>
		/// Set the default assembly to use for the mappings added to the configuration
		/// afterwards.
		/// </summary>
		/// <param name="newDefaultAssembly">The default assembly name.</param>
		/// <returns>This configuration instance.</returns>
		/// <remarks>
		/// This setting can be overridden for a mapping file by setting <c>default-assembly</c>
		/// attribute of <c>&lt;hibernate-mapping&gt;</c> element.
		/// </remarks>
		public Configuration SetDefaultAssembly(string newDefaultAssembly)
		{
			defaultAssembly = newDefaultAssembly;
			return this;
		}

		/// <summary>
		/// Set the default namespace to use for the mappings added to the configuration
		/// afterwards.
		/// </summary>
		/// <param name="newDefaultNamespace">The default namespace.</param>
		/// <returns>This configuration instance.</returns>
		/// <remarks>
		/// This setting can be overridden for a mapping file by setting <c>default-namespace</c>
		/// attribute of <c>&lt;hibernate-mapping&gt;</c> element.
		/// </remarks>
		public Configuration SetDefaultNamespace(string newDefaultNamespace)
		{
			defaultNamespace = newDefaultNamespace;
			return this;
		}

		/// <summary>
		/// Sets the default interceptor for use by all sessions.
		/// </summary>
		/// <param name="newInterceptor">The default interceptor.</param>
		/// <returns>This configuration instance.</returns>
		public Configuration SetInterceptor(IInterceptor newInterceptor)
		{
			interceptor = newInterceptor;
			return this;
		}

		/// <summary>
		/// Specify a completely new set of properties
		/// </summary>
		public Configuration SetProperties(IDictionary<string, string> newProperties)
		{
			properties = newProperties;
			return this;
		}

		/// <summary>
		/// Adds an <see cref="IDictionary"/> of configuration properties.  The 
		/// Key is the name of the Property and the Value is the <see cref="String"/>
		/// value of the Property.
		/// </summary>
		/// <param name="additionalProperties">An <see cref="IDictionary"/> of configuration properties.</param>
		/// <returns>
		/// This <see cref="Configuration"/> object.
		/// </returns>
		public Configuration AddProperties(IDictionary<string, string> additionalProperties)
		{
			foreach (var de in additionalProperties)
			{
				properties.Add(de.Key, de.Value);
			}
			return this;
		}

		/// <summary>
		/// Sets the value of the configuration property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <returns>
		/// This configuration object.
		/// </returns>
		public Configuration SetProperty(string name, string value)
		{
			properties[name] = value;
			return this;
		}

		/// <summary>
		/// Gets the value of the configuration property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The configured value of the property, or <see langword="null" /> if the property was not specified.</returns>
		public string GetProperty(string name)
		{
			return PropertiesHelper.GetString(name, properties, null);
		}

		private void AddProperties(ISessionFactoryConfiguration factoryConfiguration)
		{
			foreach (var kvp in factoryConfiguration.Properties)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug(kvp.Key + "=" + kvp.Value);
				}
				properties[kvp.Key] = kvp.Value;
			}
			Environment.VerifyProperties(properties);
		}

		// TODO - getConfigurationInputStream(String resource)

		/// <summary>
		/// Configure NHibernate using the <c>&lt;hibernate-configuration&gt;</c> section
		/// from the application config file, if found, or the file <c>hibernate.cfg.xml</c> if the
		/// <c>&lt;hibernate-configuration&gt;</c> section not include the session-factory configuration.
		/// </summary>
		/// <returns>A configuration object initialized with the file.</returns>
		/// <remarks>
		/// To configure NHibernate explicitly using <c>hibernate.cfg.xml</c>, appling merge/override
		/// of the application configuration file, use this code:
		/// <code>
		///		configuration.Configure("path/to/hibernate.cfg.xml");
		/// </code>
		/// </remarks>
		public Configuration Configure()
		{
			var hc = ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as IHibernateConfiguration;
			if (hc != null && hc.SessionFactory != null)
			{
				return DoConfigure(hc.SessionFactory);
			}
			else
			{
				return Configure(GetDefaultConfigurationFilePath());
			}
		}

		/// <summary>
		/// Configure NHibernate using the file specified.
		/// </summary>
		/// <param name="fileName">The location of the XML file to use to configure NHibernate.</param>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure(string) will override/merge the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure(string fileName)
		{
			return Configure(fileName, false);
		}

		private Configuration Configure(string fileName, bool ignoreSessionFactoryConfig)
		{
			if (ignoreSessionFactoryConfig)
			{
				Environment.ResetSessionFactoryProperties();
				properties = Environment.Properties;
			}

			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(fileName);
				return Configure(reader);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
		}

		/// <summary>
		/// Configure NHibernate using a resource contained in an Assembly.
		/// </summary>
		/// <param name="assembly">The <see cref="Assembly"/> that contains the resource.</param>
		/// <param name="resourceName">The name of the manifest resource being requested.</param>
		/// <returns>A Configuration object initialized from the manifest resource.</returns>
		/// <remarks>
		/// Calling Configure(Assembly, string) will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure(Assembly assembly, string resourceName)
		{
			if (assembly == null)
			{
				throw new HibernateException("Could not configure NHibernate.", new ArgumentNullException("assembly"));
			}

			if (resourceName == null)
			{
				throw new HibernateException("Could not configure NHibernate.", new ArgumentNullException("resourceName"));
			}

			Stream stream = null;
			try
			{
				stream = assembly.GetManifestResourceStream(resourceName);
				if (stream == null)
				{
					// resource does not exist - throw appropriate exception 
					throw new HibernateException("A ManifestResourceStream could not be created for the resource " + resourceName
												 + " in Assembly " + assembly.FullName);
				}

				return Configure(new XmlTextReader(stream));
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		/// <summary>
		/// Configure NHibernate using the specified XmlReader.
		/// </summary>
		/// <param name="textReader">The <see cref="XmlReader"/> that contains the Xml to configure NHibernate.</param>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure(XmlReader) will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure(XmlReader textReader)
		{
			if (textReader == null)
			{
				throw new HibernateConfigException("Could not configure NHibernate.",
												   new ArgumentException("A null value was passed in.", "textReader"));
			}

			try
			{
				IHibernateConfiguration hc = new HibernateConfiguration(textReader);
				return DoConfigure(hc.SessionFactory);
			}
			catch (Exception e)
			{
				log.Error("Problem parsing configuration", e);
				throw;
			}
		}

		// Not ported - configure(org.w3c.dom.Document)

		protected Configuration DoConfigure(ISessionFactoryConfiguration factoryConfiguration)
		{
			if (!string.IsNullOrEmpty(factoryConfiguration.Name))
			{
				properties[Environment.SessionFactoryName] = factoryConfiguration.Name;
			}

			AddProperties(factoryConfiguration);

			// Load mappings
			foreach (var mc in factoryConfiguration.Mappings)
			{
				if (mc.IsEmpty())
				{
					throw new HibernateConfigException("<mapping> element in configuration specifies no attributes");
				}
				if (!string.IsNullOrEmpty(mc.Resource) && !string.IsNullOrEmpty(mc.Assembly))
				{
					log.Debug(factoryConfiguration.Name + "<-" + mc.Resource + " in " + mc.Assembly);
					AddResource(mc.Resource, Assembly.Load(mc.Assembly));
				}
				else if (!string.IsNullOrEmpty(mc.Assembly))
				{
					log.Debug(factoryConfiguration.Name + "<-" + mc.Assembly);
					AddAssembly(mc.Assembly);
				}
				else if (!string.IsNullOrEmpty(mc.File))
				{
					log.Debug(factoryConfiguration.Name + "<-" + mc.File);
					AddFile(mc.File);
				}
			}

			// Load class-cache
			foreach (var ccc in factoryConfiguration.ClassesCache)
			{
				string region = string.IsNullOrEmpty(ccc.Region) ? ccc.Class : ccc.Region;
				bool includeLazy = (ccc.Include != ClassCacheInclude.NonLazy);
				SetCacheConcurrencyStrategy(ccc.Class, EntityCacheUsageParser.ToString(ccc.Usage), region, includeLazy);
			}

			// Load collection-cache
			foreach (var ccc in factoryConfiguration.CollectionsCache)
			{
				string role = ccc.Collection;
				NHibernate.Mapping.Collection collection = GetCollectionMapping(role);
				if (collection == null)
				{
					throw new HibernateConfigException(
						"collection-cache Configuration: Cannot configure cache for unknown collection role " + role);
				}

				string region = string.IsNullOrEmpty(ccc.Region) ? role : ccc.Region;
				SetCollectionCacheConcurrencyStrategy(role, EntityCacheUsageParser.ToString(ccc.Usage), region);
			}

			// Events
			foreach (var ec in factoryConfiguration.Events)
			{
				var listenerClasses = new string[ec.Listeners.Count];
				for (int i = 0; i < ec.Listeners.Count; i++)
				{
					listenerClasses[i] = ec.Listeners[i].Class;
				}
				log.Debug("Event listeners: " + ec.Type + "=" + StringHelper.ToString(listenerClasses));
				SetListeners(ec.Type, listenerClasses);
			}
			// Listeners
			foreach (var lc in factoryConfiguration.Listeners)
			{
				log.Debug("Event listener: " + lc.Type + "=" + lc.Class);
				SetListeners(lc.Type, new[] { lc.Class });
			}

			if (!string.IsNullOrEmpty(factoryConfiguration.Name))
			{
				log.Info("Configured SessionFactory: " + factoryConfiguration.Name);
			}
			log.Debug("properties: " + properties);

			return this;
		}

		internal RootClass GetRootClassMapping(string clazz)
		{
			try
			{
				return (RootClass)GetClassMapping(clazz);
			}
			catch (InvalidCastException)
			{
				throw new HibernateConfigException(
					"class-cache Configuration: You may only specify a cache for root <class> mappings " + "(cache was specified for "
					+ clazz + ")");
			}
		}

		internal RootClass GetRootClassMapping(System.Type clazz)
		{
			PersistentClass persistentClass = GetClassMapping(clazz);

			if (persistentClass == null)
			{
				throw new HibernateConfigException("class-cache Configuration: Cache specified for unmapped class " + clazz);
			}

			var rootClass = persistentClass as RootClass;

			if (rootClass == null)
			{
				throw new HibernateConfigException(
					"class-cache Configuration: You may only specify a cache for root <class> mappings " + "(cache was specified for "
					+ clazz + ")");
			}

			return rootClass;
		}

		/// <summary>
		/// Set up a cache for an entity class
		/// </summary>
		public Configuration SetCacheConcurrencyStrategy(String clazz, String concurrencyStrategy)
		{
			SetCacheConcurrencyStrategy(clazz, concurrencyStrategy, clazz);
			return this;
		}

		public void SetCacheConcurrencyStrategy(String clazz, String concurrencyStrategy, String region)
		{
			SetCacheConcurrencyStrategy(clazz, concurrencyStrategy, region, true);
		}

		internal void SetCacheConcurrencyStrategy(String clazz, String concurrencyStrategy, String region, bool includeLazy)
		{
			RootClass rootClass = GetRootClassMapping(StringHelper.GetFullClassname(clazz));
			if (rootClass == null)
			{
				throw new HibernateConfigException("Cannot cache an unknown entity: " + clazz);
			}
			rootClass.CacheConcurrencyStrategy = concurrencyStrategy;
			rootClass.CacheRegionName = region;
			rootClass.SetLazyPropertiesCacheable(includeLazy);
		}

		/// <summary>
		/// Set up a cache for a collection role
		/// </summary>
		public Configuration SetCollectionCacheConcurrencyStrategy(string collectionRole, string concurrencyStrategy)
		{
			SetCollectionCacheConcurrencyStrategy(collectionRole, concurrencyStrategy, collectionRole);
			return this;
		}

		internal void SetCollectionCacheConcurrencyStrategy(string collectionRole, string concurrencyStrategy, string region)
		{
			NHibernate.Mapping.Collection collection = GetCollectionMapping(collectionRole);
			collection.CacheConcurrencyStrategy = concurrencyStrategy;
			collection.CacheRegionName = region;
		}

		/// <summary>
		/// Get the query language imports (entityName/className -> AssemblyQualifiedName)
		/// </summary>
		public IDictionary<string, string> Imports { get; protected set; }

		/// <summary>
		/// Create an object-oriented view of the configuration properties
		/// </summary>
		/// <returns>A <see cref="Settings"/> object initialized from the settings properties.</returns>
		//protected Settings BuildSettings()
		private Settings BuildSettings()
		{
			var result = settingsFactory.BuildSettings(GetDerivedProperties());
			// NH : Set configuration for IdGenerator SQL logging
			PersistentIdGeneratorParmsNames.SqlStatementLogger.FormatSql = result.SqlStatementLogger.FormatSql;
			PersistentIdGeneratorParmsNames.SqlStatementLogger.LogToStdout = result.SqlStatementLogger.LogToStdout;
			return result;
		}

		/// <summary>
		/// The named SQL queries
		/// </summary>
		public IDictionary<string, NamedSQLQueryDefinition> NamedSQLQueries
		{
			get;
			protected set;
		}

		/// <summary>
		/// Naming strategy for tables and columns
		/// </summary>
		public INamingStrategy NamingStrategy
		{
			get { return namingStrategy; }
		}

		/// <summary>
		/// Set a custom naming strategy
		/// </summary>
		/// <param name="newNamingStrategy">the NamingStrategy to set</param>
		/// <returns></returns>
		public Configuration SetNamingStrategy(INamingStrategy newNamingStrategy)
		{
			namingStrategy = newNamingStrategy;
			return this;
		}

		public IDictionary<string, ResultSetMappingDefinition> SqlResultSetMappings
		{
			get;
			protected set;
		}

		public IDictionary<string, FilterDefinition> FilterDefinitions { get; protected set; }

		public void AddFilterDefinition(FilterDefinition definition)
		{
			FilterDefinitions.Add(definition.FilterName, definition);
		}

		public void AddAuxiliaryDatabaseObject(IAuxiliaryDatabaseObject obj)
		{
			auxiliaryDatabaseObjects.Add(obj);
		}

		public IDictionary<string, ISQLFunction> SqlFunctions { get; protected set; }

		public void AddSqlFunction(string functionName, ISQLFunction sqlFunction)
		{
			SqlFunctions[functionName] = sqlFunction;
		}

		#region NHibernate-Specific Members

		/// <summary>
		/// Load and validate the mappings in the <see cref="XmlReader" /> against
		/// the nhibernate-mapping-2.2 schema, without adding them to the configuration.
		/// </summary>
		/// <remarks>
		/// This method is made public to be usable from the unit tests. It is not intended
		/// to be called by end users.
		/// </remarks>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <param name="name">The name of the document, for error reporting purposes.</param>
		/// <returns>NamedXmlDocument containing the validated XmlDocument built from the XmlReader.</returns>
		public NamedXmlDocument LoadMappingDocument(XmlReader hbmReader, string name)
		{
			XmlReaderSettings settings = Schemas.CreateMappingReaderSettings();
			settings.ValidationEventHandler += ValidationHandler;

			using (XmlReader reader = XmlReader.Create(hbmReader, settings))
			{
				Debug.Assert(currentDocumentName == null);
				currentDocumentName = name;

				try
				{
					var hbmDocument = new XmlDocument();
					hbmDocument.Load(reader);
					return new NamedXmlDocument(name, hbmDocument);
				}
				catch (MappingException)
				{
					throw;
				}
				catch (Exception e)
				{
					string nameFormatted = name ?? "(unknown)";
					LogAndThrow(new MappingException("Could not compile the mapping document: " + nameFormatted, e));
				}
				finally
				{
					currentDocumentName = null;
				}
			}
			return null;
		}

		/// <summary>
		/// Adds the Mappings in the <see cref="XmlReader"/> after validating it
		/// against the nhibernate-mapping-2.2 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader(XmlReader hbmReader)
		{
			return AddXmlReader(hbmReader, null);
		}

		/// <summary>
		/// Adds the Mappings in the <see cref="XmlReader"/> after validating it
		/// against the nhibernate-mapping-2.2 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <param name="name">The name of the document to use for error reporting. May be <see langword="null" />.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader(XmlReader hbmReader, string name)
		{
			NamedXmlDocument document = LoadMappingDocument(hbmReader, name);
			AddDocumentThroughQueue(document);
			return this;
		}

		private void AddDocumentThroughQueue(NamedXmlDocument document)
		{
			mappingsQueue.AddDocument(document);
			ProcessMappingsQueue();
		}

		private void ProcessMappingsQueue()
		{
			NamedXmlDocument document;

			while ((document = mappingsQueue.GetNextAvailableResource()) != null)
			{
				AddValidatedDocument(document);
			}
		}

		private void ValidationHandler(object o, ValidationEventArgs args)
		{
			string message = string.Format("{0}({1},{2}): XML validation error: {3}", currentDocumentName,
										   args.Exception.LineNumber, args.Exception.LinePosition, args.Exception.Message);
			LogAndThrow(new MappingException(message, args.Exception));
		}

		protected virtual string GetDefaultConfigurationFilePath()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;

			// Note RelativeSearchPath can be null even if the doc say something else; don't remove the check
			var searchPath = AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty;

			string relativeSearchPath = searchPath.Split(';').First();
			string binPath = Path.Combine(baseDir, relativeSearchPath);
			return Path.Combine(binPath, DefaultHibernateCfgFileName);
		}

		#endregion

		private XmlSchemas schemas;

		private XmlSchemas Schemas
		{
			get { return schemas = schemas ?? new XmlSchemas(); }
			set { schemas = value; }
		}

		/// <summary>
		/// Set or clear listener for a given <see cref="ListenerType"/>.
		/// </summary>
		/// <param name="type">The <see cref="ListenerType"/>.</param>
		/// <param name="listenerClasses">The array of AssemblyQualifiedName of each listener for <paramref name="type"/>.</param>
		/// <remarks>
		/// <paramref name="listenerClasses"/> must implements the interface related with <paramref name="type"/>.
		/// All listeners of the given <see cref="ListenerType"/> will be cleared if the <paramref name="listenerClasses"/> 
		/// is null or empty.
		/// </remarks>
		/// <exception cref="MappingException">
		/// when an element of <paramref name="listenerClasses"/> have an invalid value or cant be instantiated.
		/// </exception>
		public void SetListeners(ListenerType type, string[] listenerClasses)
		{
			if (listenerClasses == null || listenerClasses.Length == 0)
			{
				ClearListeners(type);
			}
			else
			{
				var listeners = (object[])Array.CreateInstance(eventListeners.GetListenerClassFor(type), listenerClasses.Length);
				for (int i = 0; i < listeners.Length; i++)
				{
					try
					{
						listeners[i] = Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(listenerClasses[i]));
					}
					catch (Exception e)
					{
						throw new MappingException(
							"Unable to instantiate specified event (" + type + ") listener class: " + listenerClasses[i], e);
					}
				}
				SetListeners(type, listeners);
			}
		}

		/// <summary>
		/// Set or clear listener for a given <see cref="ListenerType"/>.
		/// </summary>
		/// <param name="type">The <see cref="ListenerType"/>.</param>
		/// <param name="listener">The listener for <paramref name="type"/> or null to clear.</param>
		/// <remarks><paramref name="listener"/> must implements the interface related with <paramref name="type"/>.</remarks>
		/// <seealso cref="NHibernate.Event"/>
		public void SetListener(ListenerType type, object listener)
		{
			if (listener == null)
			{
				ClearListeners(type);
			}
			else
			{
				var listeners = (object[])Array.CreateInstance(eventListeners.GetListenerClassFor(type), 1);
				listeners[0] = listener;
				SetListeners(type, listeners);
			}
		}

		private void ClearListeners(ListenerType type)
		{
			switch (type)
			{
				case ListenerType.Autoflush:
					eventListeners.AutoFlushEventListeners = new IAutoFlushEventListener[] { };
					break;
				case ListenerType.Merge:
					eventListeners.MergeEventListeners = new IMergeEventListener[] { };
					break;
				case ListenerType.Create:
					eventListeners.PersistEventListeners = new IPersistEventListener[] { };
					break;
				case ListenerType.CreateOnFlush:
					eventListeners.PersistOnFlushEventListeners = new IPersistEventListener[] { };
					break;
				case ListenerType.Delete:
					eventListeners.DeleteEventListeners = new IDeleteEventListener[] { };
					break;
				case ListenerType.DirtyCheck:
					eventListeners.DirtyCheckEventListeners = new IDirtyCheckEventListener[] { };
					break;
				case ListenerType.Evict:
					eventListeners.EvictEventListeners = new IEvictEventListener[] { };
					break;
				case ListenerType.Flush:
					eventListeners.FlushEventListeners = new IFlushEventListener[] { };
					break;
				case ListenerType.FlushEntity:
					eventListeners.FlushEntityEventListeners = new IFlushEntityEventListener[] { };
					break;
				case ListenerType.Load:
					eventListeners.LoadEventListeners = new ILoadEventListener[] { };
					break;
				case ListenerType.LoadCollection:
					eventListeners.InitializeCollectionEventListeners = new IInitializeCollectionEventListener[] { };
					break;
				case ListenerType.Lock:
					eventListeners.LockEventListeners = new ILockEventListener[] { };
					break;
				case ListenerType.Refresh:
					eventListeners.RefreshEventListeners = new IRefreshEventListener[] { };
					break;
				case ListenerType.Replicate:
					eventListeners.ReplicateEventListeners = new IReplicateEventListener[] { };
					break;
				case ListenerType.SaveUpdate:
					eventListeners.SaveOrUpdateEventListeners = new ISaveOrUpdateEventListener[] { };
					break;
				case ListenerType.Save:
					eventListeners.SaveEventListeners = new ISaveOrUpdateEventListener[] { };
					break;
				case ListenerType.PreUpdate:
					eventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[] { };
					break;
				case ListenerType.Update:
					eventListeners.UpdateEventListeners = new ISaveOrUpdateEventListener[] { };
					break;
				case ListenerType.PreLoad:
					eventListeners.PreLoadEventListeners = new IPreLoadEventListener[] { };
					break;
				case ListenerType.PreDelete:
					eventListeners.PreDeleteEventListeners = new IPreDeleteEventListener[] { };
					break;
				case ListenerType.PreInsert:
					eventListeners.PreInsertEventListeners = new IPreInsertEventListener[] { };
					break;
				case ListenerType.PostLoad:
					eventListeners.PostLoadEventListeners = new IPostLoadEventListener[] { };
					break;
				case ListenerType.PostInsert:
					eventListeners.PostInsertEventListeners = new IPostInsertEventListener[] { };
					break;
				case ListenerType.PostUpdate:
					eventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[] { };
					break;
				case ListenerType.PostDelete:
					eventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[] { };
					break;
				case ListenerType.PostCommitUpdate:
					eventListeners.PostCommitUpdateEventListeners = new IPostUpdateEventListener[] { };
					break;
				case ListenerType.PostCommitInsert:
					eventListeners.PostCommitInsertEventListeners = new IPostInsertEventListener[] { };
					break;
				case ListenerType.PostCommitDelete:
					eventListeners.PostCommitDeleteEventListeners = new IPostDeleteEventListener[] { };
					break;
				case ListenerType.PreCollectionRecreate:
					eventListeners.PreCollectionRecreateEventListeners = new IPreCollectionRecreateEventListener[] { };
					break;
				case ListenerType.PreCollectionRemove:
					eventListeners.PreCollectionRemoveEventListeners = new IPreCollectionRemoveEventListener[] { };
					break;
				case ListenerType.PreCollectionUpdate:
					eventListeners.PreCollectionUpdateEventListeners = new IPreCollectionUpdateEventListener[] { };
					break;
				case ListenerType.PostCollectionRecreate:
					eventListeners.PostCollectionRecreateEventListeners = new IPostCollectionRecreateEventListener[] { };
					break;
				case ListenerType.PostCollectionRemove:
					eventListeners.PostCollectionRemoveEventListeners = new IPostCollectionRemoveEventListener[] { };
					break;
				case ListenerType.PostCollectionUpdate:
					eventListeners.PostCollectionUpdateEventListeners = new IPostCollectionUpdateEventListener[] { };
					break;
				default:
					log.Warn("Unrecognized listener type [" + type + "]");
					break;
			}
		}

		/// <summary>
		/// Set or clear listeners for a given <see cref="ListenerType"/>.
		/// </summary>
		/// <param name="type">The <see cref="ListenerType"/>.</param>
		/// <param name="listeners">The listener for <paramref name="type"/> or null to clear.</param>
		/// <remarks>Listeners of <paramref name="listeners"/> must implements one of the interface of event listenesr.</remarks>
		/// <seealso cref="NHibernate.Event"/>
		public void SetListeners(ListenerType type, object[] listeners)
		{
			if (listeners == null)
			{
				ClearListeners(type);
			}
			switch (type)
			{
				case ListenerType.Autoflush:
					eventListeners.AutoFlushEventListeners = (IAutoFlushEventListener[])listeners;
					break;
				case ListenerType.Merge:
					eventListeners.MergeEventListeners = (IMergeEventListener[])listeners;
					break;
				case ListenerType.Create:
					eventListeners.PersistEventListeners = (IPersistEventListener[])listeners;
					break;
				case ListenerType.CreateOnFlush:
					eventListeners.PersistOnFlushEventListeners = (IPersistEventListener[])listeners;
					break;
				case ListenerType.Delete:
					eventListeners.DeleteEventListeners = (IDeleteEventListener[])listeners;
					break;
				case ListenerType.DirtyCheck:
					eventListeners.DirtyCheckEventListeners = (IDirtyCheckEventListener[])listeners;
					break;
				case ListenerType.Evict:
					eventListeners.EvictEventListeners = (IEvictEventListener[])listeners;
					break;
				case ListenerType.Flush:
					eventListeners.FlushEventListeners = (IFlushEventListener[])listeners;
					break;
				case ListenerType.FlushEntity:
					eventListeners.FlushEntityEventListeners = (IFlushEntityEventListener[])listeners;
					break;
				case ListenerType.Load:
					eventListeners.LoadEventListeners = (ILoadEventListener[])listeners;
					break;
				case ListenerType.LoadCollection:
					eventListeners.InitializeCollectionEventListeners = (IInitializeCollectionEventListener[])listeners;
					break;
				case ListenerType.Lock:
					eventListeners.LockEventListeners = (ILockEventListener[])listeners;
					break;
				case ListenerType.Refresh:
					eventListeners.RefreshEventListeners = (IRefreshEventListener[])listeners;
					break;
				case ListenerType.Replicate:
					eventListeners.ReplicateEventListeners = (IReplicateEventListener[])listeners;
					break;
				case ListenerType.SaveUpdate:
					eventListeners.SaveOrUpdateEventListeners = (ISaveOrUpdateEventListener[])listeners;
					break;
				case ListenerType.Save:
					eventListeners.SaveEventListeners = (ISaveOrUpdateEventListener[])listeners;
					break;
				case ListenerType.PreUpdate:
					eventListeners.PreUpdateEventListeners = (IPreUpdateEventListener[])listeners;
					break;
				case ListenerType.Update:
					eventListeners.UpdateEventListeners = (ISaveOrUpdateEventListener[])listeners;
					break;
				case ListenerType.PreLoad:
					eventListeners.PreLoadEventListeners = (IPreLoadEventListener[])listeners;
					break;
				case ListenerType.PreDelete:
					eventListeners.PreDeleteEventListeners = (IPreDeleteEventListener[])listeners;
					break;
				case ListenerType.PreInsert:
					eventListeners.PreInsertEventListeners = (IPreInsertEventListener[])listeners;
					break;
				case ListenerType.PostLoad:
					eventListeners.PostLoadEventListeners = (IPostLoadEventListener[])listeners;
					break;
				case ListenerType.PostInsert:
					eventListeners.PostInsertEventListeners = (IPostInsertEventListener[])listeners;
					break;
				case ListenerType.PostUpdate:
					eventListeners.PostUpdateEventListeners = (IPostUpdateEventListener[])listeners;
					break;
				case ListenerType.PostDelete:
					eventListeners.PostDeleteEventListeners = (IPostDeleteEventListener[])listeners;
					break;
				case ListenerType.PostCommitUpdate:
					eventListeners.PostCommitUpdateEventListeners = (IPostUpdateEventListener[])listeners;
					break;
				case ListenerType.PostCommitInsert:
					eventListeners.PostCommitInsertEventListeners = (IPostInsertEventListener[])listeners;
					break;
				case ListenerType.PostCommitDelete:
					eventListeners.PostCommitDeleteEventListeners = (IPostDeleteEventListener[])listeners;
					break;
				case ListenerType.PreCollectionRecreate:
					eventListeners.PreCollectionRecreateEventListeners = (IPreCollectionRecreateEventListener[])listeners;
					break;
				case ListenerType.PreCollectionRemove:
					eventListeners.PreCollectionRemoveEventListeners = (IPreCollectionRemoveEventListener[])listeners;
					break;
				case ListenerType.PreCollectionUpdate:
					eventListeners.PreCollectionUpdateEventListeners = (IPreCollectionUpdateEventListener[])listeners;
					break;
				case ListenerType.PostCollectionRecreate:
					eventListeners.PostCollectionRecreateEventListeners = (IPostCollectionRecreateEventListener[])listeners;
					break;
				case ListenerType.PostCollectionRemove:
					eventListeners.PostCollectionRemoveEventListeners = (IPostCollectionRemoveEventListener[])listeners;
					break;
				case ListenerType.PostCollectionUpdate:
					eventListeners.PostCollectionUpdateEventListeners = (IPostCollectionUpdateEventListener[])listeners;
					break;
				default:
					log.Warn("Unrecognized listener type [" + type + "]");
					break;
			}
		}


		/// <summary>
		/// Append the listeners to the end of the currently configured
		/// listeners
		/// </summary>
		public void AppendListeners(ListenerType type, object[] listeners)
		{
			switch (type)
			{
				case ListenerType.Autoflush:
					eventListeners.AutoFlushEventListeners = AppendListeners(eventListeners.AutoFlushEventListeners, (IAutoFlushEventListener[])listeners);
					break;
				case ListenerType.Merge:
					eventListeners.MergeEventListeners = AppendListeners(eventListeners.MergeEventListeners, (IMergeEventListener[])listeners);
					break;
				case ListenerType.Create:
					eventListeners.PersistEventListeners = AppendListeners(eventListeners.PersistEventListeners, (IPersistEventListener[])listeners);
					break;
				case ListenerType.CreateOnFlush:
					eventListeners.PersistOnFlushEventListeners = AppendListeners(eventListeners.PersistOnFlushEventListeners, (IPersistEventListener[])listeners);
					break;
				case ListenerType.Delete:
					eventListeners.DeleteEventListeners = AppendListeners(eventListeners.DeleteEventListeners, (IDeleteEventListener[])listeners);
					break;
				case ListenerType.DirtyCheck:
					eventListeners.DirtyCheckEventListeners = AppendListeners(eventListeners.DirtyCheckEventListeners, (IDirtyCheckEventListener[])listeners);
					break;
				case ListenerType.Evict:
					eventListeners.EvictEventListeners = AppendListeners(eventListeners.EvictEventListeners, (IEvictEventListener[])listeners);
					break;
				case ListenerType.Flush:
					eventListeners.FlushEventListeners = AppendListeners(eventListeners.FlushEventListeners, (IFlushEventListener[])listeners);
					break;
				case ListenerType.FlushEntity:
					eventListeners.FlushEntityEventListeners = AppendListeners(eventListeners.FlushEntityEventListeners, (IFlushEntityEventListener[])listeners);
					break;
				case ListenerType.Load:
					eventListeners.LoadEventListeners = AppendListeners(eventListeners.LoadEventListeners, (ILoadEventListener[])listeners);
					break;
				case ListenerType.LoadCollection:
					eventListeners.InitializeCollectionEventListeners = AppendListeners(eventListeners.InitializeCollectionEventListeners, (IInitializeCollectionEventListener[])listeners);
					break;
				case ListenerType.Lock:
					eventListeners.LockEventListeners = AppendListeners(eventListeners.LockEventListeners, (ILockEventListener[])listeners);
					break;
				case ListenerType.Refresh:
					eventListeners.RefreshEventListeners = AppendListeners(eventListeners.RefreshEventListeners, (IRefreshEventListener[])listeners);
					break;
				case ListenerType.Replicate:
					eventListeners.ReplicateEventListeners = AppendListeners(eventListeners.ReplicateEventListeners, (IReplicateEventListener[])listeners);
					break;
				case ListenerType.SaveUpdate:
					eventListeners.SaveOrUpdateEventListeners = AppendListeners(eventListeners.SaveOrUpdateEventListeners, (ISaveOrUpdateEventListener[])listeners);
					break;
				case ListenerType.Save:
					eventListeners.SaveEventListeners = AppendListeners(eventListeners.SaveEventListeners, (ISaveOrUpdateEventListener[])listeners);
					break;
				case ListenerType.PreUpdate:
					eventListeners.PreUpdateEventListeners = AppendListeners(eventListeners.PreUpdateEventListeners, (IPreUpdateEventListener[])listeners);
					break;
				case ListenerType.Update:
					eventListeners.UpdateEventListeners = AppendListeners(eventListeners.UpdateEventListeners, (ISaveOrUpdateEventListener[])listeners);
					break;
				case ListenerType.PreLoad:
					eventListeners.PreLoadEventListeners = AppendListeners(eventListeners.PreLoadEventListeners, (IPreLoadEventListener[])listeners);
					break;
				case ListenerType.PreDelete:
					eventListeners.PreDeleteEventListeners = AppendListeners(eventListeners.PreDeleteEventListeners, (IPreDeleteEventListener[])listeners);
					break;
				case ListenerType.PreInsert:
					eventListeners.PreInsertEventListeners = AppendListeners(eventListeners.PreInsertEventListeners, (IPreInsertEventListener[])listeners);
					break;
				case ListenerType.PostLoad:
					eventListeners.PostLoadEventListeners = AppendListeners(eventListeners.PostLoadEventListeners, (IPostLoadEventListener[])listeners);
					break;
				case ListenerType.PostInsert:
					eventListeners.PostInsertEventListeners = AppendListeners(eventListeners.PostInsertEventListeners, (IPostInsertEventListener[])listeners);
					break;
				case ListenerType.PostUpdate:
					eventListeners.PostUpdateEventListeners = AppendListeners(eventListeners.PostUpdateEventListeners, (IPostUpdateEventListener[])listeners);
					break;
				case ListenerType.PostDelete:
					eventListeners.PostDeleteEventListeners = AppendListeners(eventListeners.PostDeleteEventListeners, (IPostDeleteEventListener[])listeners);
					break;
				case ListenerType.PostCommitUpdate:
					eventListeners.PostCommitUpdateEventListeners = AppendListeners(eventListeners.PostCommitUpdateEventListeners, (IPostUpdateEventListener[])listeners);
					break;
				case ListenerType.PostCommitInsert:
					eventListeners.PostCommitInsertEventListeners = AppendListeners(eventListeners.PostCommitInsertEventListeners, (IPostInsertEventListener[])listeners);
					break;
				case ListenerType.PostCommitDelete:
					eventListeners.PostCommitDeleteEventListeners = AppendListeners(eventListeners.PostCommitDeleteEventListeners, (IPostDeleteEventListener[])listeners);
					break;
				case ListenerType.PreCollectionRecreate:
					eventListeners.PreCollectionRecreateEventListeners = AppendListeners(eventListeners.PreCollectionRecreateEventListeners, (IPreCollectionRecreateEventListener[])listeners);
					break;
				case ListenerType.PreCollectionRemove:
					eventListeners.PreCollectionRemoveEventListeners = AppendListeners(eventListeners.PreCollectionRemoveEventListeners, (IPreCollectionRemoveEventListener[])listeners);
					break;
				case ListenerType.PreCollectionUpdate:
					eventListeners.PreCollectionUpdateEventListeners = AppendListeners(eventListeners.PreCollectionUpdateEventListeners, (IPreCollectionUpdateEventListener[])listeners);
					break;
				case ListenerType.PostCollectionRecreate:
					eventListeners.PostCollectionRecreateEventListeners = AppendListeners(eventListeners.PostCollectionRecreateEventListeners, (IPostCollectionRecreateEventListener[])listeners);
					break;
				case ListenerType.PostCollectionRemove:
					eventListeners.PostCollectionRemoveEventListeners = AppendListeners(eventListeners.PostCollectionRemoveEventListeners, (IPostCollectionRemoveEventListener[])listeners);
					break;
				case ListenerType.PostCollectionUpdate:
					eventListeners.PostCollectionUpdateEventListeners = AppendListeners(eventListeners.PostCollectionUpdateEventListeners, (IPostCollectionUpdateEventListener[])listeners);
					break;
				default:
					log.Warn("Unrecognized listener type [" + type + "]");
					break;
			}
		}

		private static T[] AppendListeners<T>(T[] existing, T[] listenersToAdd)
		{
			var list = new List<T>(existing ?? new T[0]);
			list.AddRange(listenersToAdd);
			return list.ToArray();
		}

		///<summary>
		/// Generate DDL for altering tables
		///</summary>
		/// <seealso cref="NHibernate.Tool.hbm2ddl.SchemaUpdate"/>
		public string[] GenerateSchemaUpdateScript(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata)
		{
			SecondPassCompile();

			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			var script = new List<string>(50);
			foreach (var table in TableMappings)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Update))
				{
					ITableMetadata tableInfo = databaseMetadata.GetTableMetadata(table.Name, table.Schema ?? defaultSchema,
																				 table.Catalog ?? defaultCatalog, table.IsQuoted);
					if (tableInfo == null)
					{
						script.Add(table.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
					}
					else
					{
						string[] alterDDL = table.SqlAlterStrings(dialect, mapping, tableInfo, defaultCatalog, defaultSchema);
						script.AddRange(alterDDL);
					}

					string[] comments = table.SqlCommentStrings(dialect, defaultCatalog, defaultSchema);
					script.AddRange(comments);
				}
			}

			foreach (var table in TableMappings)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Update))
				{
					ITableMetadata tableInfo = databaseMetadata.GetTableMetadata(table.Name, table.Schema, table.Catalog,
																				 table.IsQuoted);

					if (dialect.SupportsForeignKeyConstraintInAlterTable)
					{
						foreach (var fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint && IncludeAction(fk.ReferencedTable.SchemaActions, SchemaAction.Update))
							{
								bool create = tableInfo == null
											  ||
											  (tableInfo.GetForeignKeyMetadata(fk.Name) == null
											   && (!(dialect is MySQLDialect) || tableInfo.GetIndexMetadata(fk.Name) == null));
								if (create)
								{
									script.Add(fk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
								}
							}
						}
					}

					foreach (var index in table.IndexIterator)
					{
						if (tableInfo == null || tableInfo.GetIndexMetadata(index.Name) == null)
						{
							script.Add(index.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
						}
					}
				}
			}

			foreach (var generator in IterateGenerators(dialect))
			{
				string key = generator.GeneratorKey();
				if (!databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key))
				{
					string[] lines = generator.SqlCreateStrings(dialect);
					for (int i = 0; i < lines.Length; i++)
					{
						script.Add(lines[i]);
					}
				}
			}

			return script.ToArray();
		}

		public void ValidateSchema(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata)
		{
			SecondPassCompile();

			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			var iter = TableMappings;
			foreach (var table in iter)
			{
				if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Validate))
				{
					/*NH Different Implementation :
						TableMetadata tableInfo = databaseMetadata.getTableMetadata(
						table.getName(),
						( table.getSchema() == null ) ? defaultSchema : table.getSchema(),
						( table.getCatalog() == null ) ? defaultCatalog : table.getCatalog(),
								table.isQuoted());*/
					ITableMetadata tableInfo = databaseMetadata.GetTableMetadata(
						table.Name,
						table.Schema ?? defaultSchema,
						table.Catalog ?? defaultCatalog,
						table.IsQuoted);
					if (tableInfo == null)
						throw new HibernateException("Missing table: " + table.Name);
					else
						table.ValidateColumns(dialect, mapping, tableInfo);
				}
			}

			var persistenceIdentifierGenerators = IterateGenerators(dialect);
			foreach (var generator in persistenceIdentifierGenerators)
			{
				string key = generator.GeneratorKey();
				if (!databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key))
				{
					throw new HibernateException(string.Format("Missing sequence or table: " + key));
				}
			}
		}

		private IEnumerable<IPersistentIdentifierGenerator> IterateGenerators(Dialect.Dialect dialect)
		{
			var generators = new Dictionary<string, IPersistentIdentifierGenerator>();
			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			foreach (var pc in classes.Values)
			{
				if (!pc.IsInherited)
				{
					var ig =
						pc.Identifier.CreateIdentifierGenerator(dialect, defaultCatalog, defaultSchema, (RootClass)pc) as
						IPersistentIdentifierGenerator;

					if (ig != null)
					{
						generators[ig.GeneratorKey()] = ig;
					}
				}
			}

			foreach (var collection in collections.Values)
			{
				if (collection.IsIdentified)
				{
					var ig =
						((IdentifierCollection)collection).Identifier.CreateIdentifierGenerator(dialect, defaultCatalog, defaultSchema,
																								 null) as IPersistentIdentifierGenerator;

					if (ig != null)
					{
						generators[ig.GeneratorKey()] = ig;
					}
				}
			}

			return generators.Values;
		}


	}
}
