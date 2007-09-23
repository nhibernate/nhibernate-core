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
using log4net;
using NHibernate.Cfg.ConfigurationSchema;
using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Proxy;
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

		private IDictionary<System.Type, PersistentClass> classes;
		private IDictionary<string, string> imports;
		private IDictionary<string, NHibernate.Mapping.Collection> collections;
		private IDictionary<string, Table> tables;
		private IDictionary<string, NamedQueryDefinition> namedQueries;
		private IDictionary<string, NamedSQLQueryDefinition> namedSqlQueries;
		private IDictionary<string, ResultSetMappingDefinition> sqlResultSetMappings;
		private IList<SecondPassCommand> secondPasses;
		private IList<Mappings.UniquePropertyReference> propertyReferences;
		private IInterceptor interceptor;
		private IDictionary properties;
		private IDictionary filterDefinitions;
		private IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects;
		private IDictionary<string, ISQLFunction> sqlFunctions;

		private INamingStrategy namingStrategy = DefaultNamingStrategy.Instance;
		private System.Type proxyFactoryClass = typeof(CastleProxyFactory);
		private MappingsQueue mappingsQueue;

		private EventListeners eventListeners;

		private static readonly ILog log = LogManager.GetLogger(typeof(Configuration));

		/// <summary>
		/// Clear the internal state of the <see cref="Configuration"/> object.
		/// </summary>
		private void Reset()
		{
			classes = new Dictionary<System.Type, PersistentClass>(); //new SequencedHashMap(); - to make NH-369 bug deterministic
			imports = new Dictionary<string, string>();
			collections = new Dictionary<string, NHibernate.Mapping.Collection>();
			tables = new Dictionary<string, Table>();
			namedQueries = new Dictionary<string, NamedQueryDefinition>();
			namedSqlQueries = new Dictionary<string, NamedSQLQueryDefinition>();
			sqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>();
			secondPasses = new List<SecondPassCommand>();
			propertyReferences = new List<Mappings.UniquePropertyReference>();
			filterDefinitions = new Hashtable();
			interceptor = emptyInterceptor;
			mapping = new Mapping(this);
			properties = Environment.Properties;
			auxiliaryDatabaseObjects = new List<IAuxiliaryDatabaseObject>();
			sqlFunctions = new Dictionary<string, ISQLFunction>();
			mappingsQueue = new MappingsQueue();
			eventListeners = new EventListeners();
		}

		private class Mapping : IMapping
		{
			private readonly Configuration configuration;

			public Mapping(Configuration configuration)
			{
				this.configuration = configuration;
			}

			private PersistentClass GetPersistentClass(System.Type type)
			{
				PersistentClass pc = configuration.classes[type];
				if (pc == null)
				{
					throw new MappingException("persistent class not known: " + type.FullName);
				}
				return pc;
			}

			public IType GetIdentifierType(System.Type persistentClass)
			{
				return GetPersistentClass(persistentClass).Identifier.Type;
			}

			public string GetIdentifierPropertyName(System.Type persistentClass)
			{
				PersistentClass pc = GetPersistentClass(persistentClass);
				if (!pc.HasIdentifierProperty)
				{
					return null;
				}
				return pc.IdentifierProperty.Name;
			}

			public IType GetPropertyType(System.Type persistentClass, string propertyName)
			{
				PersistentClass pc = GetPersistentClass(persistentClass);
				NHibernate.Mapping.Property prop = pc.GetProperty(propertyName);

				if (prop == null)
				{
					throw new MappingException("property not known: " + persistentClass.FullName + '.' + propertyName);
				}
				return prop.Type;
			}
		}

		private Mapping mapping;

		/// <summary>
		/// Create a new Configuration object.
		/// </summary>
		public Configuration()
		{
			Reset();
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
			if (classes.ContainsKey(persistentClass))
				return classes[persistentClass];
			else
				return null;
		}

		/// <summary>
		/// Get the mapping for a particular collection role
		/// </summary>
		/// <param name="role">a collection role</param>
		/// <returns><see cref="NHibernate.Mapping.Collection" /></returns>
		public NHibernate.Mapping.Collection GetCollectionMapping(string role)
		{
			if (collections.ContainsKey(role))
				return collections[role];
			else
				return null;
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
				Mappings mappings = CreateMappings();

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
		public Mappings CreateMappings()
		{
			return new Mappings(
				classes,
				collections,
				tables,
				namedQueries,
				namedSqlQueries,
				sqlResultSetMappings,
				imports,
				secondPasses,
				propertyReferences,
				namingStrategy,
				filterDefinitions,
				auxiliaryDatabaseObjects,
				defaultAssembly,
				defaultNamespace
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

		private ICollection CollectionGenerators(Dialect.Dialect dialect)
		{
			Hashtable generators = new Hashtable();
			foreach (PersistentClass clazz in classes.Values)
			{
				IIdentifierGenerator ig = clazz.Identifier.CreateIdentifierGenerator(dialect);
				if (ig is IPersistentIdentifierGenerator)
				{
					generators[((IPersistentIdentifierGenerator)ig).GeneratorKey()] = ig;
				}
			}

			foreach (NHibernate.Mapping.Collection collection in collections.Values)
			{
				if (collection is IdentifierCollection)
				{
					IIdentifierGenerator ig = ((IdentifierCollection)collection)
						.Identifier
						.CreateIdentifierGenerator(dialect);

					if (ig is IPersistentIdentifierGenerator)
					{
						generators[((IPersistentIdentifierGenerator)ig).GeneratorKey()] = ig;
					}
				}
			}
			return generators.Values;
		}

		/// <summary>
		/// Generate DDL for droping tables
		/// </summary>
		/// <seealso cref="NHibernate.Tool.hbm2ddl.SchemaExport" />
		public string[] GenerateDropSchemaScript(Dialect.Dialect dialect)
		{
			SecondPassCompile();

			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			ArrayList script = new ArrayList(50);

			// drop them in reverse order in case db needs it done that way...
			for (int i = auxiliaryDatabaseObjects.Count - 1; i >= 0; i--)
			{
				IAuxiliaryDatabaseObject auxDbObj = auxiliaryDatabaseObjects[i];
				if (auxDbObj.AppliesToDialect(dialect))
				{
					script.Add(auxDbObj.SqlDropString(dialect, defaultSchema));
				}
			}

			if (dialect.DropConstraints)
			{
				foreach (Table table in TableMappings)
				{
					foreach (ForeignKey fk in table.ForeignKeyCollection)
					{
						script.Add(fk.SqlDropString(dialect, defaultSchema));
					}
				}
			}

			foreach (Table table in TableMappings)
			{
				script.Add(table.SqlDropString(dialect, defaultSchema));
			}

			foreach (IPersistentIdentifierGenerator idGen in CollectionGenerators(dialect))
			{
				string dropString = idGen.SqlDropString(dialect);
				if (dropString != null)
				{
					script.Add(dropString);
				}
			}

			return ArrayHelper.ToStringArray(script);
		}

		/// <summary>
		/// Generate DDL for creating tables
		/// </summary>
		/// <param name="dialect"></param>
		public string[] GenerateSchemaCreationScript(Dialect.Dialect dialect)
		{
			SecondPassCompile();

			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

			ArrayList script = new ArrayList(50);

			foreach (Table table in TableMappings)
			{
				script.Add(table.SqlCreateString(dialect, mapping, defaultSchema));
			}

			foreach (Table table in TableMappings)
			{
				/*
				TODO: H2.1.8 has the code below, but only TimesTen dialect ever
				enters the if, so I don't want to add this to Dialect right now

				if( !dialect.SupportsUniqueConstraintInCreateAlterTable )
				{
					foreach( UniqueKey uk in table.UniqueKeyCollection )
					{
						script.Add( uk.SqlCreateString( dialect, mapping, defaultSchema ) );
					}
				}
				*/

				foreach (Index index in table.IndexCollection)
				{
					script.Add(index.SqlCreateString(dialect, mapping, defaultSchema));
				}

				if (dialect.HasAlterTable)
				{
					foreach (ForeignKey fk in table.ForeignKeyCollection)
					{
						script.Add(fk.SqlCreateString(dialect, mapping, defaultSchema));
					}
				}
			}

			foreach (IPersistentIdentifierGenerator idGen in CollectionGenerators(dialect))
			{
				string[] lines = idGen.SqlCreateStrings(dialect);
				script.AddRange(lines);
			}

			foreach (IAuxiliaryDatabaseObject auxDbObj in auxiliaryDatabaseObjects)
			{
				if (auxDbObj.AppliesToDialect(dialect))
				{
					script.Add(auxDbObj.SqlCreateString(dialect, mapping, defaultSchema));
				}
			}

			return ArrayHelper.ToStringArray(script);
		}

		/*
		///<summary>
		/// Generate DDL for altering tables
		///</summary>
		public string[] GenerateSchemaUpdateScript(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata) 
		{
			secondPassCompile();
				
			ArrayList script = new ArrayList(50);
		
			foreach(Table table in TableMappings)
			{
				TableMetadata tableInfo = databaseMetadata.GetTableMetadata( table.Name, table.Schema, null );
				if (tableInfo == null) 
				{
					script.Add( table.SqlCreateString( dialect, this ) );
				}
				else 
				{
					foreach(string alterString in table.SqlAlterStrings(dialect, this, tableInfo))
					{
						script.Add( alterString );
					}
				}
			}
				
			foreach(Table table in TableMappings)
			{
				TableMetadata tableInfo = databaseMetadata.GetTableMetadata( table.Name, table.Schema, null );
					
				if ( dialect.HasAlterTable)
				{
					foreach(ForeignKey fk in table.ForeignKeyCollection)
					{
						bool create = tableInfo == null || ( tableInfo.getForeignKeyMetadata( fk.Name ) == null && (
							// Icky workaround for MySQL bug:
							!( dialect is NHibernate.Dialect.MySQLDialect ) || table.GetIndex( fk.Name ) == null )
						);
						if ( create ) 
						{
							script.Add( fk.SqlCreateString(dialect, mapping) );
						}
					}
				}

				// Broken 'cos we don't generate these with names in SchemaExport
				foreach(Index index in table.IndexCollection)
				{
					if ( tableInfo == null || tableInfo.GetIndexMetadata( index.Name ) == null ) 
					{
						script.Add( index.SqlCreateString( dialect, mapping ) );
					}
				}

				// Broken 'cos we don't generate these with names in SchemaExport
				foreach(UniqueKey uk in table.UniqueKeyCollection)
				{
					if ( tableInfo == null || tableInfo.GetIndexMetadata( uk.Name ) == null ) 
					{
						script.Add( uk.SqlCreateString( dialect, mapping ) );
					}
				}
			}
		
			foreach(IPersistentIdentifierGenerator generator in CollectionGenerators(dialect))
			{
				object key = generator.GeneratorKey();
				if ( !databaseMetadata.IsSequence( key ) && !databaseMetadata.IsTable(key) ) 
				{
					string[] lines = generator.SqlCreateStrings( dialect );
					for (int i = 0; i < lines.Length; i++) script.Add( lines[i] );
				}
			}
				
			return ArrayHelper.ToStringArray(script);
		}
		*/

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

			foreach (Mappings.UniquePropertyReference upr in propertyReferences)
			{
				PersistentClass clazz = GetClassMapping(upr.ReferencedClass);
				if (clazz == null)
				{
					throw new MappingException("property-ref to unmapped class: " + upr.ReferencedClass);
				}

				NHibernate.Mapping.Property prop = clazz.GetReferencedProperty(upr.PropertyName);
				((SimpleValue)prop.Value).IsUnique = true;
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
			foreach (ForeignKey fk in table.ForeignKeyCollection)
			{
				if (!done.Contains(fk))
				{
					done.Add(fk);

					if (log.IsDebugEnabled)
					{
						log.Debug("resolving reference to class: " + fk.ReferencedClass.Name);
					}

					if (!classes.ContainsKey(fk.ReferencedClass))
					{
						string messageTemplate = "An association from the table {0} refers to an unmapped class: {1}";
						string message = string.Format(messageTemplate, fk.Table.Name, fk.ReferencedClass.Name);

						LogAndThrow(new MappingException(message));
					}
					else
					{
						PersistentClass referencedClass = classes[fk.ReferencedClass];

						if (referencedClass.IsJoinedSubclass)
						{
							SecondPassCompileForeignKeys(referencedClass.Superclass.Table, done);
						}

						try
						{
							fk.ReferencedTable = referencedClass.Table;
						}
						catch (MappingException me)
						{
							if (log.IsErrorEnabled)
							{
								log.Error(me);
							}

							// rethrow the error - only caught it for logging purposes
							throw;
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
		public IDictionary Properties
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
		public Configuration SetProperties(IDictionary newProperties)
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
		public Configuration AddProperties(IDictionary additionalProperties)
		{
			foreach (DictionaryEntry de in additionalProperties)
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
			return properties[name] as string;
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
		/// Configure NHibernate using the specified XmlTextReader.
		/// </summary>
		/// <param name="textReader">The <see cref="XmlTextReader"/> that contains the Xml to configure NHibernate.</param>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure(XmlTextReader) will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure(XmlTextReader textReader)
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
				System.Type clazz;
				try
				{
					clazz = ReflectHelper.ClassForName(ccc.Class);
				}
				catch (TypeLoadException tle)
				{
					throw new HibernateConfigException("class-cache Configuration: Could not find class " + ccc.Class, tle);
				}

				string region = string.IsNullOrEmpty(ccc.Region) ? ccc.Class : ccc.Region;
				bool includeLazy = (ccc.Include != ClassCacheInclude.NonLazy);
				SetCacheConcurrencyStrategy(clazz, CfgXmlHelper.ClassCacheUsageConvertToString(ccc.Usage), region, includeLazy);
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
				SetCacheConcurrencyStrategy(role, CfgXmlHelper.ClassCacheUsageConvertToString(ccc.Usage), region);
			}

			// Events
			foreach (EventConfiguration ec in hc.SessionFactory.Events)
			{
				string[] listenerClasses = new string[ec.Listeners.Count];
				for(int i=0;i< ec.Listeners.Count;i++)
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
		public Configuration SetCacheConcurrencyStrategy(System.Type clazz, string concurrencyStrategy)
		{
			SetCacheConcurrencyStrategy(clazz, concurrencyStrategy, clazz.FullName);
			return this;
		}

		internal void SetCacheConcurrencyStrategy(System.Type clazz, string concurrencyStrategy, string region)
		{
			SetCacheConcurrencyStrategy(clazz, concurrencyStrategy, region, true);
		}

		internal void SetCacheConcurrencyStrategy(System.Type clazz, string concurrencyStrategy,
			string region, bool includeLazy)
		{
			RootClass rootClass = GetRootClassMapping(clazz);
			rootClass.CacheConcurrencyStrategy = concurrencyStrategy;
			rootClass.CacheRegionName = region;
			rootClass.SetLazyPropertiesCacheable(includeLazy);
		}

		/// <summary>
		/// Set up a cache for a collection role
		/// </summary>
		public Configuration SetCacheConcurrencyStrategy(string collectionRole, string concurrencyStrategy)
		{
			SetCacheConcurrencyStrategy(collectionRole, concurrencyStrategy, collectionRole);
			return this;
		}

		internal void SetCacheConcurrencyStrategy(string collectionRole, string concurrencyStrategy, string region)
		{
			NHibernate.Mapping.Collection collection = GetCollectionMapping(collectionRole);
			collection.CacheConcurrencyStrategy = concurrencyStrategy;
			collection.CacheRegionName = region;
		}

		/// <summary>
		/// Get the query language imports
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
			return SettingsFactory.BuildSettings(properties);
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

		public System.Type ProxyFactoryClass
		{
			get { return proxyFactoryClass; }
		}

		public Configuration SetProxyFactoryClass(System.Type newProxyFactoryClass)
		{
			if (typeof(IProxyFactory).IsAssignableFrom(newProxyFactoryClass) == false)
			{
				HibernateException he =
					new HibernateException(newProxyFactoryClass.FullName + " does not implement " +
						typeof(IProxyFactory).FullName);
				log.Error(he);
				throw he;
			}
			proxyFactoryClass = newProxyFactoryClass;
			return this;
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

		public IDictionary FilterDefinitions
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
		/// Load and validate the mappings in the <see cref="XmlTextReader" /> against
		/// the nhibernate-mapping-2.2 schema, without adding them to the configuration.
		/// </summary>
		/// <remarks>
		/// This method is made public to be usable from the unit tests. It is not intended
		/// to be called by end users.
		/// </remarks>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <param name="name">The name of the document, for error reporting purposes.</param>
		/// <returns>NamedXmlDocument containing the validated XmlDocument built from the XmlReader.</returns>
		public NamedXmlDocument LoadMappingDocument(XmlTextReader hbmReader, string name)
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
		/// Adds the Mappings in the <see cref="XmlTextReader"/> after validating it
		/// against the nhibernate-mapping-2.2 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlTextReader that contains the mapping.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader(XmlTextReader hbmReader)
		{
			return AddXmlReader(hbmReader, null);
		}

		/// <summary>
		/// Adds the Mappings in the <see cref="XmlTextReader"/> after validating it
		/// against the nhibernate-mapping-2.2 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlTextReader that contains the mapping.</param>
		/// <param name="name">The name of the document to use for error reporting. May be <see langword="null" />.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader(XmlTextReader hbmReader, string name)
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

		private XmlSchemas Schemas
		{
			get { return schemas = schemas ?? new XmlSchemas(); }
			set { schemas = value; }
		}

		public void SetListeners(ListenerType type, string[] listenerClasses)
		{
			object[] listeners = (object[])System.Array.CreateInstance(eventListeners.GetListenerClassFor(type), listenerClasses.Length);
			for (int i = 0; i < listeners.Length; i++)
			{
				try
				{
					listeners[i] = Activator.CreateInstance(ReflectHelper.ClassForName(listenerClasses[i]));
				}
				catch (Exception e)
				{
					throw new MappingException("Unable to instantiate specified event (" + type + ") listener class: " + listenerClasses[i], e);
				}
			}
			SetListeners(type, listeners);
		}

		public void SetListener(ListenerType type, object listener)
		{
			if (listener == null)
			{
				SetListener(type, null);
			}
			else
			{
				object[] listeners = (object[])System.Array.CreateInstance(eventListeners.GetListenerClassFor(type), 1);
				listeners[0] = listener;
				SetListeners(type, listeners);
			}
		}

		public void SetListeners(ListenerType type, object[] listeners)
		{
			switch (type)
			{
				case ListenerType.Autoflush:
					if (listeners == null)
					{
						eventListeners.AutoFlushEventListeners = new IAutoFlushEventListener[] {};
					}
					else
					{
						eventListeners.AutoFlushEventListeners = (IAutoFlushEventListener[]) listeners;
					}
					break;
				case ListenerType.Merge:
					if (listeners == null)
					{
						eventListeners.MergeEventListeners = new IMergeEventListener[] { };
					}
					else
					{
						eventListeners.MergeEventListeners = (IMergeEventListener[])listeners;
					} 
					break;
				case ListenerType.Create:
					if (listeners == null)
					{
						eventListeners.PersistEventListeners = new IPersistEventListener[] { };
					}
					else
					{
						eventListeners.PersistEventListeners = (IPersistEventListener[])listeners;
					}
					break;
				case ListenerType.CreateOnFlush:
					if (listeners == null)
					{
						eventListeners.PersistOnFlushEventListeners = new IPersistEventListener[] { };
					}
					else
					{
						eventListeners.PersistOnFlushEventListeners = (IPersistEventListener[])listeners;
					}
					break;
				case ListenerType.Delete:
					if (listeners == null)
					{
						eventListeners.DeleteEventListeners = new IDeleteEventListener[] { };
					}
					else
					{
						eventListeners.DeleteEventListeners = (IDeleteEventListener[])listeners;
					}
					break;
				case ListenerType.DirtyCheck:
					if (listeners == null)
					{
						eventListeners.DirtyCheckEventListeners = new IDirtyCheckEventListener[] { };
					}
					else
					{
						eventListeners.DirtyCheckEventListeners = (IDirtyCheckEventListener[])listeners;
					}
					break;
				case ListenerType.Evict:
					if (listeners == null)
					{
						eventListeners.EvictEventListeners = new IEvictEventListener[] { };
					}
					else
					{
						eventListeners.EvictEventListeners = (IEvictEventListener[])listeners;
					}
					break;
				case ListenerType.Flush:
					if (listeners == null)
					{
						eventListeners.FlushEventListeners = new IFlushEventListener[] { };
					}
					else
					{
						eventListeners.FlushEventListeners = (IFlushEventListener[])listeners;
					}
					break;
				case ListenerType.FlushEntity:
					if (listeners == null)
					{
						eventListeners.FlushEntityEventListeners = new IFlushEntityEventListener[] { };
					}
					else
					{
						eventListeners.FlushEntityEventListeners = (IFlushEntityEventListener[])listeners;
					}
					break;
				case ListenerType.Load:
					if (listeners == null)
					{
						eventListeners.LoadEventListeners = new ILoadEventListener[] { };
					}
					else
					{
						eventListeners.LoadEventListeners = (ILoadEventListener[])listeners;
					}
					break;
				case ListenerType.LoadCollection:
					if (listeners == null)
					{
						eventListeners.InitializeCollectionEventListeners = new IInitializeCollectionEventListener[] { };
					}
					else
					{
						eventListeners.InitializeCollectionEventListeners = (IInitializeCollectionEventListener[])listeners;
					}
					break;
				case ListenerType.Lock:
					if (listeners == null)
					{
						eventListeners.LockEventListeners = new ILockEventListener[] { };
					}
					else
					{
						eventListeners.LockEventListeners = (ILockEventListener[])listeners;
					}
					break;
				case ListenerType.Refresh:
					if (listeners == null)
					{
						eventListeners.RefreshEventListeners = new IRefreshEventListener[] { };
					}
					else
					{
						eventListeners.RefreshEventListeners = (IRefreshEventListener[])listeners;
					}
					break;
				case ListenerType.Replicate:
					if (listeners == null)
					{
						eventListeners.ReplicateEventListeners = new IReplicateEventListener[] { };
					}
					else
					{
						eventListeners.ReplicateEventListeners = (IReplicateEventListener[])listeners;
					}
					break;
				case ListenerType.SaveUpdate:
					if (listeners == null)
					{
						eventListeners.SaveOrUpdateEventListeners = new ISaveOrUpdateEventListener[] { };
					}
					else
					{
						eventListeners.SaveOrUpdateEventListeners = (ISaveOrUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.Save:
					if (listeners == null)
					{
						eventListeners.SaveEventListeners = new ISaveOrUpdateEventListener[] { };
					}
					else
					{
						eventListeners.SaveEventListeners = (ISaveOrUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.PreUpdate:
					if (listeners == null)
					{
						eventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[] { };
					}
					else
					{
						eventListeners.PreUpdateEventListeners = (IPreUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.Update:
					if (listeners == null)
					{
						eventListeners.UpdateEventListeners = new ISaveOrUpdateEventListener[] { };
					}
					else
					{
						eventListeners.UpdateEventListeners = (ISaveOrUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.PreLoad:
					if (listeners == null)
					{
						eventListeners.PreLoadEventListeners = new IPreLoadEventListener[] { };
					}
					else
					{
						eventListeners.PreLoadEventListeners = (IPreLoadEventListener[])listeners;
					}
					break;
				case ListenerType.PreDelete:
					if (listeners == null)
					{
						eventListeners.PreDeleteEventListeners = new IPreDeleteEventListener[] { };
					}
					else
					{
						eventListeners.PreDeleteEventListeners = (IPreDeleteEventListener[])listeners;
					}
					break;
				case ListenerType.PreInsert:
					if (listeners == null)
					{
						eventListeners.PreInsertEventListeners = new IPreInsertEventListener[] { };
					}
					else
					{
						eventListeners.PreInsertEventListeners = (IPreInsertEventListener[])listeners;
					}
					break;
				case ListenerType.PostLoad:
					if (listeners == null)
					{
						eventListeners.PostLoadEventListeners = new IPostLoadEventListener[] { };
					}
					else
					{
						eventListeners.PostLoadEventListeners = (IPostLoadEventListener[])listeners;
					}
					break;
				case ListenerType.PostInsert:
					if (listeners == null)
					{
						eventListeners.PostInsertEventListeners = new IPostInsertEventListener[] { };
					}
					else
					{
						eventListeners.PostInsertEventListeners = (IPostInsertEventListener[])listeners;
					}
					break;
				case ListenerType.PostUpdate:
					if (listeners == null)
					{
						eventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[] { };
					}
					else
					{
						eventListeners.PostUpdateEventListeners = (IPostUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.PostDelete:
					if (listeners == null)
					{
						eventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[] { };
					}
					else
					{
						eventListeners.PostDeleteEventListeners = (IPostDeleteEventListener[])listeners;
					}
					break;
				case ListenerType.PostCommitUpdate:
					if (listeners == null)
					{
						eventListeners.PostCommitUpdateEventListeners = new IPostUpdateEventListener[] { };
					}
					else
					{
						eventListeners.PostCommitUpdateEventListeners = (IPostUpdateEventListener[])listeners;
					}
					break;
				case ListenerType.PostCommitInsert:
					if (listeners == null)
					{
						eventListeners.PostCommitInsertEventListeners = new IPostInsertEventListener[] { };
					}
					else
					{
						eventListeners.PostCommitInsertEventListeners = (IPostInsertEventListener[])listeners;
					}
					break;
				case ListenerType.PostCommitDelete:
					if (listeners == null)
					{
						eventListeners.PostCommitDeleteEventListeners = new IPostDeleteEventListener[] { };
					}
					else
					{
						eventListeners.PostCommitDeleteEventListeners = (IPostDeleteEventListener[])listeners;
					}
					break;
				default:
					log.Warn("Unrecognized listener type [" + type + "]");
					break;
			}
		}
	}
}
