using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Iesi.Collections;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Bytecode;
using NHibernate.Cfg.ConfigurationSchema;
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
	public class Configuration
	{
		/// <summary>The XML Namespace for the nhibernate-mapping</summary>
		public const string MappingSchemaXMLNS = "urn:nhibernate-mapping-2.2";

		/// <summary>Default name for hibernate configuration file.</summary>
		public const string DefaultHibernateCfgFileName = "hibernate.cfg.xml";

		private string currentDocumentName;

		protected IDictionary<string, PersistentClass> classes; // entityName, PersistentClass
		protected IDictionary<string, string> imports;
		protected IDictionary<string, NHibernate.Mapping.Collection> collections;
		protected IDictionary<string, Table> tables;
		protected IDictionary<string, NamedQueryDefinition> namedQueries;
		protected IDictionary<string, NamedSQLQueryDefinition> namedSqlQueries;
		protected IDictionary<string, ResultSetMappingDefinition> sqlResultSetMappings;
		protected IList<SecondPassCommand> secondPasses;
		protected IList<Mappings.PropertyReference> propertyReferences;
		private IInterceptor interceptor;
		private IDictionary<string, string> properties;
		protected IDictionary<string, FilterDefinition> filterDefinitions;
		protected IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects;
		protected IDictionary<string, ISQLFunction> sqlFunctions;

		private INamingStrategy namingStrategy = DefaultNamingStrategy.Instance;
		private MappingsQueue mappingsQueue;

		private EventListeners eventListeners;
		protected IDictionary<string, TypeDef> typeDefs;
		protected ISet<ExtendsQueueEntry> extendsQueue;
		protected IDictionary<string, Mappings.TableDescription> tableNameBinding;
		protected IDictionary<Table, Mappings.ColumnNames> columnNameBindingPerTable;

		private static readonly ILog log = LogManager.GetLogger(typeof(Configuration));

		protected internal SettingsFactory settingsFactory;

		/// <summary>
		/// Clear the internal state of the <see cref="Configuration"/> object.
		/// </summary>
		protected void Reset()
		{
			classes = new Dictionary<string, PersistentClass>(); //new SequencedHashMap(); - to make NH-369 bug deterministic
			imports = new Dictionary<string, string>();
			collections = new Dictionary<string, NHibernate.Mapping.Collection>();
			tables = new Dictionary<string, Table>();
			namedQueries = new Dictionary<string, NamedQueryDefinition>();
			namedSqlQueries = new Dictionary<string, NamedSQLQueryDefinition>();
			sqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>();
			secondPasses = new List<SecondPassCommand>();
			propertyReferences = new List<Mappings.PropertyReference>();
			filterDefinitions = new Dictionary<string, FilterDefinition>();
			interceptor = emptyInterceptor;
			properties = Environment.Properties;
			auxiliaryDatabaseObjects = new List<IAuxiliaryDatabaseObject>();
			sqlFunctions = new Dictionary<string, ISQLFunction>();
			mappingsQueue = new MappingsQueue();
			eventListeners = new EventListeners();
			typeDefs = new Dictionary<string, TypeDef>();
			extendsQueue = new HashedSet<ExtendsQueueEntry>();
			tableNameBinding = new Dictionary<string, Mappings.TableDescription>();
			columnNameBindingPerTable = new Dictionary<Table, Mappings.ColumnNames>();
		}

		private class Mapping : IMapping
		{
			private readonly Configuration configuration;

			public Mapping(Configuration configuration)
			{
				this.configuration = configuration;
			}

			private PersistentClass GetPersistentClass(string className)
			{
				PersistentClass pc = configuration.classes[className];
				if (pc == null)
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
		}

		[NonSerialized]
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
		public Configuration()
			: this(new SettingsFactory())
		{
		}

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
			return collections.ContainsKey(role) ? collections[role] : null;
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
				log.Error(exception.Message, exception);

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
				using (MemoryStream ms = new MemoryStream())
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
			try
			{
				// note that the prefix has absolutely nothing to do with what the user
				// selects as their prefix in the document.  It is the prefix we use to 
				// build the XPath and the nsmgr takes care of translating our prefix into
				// the user defined prefix...
				XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.Document.NameTable);
				namespaceManager.AddNamespace(HbmConstants.nsPrefix, MappingSchemaXMLNS);

				Dialect.Dialect dialect = Dialect.Dialect.GetDialect(properties);
				Mappings mappings = CreateMappings(dialect);

				new MappingRootBinder(mappings, namespaceManager, dialect).Bind(doc.Document.DocumentElement);
			}
			catch (Exception e)
			{
				string nameFormatted = doc.Name ?? "(unknown)";
				LogAndThrow(new MappingException("Could not compile the mapping document: " + nameFormatted, e));
			}
		}

		/// <summary>
		/// Create a new <see cref="Mappings" /> to add classes and collection
		/// mappings to.
		/// </summary>
		public Mappings CreateMappings(Dialect.Dialect dialect)
		{
			return new Mappings(classes,
				collections,
				tables,
				namedQueries,
				namedSqlQueries,
				sqlResultSetMappings,
				imports,
				secondPasses,
				propertyReferences,
				namingStrategy,
				typeDefs,
				filterDefinitions,
				extendsQueue,
				auxiliaryDatabaseObjects,
				tableNameBinding,
				columnNameBindingPerTable,
				defaultAssembly,
				defaultNamespace,
								dialect
				);
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
					rsrc.Close();
			}
		}

		// Not ported - addResource(String path) - not applicable

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

			foreach (string name in resourceNames)
			{
				AddResource(name, assembly);
			}
			return this;
		}

		private static IList<string> GetAllHbmXmlResourceNames(Assembly assembly)
		{
			List<string> result = new List<string>();

			foreach (string resource in assembly.GetManifestResourceNames())
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
			foreach (DirectoryInfo subDirectory in dir.GetDirectories())
			{
				AddDirectory(subDirectory);
			}

			foreach (FileInfo hbmXml in dir.GetFiles("*.hbm.xml"))
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

			List<string> script = new List<string>();

			// drop them in reverse order in case db needs it done that way...
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
				foreach (Table table in TableMappings)
				{
					if (table.IsPhysicalTable)
					{
						foreach (ForeignKey fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint)
								script.Add(fk.SqlDropString(dialect, defaultCatalog, defaultSchema));
						}
					}
				}
			}

			foreach (Table table in TableMappings)
			{
				if (table.IsPhysicalTable)
					script.Add(table.SqlDropString(dialect, defaultCatalog, defaultSchema));
			}

			IEnumerable<IPersistentIdentifierGenerator> pIDg = IterateGenerators(dialect);
			foreach (IPersistentIdentifierGenerator idGen in pIDg)
			{
				string[] lines = idGen.SqlDropString(dialect);
				if (lines != null)
				{
					foreach (string line in lines)
					{
						script.Add(line);
					}
				}
			}

			return script.ToArray();
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

			List<string> script = new List<string>();

			foreach (Table table in TableMappings)
			{
				if (table.IsPhysicalTable)
				{
					script.Add(table.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
					script.AddRange(table.SqlCommentStrings(dialect, defaultCatalog, defaultSchema));
				}
			}

			foreach (Table table in TableMappings)
			{
				if (table.IsPhysicalTable)
				{
					if (!dialect.SupportsUniqueConstraintInCreateAlterTable)
					{
						foreach (UniqueKey uk in table.UniqueKeyIterator)
						{
							string constraintString = uk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema);
							if (constraintString != null)
								script.Add(constraintString);
						}
					}

					foreach (Index index in table.IndexIterator)
						script.Add(index.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));

					if (dialect.HasAlterTable)
					{
						foreach (ForeignKey fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint)
								script.Add(fk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
						}
					}
				}
			}

			IEnumerable<IPersistentIdentifierGenerator> pIDg = IterateGenerators(dialect);
			foreach (IPersistentIdentifierGenerator idGen in pIDg)
				script.AddRange(idGen.SqlCreateStrings(dialect));

			foreach (IAuxiliaryDatabaseObject auxDbObj in auxiliaryDatabaseObjects)
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
			bool validateProxy = PropertiesHelper.GetBoolean(Environment.UseProxyValidator, properties, true);
			HashedSet allProxyErrors = null;

			foreach (PersistentClass clazz in classes.Values)
			{
				clazz.Validate(mapping);
				if (validateProxy)
				{
					ICollection errors = ValidateProxyInterface(clazz);
					if (errors != null)
					{
						if (allProxyErrors == null)
						{
							allProxyErrors = new HashedSet(errors);
						}
						else
						{
							allProxyErrors.AddAll(errors);
						}
					}
				}
			}

			if (allProxyErrors != null)
			{
				throw new InvalidProxyTypeException(allProxyErrors);
			}

			foreach (NHibernate.Mapping.Collection col in collections.Values)
			{
				col.Validate(mapping);
			}
		}

		private static ICollection ValidateProxyInterface(PersistentClass persistentClass)
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

			return ProxyTypeValidator.ValidateType(persistentClass.ProxyInterface);
		}

		/// <remarks>
		/// This method may be called many times!!
		/// </remarks>
		private void SecondPassCompile()
		{
			log.Info("checking mappings queue");

			mappingsQueue.CheckNoUnavailableEntries();

			log.Info("processing one-to-many association mappings");

			foreach (SecondPassCommand command in secondPasses)
			{
				command(classes);
			}

			secondPasses.Clear();

			log.Info("processing one-to-one association property references");

			foreach (Mappings.PropertyReference upr in propertyReferences)
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

			ISet done = new HashedSet();
			foreach (Table table in TableMappings)
			{
				SecondPassCompileForeignKeys(table, done);
			}
		}

		private void SecondPassCompileForeignKeys(Table table, ISet done)
		{
			table.CreateForeignKeys();

			foreach (ForeignKey fk in table.ForeignKeyIterator)
			{
				if (!done.Contains(fk))
				{
					done.Add(fk);

					string referencedEntityName = fk.ReferencedEntityName;
					if (string.IsNullOrEmpty(referencedEntityName))
					{
						throw new MappingException(string.Format("An association from the table {0} does not specify the referenced entity", fk.Table.Name));
					}

					if (log.IsDebugEnabled)
					{
						log.Debug("resolving reference to class: " + referencedEntityName);
					}

					PersistentClass referencedClass;
					if (!classes.TryGetValue(referencedEntityName, out referencedClass))
					{
						string message = string.Format("An association from the table {0} refers to an unmapped class: {1}",
							fk.Table.Name, referencedEntityName);

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
							fk.ReferencedTable = referencedClass.Table;
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
		public IDictionary<string, NamedQueryDefinition> NamedQueries
		{
			get { return namedQueries; }
		}

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
		public IEntityNotFoundDelegate EntityNotFoundDelegate
		{
			get { return entityNotFoundDelegate; }
			set { entityNotFoundDelegate = value; }
		}

		public EventListeners EventListeners
		{
			get { return eventListeners; }
		}

		private static readonly IInterceptor emptyInterceptor = new EmptyInterceptor();
		private string defaultAssembly;
		private string defaultNamespace;

		/// <summary>
		/// Instantiate a new <see cref="ISessionFactory" />, using the properties and mappings in this
		/// configuration. The <see cref="ISessionFactory" /> will be immutable, so changes made to the
		/// configuration after building the <see cref="ISessionFactory" /> will not affect it.
		/// </summary>
		/// <returns>An <see cref="ISessionFactory" /> instance.</returns>
		public ISessionFactory BuildSessionFactory()
		{
			#region Way for the user to specify their own ProxyFactory
			//http://jira.nhibernate.org/browse/NH-975

			IInjectableProxyFactoryFactory ipff = Environment.BytecodeProvider as IInjectableProxyFactoryFactory;
			string pffClassName;
			properties.TryGetValue(Environment.ProxyFactoryFactoryClass, out pffClassName);
			if (ipff != null && !string.IsNullOrEmpty(pffClassName))
			{
				ipff.SetProxyFactoryFactory(pffClassName);
			}

			#endregion

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
			foreach (KeyValuePair<string, string> de in additionalProperties)
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

		private void AddProperties(IHibernateConfiguration hc)
		{
			foreach (KeyValuePair<string, string> kvp in hc.SessionFactory.Properties)
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
			IHibernateConfiguration hc = ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as IHibernateConfiguration;
			if (hc != null && hc.SessionFactory != null)
			{
				return DoConfigure(hc);
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
				throw new HibernateException("Could not configure NHibernate.",
					new ArgumentNullException("assembly"));

			if (resourceName == null)
				throw new HibernateException("Could not configure NHibernate.",
					new ArgumentNullException("resourceName"));

			Stream stream = null;
			try
			{
				stream = assembly.GetManifestResourceStream(resourceName);
				if (stream == null)
				{
					// resource does not exist - throw appropriate exception 
					throw new HibernateException("A ManifestResourceStream could not be created for the resource " +
						resourceName +
							" in Assembly " + assembly.FullName);
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
				return DoConfigure(hc);
			}
			catch (Exception e)
			{
				log.Error("Problem parsing configuration", e);
				throw;
			}
		}

		// Not ported - configure(org.w3c.dom.Document)

		protected Configuration DoConfigure(IHibernateConfiguration hc)
		{
			if (!string.IsNullOrEmpty(hc.SessionFactory.Name))
			{
				properties[Environment.SessionFactoryName] = hc.SessionFactory.Name;
			}

			AddProperties(hc);

			// Load mappings
			foreach (MappingConfiguration mc in hc.SessionFactory.Mappings)
			{
				if (mc.IsEmpty())
					throw new HibernateConfigException("<mapping> element in configuration specifies no attributes");
				if (!string.IsNullOrEmpty(mc.Resource))
				{
					log.Debug(hc.SessionFactory.Name + "<-" + mc.Resource + " in " + mc.Assembly);
					AddResource(mc.Resource, Assembly.Load(mc.Assembly));
				}
				else if (!string.IsNullOrEmpty(mc.Assembly))
				{
					log.Debug(hc.SessionFactory.Name + "<-" + mc.Assembly);
					AddAssembly(mc.Assembly);
				}
				else if (!string.IsNullOrEmpty(mc.File))
				{
					log.Debug(hc.SessionFactory.Name + "<-" + mc.File);
					AddFile(mc.File);
				}
			}

			// Load class-cache
			foreach (ClassCacheConfiguration ccc in hc.SessionFactory.ClassesCache)
			{
				string region = string.IsNullOrEmpty(ccc.Region) ? ccc.Class : ccc.Region;
				bool includeLazy = (ccc.Include != ClassCacheInclude.NonLazy);
				SetCacheConcurrencyStrategy(ccc.Class, CfgXmlHelper.ClassCacheUsageConvertToString(ccc.Usage), region, includeLazy);
			}

			// Load collection-cache
			foreach (CollectionCacheConfiguration ccc in hc.SessionFactory.CollectionsCache)
			{
				string role = ccc.Collection;
				NHibernate.Mapping.Collection collection = GetCollectionMapping(role);
				if (collection == null)
				{
					throw new HibernateConfigException("collection-cache Configuration: Cannot configure cache for unknown collection role " + role);
				}

				string region = string.IsNullOrEmpty(ccc.Region) ? role : ccc.Region;
				SetCollectionCacheConcurrencyStrategy(role, CfgXmlHelper.ClassCacheUsageConvertToString(ccc.Usage), region);
			}

			// Events
			foreach (EventConfiguration ec in hc.SessionFactory.Events)
			{
				string[] listenerClasses = new string[ec.Listeners.Count];
				for (int i = 0; i < ec.Listeners.Count; i++)
				{
					listenerClasses[i] = ec.Listeners[i].Class;
				}
				log.Debug("Event listeners: " + ec.Type + "=" + StringHelper.ToString(listenerClasses));
				SetListeners(ec.Type, listenerClasses);
			}
			// Listeners
			foreach (ListenerConfiguration lc in hc.SessionFactory.Listeners)
			{
				log.Debug("Event listener: " + lc.Type + "=" + lc.Class);
				SetListeners(lc.Type, new string[] { lc.Class });
			}

			if (!string.IsNullOrEmpty(hc.SessionFactory.Name))
			{
				log.Info("Configured SessionFactory: " + hc.SessionFactory.Name);
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
						"class-cache Configuration: You may only specify a cache for root <class> mappings "
								+ "(cache was specified for " + clazz + ")");
			}
		}

		internal RootClass GetRootClassMapping(System.Type clazz)
		{
			PersistentClass persistentClass = GetClassMapping(clazz);

			if (persistentClass == null)
			{
				throw new HibernateConfigException("class-cache Configuration: Cache specified for unmapped class " + clazz);
			}

			RootClass rootClass = persistentClass as RootClass;

			if (rootClass == null)
			{
				throw new HibernateConfigException(
					"class-cache Configuration: You may only specify a cache for root <class> mappings "
						+ "(cache was specified for " + clazz + ")");
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
		/// <returns></returns>
		public IDictionary<string, string> Imports
		{
			get { return imports; }
		}

		/// <summary>
		/// Create an object-oriented view of the configuration properties
		/// </summary>
		/// <returns>A <see cref="Settings"/> object initialized from the settings properties.</returns>
		//protected Settings BuildSettings()
		private Settings BuildSettings()
		{
			return settingsFactory.BuildSettings(properties);
		}

		/// <summary>
		/// The named SQL queries
		/// </summary>
		public IDictionary<string, NamedSQLQueryDefinition> NamedSQLQueries
		{
			get { return namedSqlQueries; }
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
			get { return sqlResultSetMappings; }
		}

		public IDictionary<string, FilterDefinition> FilterDefinitions
		{
			get { return filterDefinitions; }
		}

		public void AddFilterDefinition(FilterDefinition definition)
		{
			filterDefinitions.Add(definition.FilterName, definition);
		}

		public void AddAuxiliaryDatabaseObject(IAuxiliaryDatabaseObject obj)
		{
			auxiliaryDatabaseObjects.Add(obj);
		}

		public IDictionary<string, ISQLFunction> SqlFunctions
		{
			get { return sqlFunctions; }
		}

		public void AddSqlFunction(string functionName, ISQLFunction sqlFunction)
		{
			sqlFunctions[functionName] = sqlFunction;
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
					XmlDocument hbmDocument = new XmlDocument();
					hbmDocument.Load(reader);
					return new NamedXmlDocument(name, hbmDocument);
				}
				finally
				{
					currentDocumentName = null;
				}
			}
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
			string message =
				string.Format(
					"{0}({1},{2}): XML validation error: {3}",
					currentDocumentName,
					args.Exception.LineNumber,
					args.Exception.LinePosition,
					args.Exception.Message);
			LogAndThrow(new MappingException(message, args.Exception));
		}

		private static string GetDefaultConfigurationFilePath()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
			return Path.Combine(binPath, DefaultHibernateCfgFileName);
		}

		#endregion

		private XmlSchemas schemas;
		private IEntityNotFoundDelegate entityNotFoundDelegate;

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
				object[] listeners =
					(object[])System.Array.CreateInstance(eventListeners.GetListenerClassFor(type), listenerClasses.Length);
				for (int i = 0; i < listeners.Length; i++)
				{
					try
					{
						listeners[i] = Activator.CreateInstance(ReflectHelper.ClassForName(listenerClasses[i]));
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
				object[] listeners = (object[])System.Array.CreateInstance(eventListeners.GetListenerClassFor(type), 1);
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
				ClearListeners(type);
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

		///<summary>
		/// Generate DDL for altering tables
		///</summary>
		/// <seealso cref="NHibernate.Tool.hbm2ddl.SchemaUpdate"/>
		public string[] GenerateSchemaUpdateScript(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata)
		{
			SecondPassCompile();

			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			List<string> script = new List<string>(50);
			foreach (Table table in TableMappings)
			{
				if (table.IsPhysicalTable)
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

			foreach (Table table in TableMappings)
			{
				if (table.IsPhysicalTable)
				{
					ITableMetadata tableInfo =
						databaseMetadata.GetTableMetadata(table.Name, table.Schema, table.Catalog, table.IsQuoted);

					if (dialect.HasAlterTable)
					{
						foreach (ForeignKey fk in table.ForeignKeyIterator)
						{
							if (fk.HasPhysicalConstraint)
							{
								bool create = tableInfo == null || (
										tableInfo.GetForeignKeyMetadata(fk.Name) == null &&
										(!(dialect is MySQLDialect) || tableInfo.GetIndexMetadata(fk.Name) == null)
									);
								if (create)
								{
									script.Add(fk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
								}
							}
						}
					}

				}
			}

			foreach (IPersistentIdentifierGenerator generator in IterateGenerators(dialect))
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

		private IEnumerable<IPersistentIdentifierGenerator> IterateGenerators(Dialect.Dialect dialect)
		{
			Dictionary<string, IPersistentIdentifierGenerator> generators =
				new Dictionary<string, IPersistentIdentifierGenerator>();
			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			foreach (PersistentClass pc in classes.Values)
			{
				if (!pc.IsInherited)
				{
					IPersistentIdentifierGenerator ig =
						pc.Identifier.CreateIdentifierGenerator(dialect, defaultCatalog, defaultSchema, (RootClass)pc) as
						IPersistentIdentifierGenerator;

					if (ig != null)
						generators[ig.GeneratorKey()] = ig;
				}
			}

			foreach (NHibernate.Mapping.Collection collection in collections.Values)
			{
				if (collection.IsIdentified)
				{
					IPersistentIdentifierGenerator ig =
						((IdentifierCollection)collection).Identifier.CreateIdentifierGenerator(dialect, defaultCatalog, defaultSchema,
																																										 null) as IPersistentIdentifierGenerator;

					if (ig != null)
						generators[ig.GeneratorKey()] = ig;
				}
			}

			return generators.Values;
		}
	}
}
