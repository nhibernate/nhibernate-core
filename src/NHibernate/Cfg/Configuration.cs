using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg
{
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
	public class Configuration : IMapping
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( Configuration ) );

		private Hashtable classes = new Hashtable();
		private Hashtable imports = new Hashtable();
		private Hashtable collections = new Hashtable();
		private Hashtable tables = new Hashtable();
		private Hashtable namedQueries = new Hashtable();
		private ArrayList secondPasses = new ArrayList();
		private IInterceptor interceptor = EmptyInterceptor;
		private IDictionary properties = Environment.Properties;
		private IDictionary caches = new Hashtable();

		private XmlSchema mappingSchema;
		private XmlSchema cfgSchema;

		/// <summary></summary>
		public static readonly string MappingSchemaXMLNS = "urn:nhibernate-mapping-2.0";
		private static readonly string MappingSchemaResource = "NHibernate.nhibernate-mapping-2.0.xsd";

		/// <summary></summary>
		public static readonly string CfgSchemaXMLNS = "urn:nhibernate-configuration-2.0";
		private static readonly string CfgSchemaResource = "NHibernate.nhibernate-configuration-2.0.xsd";
		private static readonly string CfgNamespacePrefix = "cfg";
		private static XmlNamespaceManager CfgNamespaceMgr;

		/// <summary></summary>
		protected void Reset()
		{
			classes = new Hashtable();
			collections = new Hashtable();
			tables = new Hashtable();
			namedQueries = new Hashtable();
			secondPasses = new ArrayList();
			interceptor = EmptyInterceptor;
			properties = Environment.Properties;
		}

		/// <summary>
		/// Create a new Configuration object.
		/// </summary>
		public Configuration()
		{
			Reset();
			mappingSchema = XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream( MappingSchemaResource ), null );
			cfgSchema = XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream( CfgSchemaResource ), null );
		}

		/// <summary>
		/// Returns the identifier type of a mapped class
		/// </summary>
		public IType GetIdentifierType( System.Type persistentClass )
		{
			return ( ( PersistentClass ) classes[ persistentClass ] ).Identifier.Type;
		}

		/// <summary>
		/// The class mappings 
		/// </summary>
		public ICollection ClassMappings
		{
			get { return classes.Values; }
		}

		/// <summary>
		/// The collection mappings
		/// </summary>
		public ICollection CollectionMappings
		{
			get { return collections.Values; }
		}

		/// <summary>
		/// The table mappings
		/// </summary>
		private ICollection TableMappings
		{
			get { return tables.Values; }
		}

		/// <summary>
		/// Get the mapping for a particular class
		/// </summary>
		public PersistentClass GetClassMapping( System.Type persistentClass )
		{
			return ( PersistentClass ) classes[ persistentClass ];
		}

		/// <summary>
		/// Get the mapping for a particular collection role
		/// </summary>
		/// <param name="role">role a collection role</param>
		/// <returns>collection</returns>
		public Mapping.Collection GetCollectionMapping( string role )
		{
			return ( Mapping.Collection ) collections[ role ];
		}

		/// <summary>
		/// Adds the Mappings contained in the file specified.
		/// </summary>
		/// <param name="xmlFile">The name of the file (url or file system) that contains the Xml.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlFile( string xmlFile )
		{
			log.Debug( "Mapping file: " + xmlFile );
			try
			{
				AddXmlReader( new XmlTextReader( xmlFile ) );
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from file: " + xmlFile, e );
				throw new MappingException( e );
			}
			return this;
		}

		/// <summary>
		/// Adds the Mappings from a <c>String</c>
		/// </summary>
		/// <param name="xml">A string that contains the Mappings for the Xml</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlString( string xml )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Mapping XML:\n" + xml );
			}
			try
			{
				// make a StringReader for the string passed in - the StringReader
				// inherits from TextReader.  We can use the XmlTextReader.ctor that
				// takes the TextReader to build from a string...
				AddXmlReader( new XmlTextReader( new StringReader( xml ) ) );
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from XML", e );
			}
			return this;
		}

		/// <summary>
		/// Adds the Mappings in the XML Document
		/// </summary>
		/// <param name="doc">A loaded XmlDocument that contains the Mappings.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddDocument( XmlDocument doc )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Mapping XML:\n" + doc.OuterXml );
			}
			try
			{
				AddXmlReader( new XmlNodeReader( doc ) );
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from XML document", e );
				throw new MappingException( e );
			}
			return this;
		}

		/// <summary>
		/// Takes the validated XmlDocument and has the Binder do its work of
		/// creating Mapping objects from the Mapping Xml.
		/// </summary>
		/// <param name="doc">The validated XmlDocument that contains the Mappings.</param>
		private void Add( XmlDocument doc )
		{
			try
			{
				Binder.dialect = Dialect.Dialect.GetDialect( properties );
				Binder.BindRoot( doc, CreateMappings() );
			}
			catch( MappingException me )
			{
				log.Error( "Could not compile the mapping document", me );
				throw;
			}
		}

		/// <summary>
		/// Create a new <c>Mappings</c> to add classes and collection mappings to
		/// </summary>
		/// <returns></returns>
		public Mappings CreateMappings()
		{
			return new Mappings( classes, collections, tables, namedQueries, imports, caches, secondPasses );
		}

		/// <summary>
		/// Adds the Xml Mappings from the Stream.
		/// </summary>
		/// <param name="xmlInputStream">The Stream to read Xml from.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddInputStream( Stream xmlInputStream )
		{
			try
			{
				AddXmlReader( new XmlTextReader( xmlInputStream ) );
				return this;
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from input stream", e );
				throw new MappingException( e );
			}
		}

		/// <summary>
		/// Adds the Mappings in the XmlReader after validating it against the
		/// nhibernate-mapping-2.0 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader( XmlReader hbmReader )
		{
			XmlValidatingReader validatingReader = new XmlValidatingReader( hbmReader );
			validatingReader.ValidationType = ValidationType.Schema;
			validatingReader.Schemas.Add( mappingSchema );

			XmlDocument hbmDocument = new XmlDocument();
			hbmDocument.Load( validatingReader );
			Add( hbmDocument );

			return this;
		}

		/// <summary>
		/// Adds the Mappings in the Resource of the Assembly.
		/// </summary>
		/// <param name="path">The path to the Resource file in the Assembly</param>
		/// <param name="assembly">The Assembly that contains the Resource file.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddResource( string path, Assembly assembly )
		{
			log.Info( "mapping resource: " + path );
			Stream rsrc = assembly.GetManifestResourceStream( path );
			if( rsrc == null )
			{
				throw new MappingException( "Resource: " + path + " not found" );
			}
			return AddInputStream( rsrc );
		}

		/// <summary>
		/// Adds the Class' Mapping by appending an ".hbm.xml" to the end of the Full Class Name
		/// and looking in the Class' Assembly.
		/// </summary>
		/// <param name="persistentClass">The Type to Map.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// If the Mappings and Classes are defined in different Assemblies or don't follow
		/// the same naming convention then this can not be used.
		/// </remarks>
		public Configuration AddClass( System.Type persistentClass )
		{
			string fileName = persistentClass.FullName + ".hbm.xml";
			log.Info( "Mapping resource: " + fileName );
			Stream rsrc = persistentClass.Assembly.GetManifestResourceStream( fileName );
			if( rsrc == null )
			{
				throw new MappingException( "Resource: " + fileName + " not found" );
			}
			return AddInputStream( rsrc );
		}

		/// <summary>
		/// Adds all of the Assembly's Resource files that end with "hbm.xml" 
		/// </summary>
		/// <param name="assemblyName">The name of the Assembly to load.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The Assembly must be in the local bin, probing path, or GAC so that the
		/// Assembly can be loaded by name.  If these conditions are not satisfied
		/// then your code should load the Assembly and call the override <see cref="AddAssembly(Assembly)"/>
		/// instead.
		/// </remarks>
		public Configuration AddAssembly( string assemblyName )
		{
			log.Info( "searching for mapped documents in assembly: " + assemblyName );

			Assembly assembly = null;
			try
			{
				assembly = Assembly.Load( assemblyName );
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from assembly", e );
				throw new MappingException( "Could not add assembly named: " + assemblyName, e );
			}

			return this.AddAssembly( assembly );
		}

		/// <summary>
		/// Adds all of the Assembly's Resource files that end with "hbm.xml" 
		/// </summary>
		/// <param name="assembly">The loaded Assembly.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddAssembly( Assembly assembly )
		{
			foreach( string fileName in assembly.GetManifestResourceNames() )
			{
				if( fileName.EndsWith( ".hbm.xml" ) )
				{
					log.Info( "Found mapping documents in assembly: " + fileName );
					try
					{
						AddInputStream( assembly.GetManifestResourceStream( fileName ) );
					}
					catch( MappingException )
					{
						throw;
					}
					catch( Exception e )
					{
						log.Error( "Could not configure datastore from assembly", e );
						throw new MappingException( e );
					}
				}
			}

			return this;
		}


		private ICollection CollectionGenerators( Dialect.Dialect dialect )
		{
			Hashtable generators = new Hashtable();
			foreach( PersistentClass clazz in classes.Values )
			{
				IIdentifierGenerator ig = clazz.Identifier.CreateIdentifierGenerator( dialect );
				if( ig is IPersistentIdentifierGenerator )
				{
					generators[ ( ( IPersistentIdentifierGenerator ) ig ).GeneratorKey() ] = ig;
				}

			}
			return generators.Values;
		}

		/// <summary>
		/// Generate DDL for droping tables
		/// </summary>
		/// <param name="dialect"></param>
		public string[ ] GenerateDropSchemaScript( Dialect.Dialect dialect )
		{
			SecondPassCompile();

			ArrayList script = new ArrayList( 50 );

			if( dialect.DropConstraints )
			{
				foreach( Table table in TableMappings )
				{
					foreach( ForeignKey fk in table.ForeignKeyCollection )
					{
						script.Add( fk.SqlDropString( dialect ) );
					}
				}
			}

			foreach( Table table in TableMappings )
			{
				script.Add( table.SqlDropString( dialect ) );
			}

			foreach( IPersistentIdentifierGenerator idGen in CollectionGenerators( dialect ) )
			{
				string dropString = idGen.SqlDropString( dialect );
				if( dropString != null )
				{
					script.Add( dropString );
				}
			}

			return ArrayHelper.ToStringArray( script );
		}

		/// <summary>
		/// Generate DDL for creating tables
		/// </summary>
		/// <param name="dialect"></param>
		public string[ ] GenerateSchemaCreationScript( Dialect.Dialect dialect )
		{
			SecondPassCompile();

			ArrayList script = new ArrayList( 50 );

			foreach( Table table in TableMappings )
			{
				script.Add( table.SqlCreateString( dialect, this ) );
			}

			foreach( Table table in TableMappings )
			{
				if( dialect.HasAlterTable )
				{
					foreach( ForeignKey fk in table.ForeignKeyCollection )
					{
						script.Add( fk.SqlCreateString( dialect, this ) );
					}
				}
				foreach( Index index in table.IndexCollection )
				{
					script.Add( index.SqlCreateString( dialect, this ) );
				}
			}

			foreach( IPersistentIdentifierGenerator idGen in CollectionGenerators( dialect ) )
			{
				string[ ] lines = idGen.SqlCreateStrings( dialect );
				for( int i = 0; i < lines.Length; i++ )
				{
					script.Add( lines[ i ] );
				}
			}

			return ArrayHelper.ToStringArray( script );
		}

		//		///<summary>
		//		/// Generate DDL for altering tables
		//		///</summary>
		//		public string[] GenerateSchemaUpdateScript(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata) 
		//		{
		//			secondPassCompile();
		//		
		//			ArrayList script = new ArrayList(50);
		//
		//			foreach(Table table in TableMappings)
		//			{
		//				TableMetadata tableInfo = databaseMetadata.getTableMetadata( table.Name );
		//				if (tableInfo==null) 
		//				{
		//					script.Add( table.SqlCreateString(dialect, this) );
		//				}
		//				else 
		//				{
		//					foreach(string alterString in table.SqlAlterStrings(dialect, this, tableInfo))
		//						script.Add(alterString);
		//				}
		//			}
		//		
		//			foreach(Table table in TableMappings)
		//			{
		//				TableMetadata tableInfo = databaseMetadata.getTableMetadata( table.Name );
		//			
		//				if ( dialect.HasAlterTable)
		//				{
		//					foreach(ForeignKey fk in table.ForeignKeyCollection)
		//						if ( tableInfo==null || tableInfo.getForeignKeyMetadata( fk.Name ) == null ) 
		//						{
		//							script.Add( fk.SqlCreateString(dialect, mapping) );
		//						}
		//				}
		//				foreach(Index index in table.IndexCollection)
		//				{
		//					if ( tableInfo==null || tableInfo.getIndexMetadata( index.Name ) == null ) 
		//					{
		//						script.Add( index.SqlCreateString(dialect, mapping) );
		//					}
		//				}
		//			}
		//
		//			foreach(IPersistentIdentifierGenerator generator in CollectionGenerators(dialect))
		//			{
		//				object key = generator.GeneratorKey();
		//				if ( !databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key) ) 
		//				{
		//					string[] lines = generator.SqlCreateStrings(dialect);
		//					for (int i = 0; i < lines.Length; i++) script.Add( lines[i] );
		//				}
		//			}
		//		
		//			return ArrayHelper.ToStringArray(script);
		//		}
		//TODO: H2.0.3 After DatabaseMetadata is completed

		/// <remarks>
		/// This method may be called many times!!
		/// </remarks>
		private void SecondPassCompile()
		{
			log.Info( "processing one-to-many association mappings" );

			foreach( Binder.SecondPass sp in secondPasses )
			{
				sp.DoSecondPass( classes );
			}

			secondPasses.Clear();

			//TODO: Somehow add the newly created foreign keys to the internal collection

			log.Info( "processing foreign key constraints" );

			foreach( Table table in TableMappings )
			{
				foreach( ForeignKey fk in table.ForeignKeyCollection )
				{
					if( fk.ReferencedTable == null )
					{
						if( log.IsDebugEnabled )
						{
							log.Debug( "resolving reference to class: " + fk.ReferencedClass.Name );
						}
						PersistentClass referencedClass = ( PersistentClass ) classes[ fk.ReferencedClass ];
						if( referencedClass == null )
						{
							throw new MappingException(
								"An association from the table " +
									fk.Table.Name +
									" refers to an unmapped class: " +
									fk.ReferencedClass.Name
								);
						}
						fk.ReferencedTable = referencedClass.Table;
					}
				}
			}
		}

		/// <summary>
		/// The named queries
		/// </summary>
		public IDictionary NamedQueries
		{
			get { return namedQueries; }
		}

		private static readonly IInterceptor EmptyInterceptor = new EmptyInterceptorClass();

		[Serializable]
		private class EmptyInterceptorClass : IInterceptor
		{
			public void OnDelete( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
			{
			}

			public bool OnFlushDirty( object entity, object id, object[ ] currentState, object[ ] previousState, string[ ] propertyNames, IType[ ] types )
			{
				return false;
			}

			public bool OnLoad( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
			{
				return false;
			}

			public bool OnSave( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
			{
				return false;
			}

			public void OnPostFlush( object entity, object id, object[ ] currentState, string[ ] propertyNames, IType[ ] types )
			{
			}

			public void PostFlush( ICollection entities )
			{
			}

			public void PreFlush( ICollection entitites )
			{
			}

			public object IsUnsaved( object entity )
			{
				return null;
			}

			public object Instantiate( System.Type clazz, object id )
			{
				return null;
			}

			public int[ ] FindDirty( object entity, object id, object[ ] currentState, object[ ] previousState, string[ ] propertyNames, IType[ ] types )
			{
				return null;
			}
		}

		/// <summary>
		/// Instantitate a new <c>ISessionFactory</c>, using the properties and mappings in this
		/// configuration. The <c>ISessionFactory</c> will be immutable, so changes made to the
		/// <c>Configuration</c> after building the <c>ISessionFactory</c> will not affect it.
		/// </summary>
		/// <returns></returns>
		public ISessionFactory BuildSessionFactory()
		{
			// Version check to determine which version of .NET is running
			if( ( System.Environment.Version.Major == 1 )
				&& ( System.Environment.Version.Minor == 0 ) )
			{
				throw new HibernateException( "Wrong version of .NET framework is used, NHibernate is not to be used with versions prior to 1.1.4322" );
			}

			SecondPassCompile();
			Hashtable copy = new Hashtable();
			foreach( DictionaryEntry de in properties )
			{
				copy.Add( de.Key, de.Value );
			}

			Settings settings = BuildSettings();
			ConfigureCaches( settings );

			return new SessionFactoryImpl( this, copy, interceptor, settings );
		}

		/// <summary>
		/// Builds an object-oriented view of the settings.
		/// </summary>
		/// <returns>A <see cref="Settings"/> object initialized from the settings properties.</returns>
		protected Settings BuildSettings()
		{
			return SettingsFactory.BuildSettings( properties );
		}

		/// <summary></summary>
		public IInterceptor Interceptor
		{
			get { return interceptor; }
			set { this.interceptor = value; }
		}

		/// <summary></summary>
		public IDictionary Properties
		{
			get { return properties; }
			set { this.properties = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public Configuration AddProperties( IDictionary properties )
		{
			foreach( DictionaryEntry de in properties )
			{
				this.properties.Add( de.Key, de.Value );
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Configuration SetProperty( string name, string value )
		{
			properties[ name ] = value;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetProperty( string name )
		{
			return properties[ name ] as string;
		}

		private void AddProperties( XmlNode parent )
		{
			foreach( XmlNode node in parent.SelectNodes( CfgNamespacePrefix + ":property", CfgNamespaceMgr ) )
			{
				string name = node.Attributes[ "name" ].Value;
				string value = node.FirstChild.Value;
				log.Debug( name + "=" + value );
				properties[ name ] = value;
				if( !name.StartsWith( "hibernate" ) )
				{
					properties[ "hibernate." + name ] = value;
				}
			}
		}

		/// <summary>
		/// Configure NHibernate using the file "hibernate.cfg.xml"
		/// </summary>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure() will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure()
		{
			return Configure( "hibernate.cfg.xml" );
		}

		/// <summary>
		/// Configure NHibernate using the file specified.
		/// </summary>
		/// <param name="resource">The location of the Xml file to use to configure NHibernate.</param>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure(string) will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure( string resource )
		{
			XmlTextReader reader = new XmlTextReader( resource );
			return Configure( reader );
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
		public Configuration Configure( Assembly assembly, string resourceName )
		{
			if( assembly == null || resourceName == null )
			{
				throw new HibernateException( "Could not configure NHibernate.", new ArgumentException( "A null value was passed in.", "assembly or resourceName" ) );
			}

			Stream stream = assembly.GetManifestResourceStream( resourceName );
			if( stream == null )
			{
				// resource does not exist - throw appropriate exception 
				throw new HibernateException( "A ManifestResourceStream could not be created for the resource " + resourceName + " in Assembly " + assembly.FullName );
			}

			return Configure( new XmlTextReader( stream ) );

		}

		/// <summary>
		/// Configure NHibernate using the specified XmlTextReader.
		/// </summary>
		/// <param name="reader">The <see cref="XmlTextReader"/> that contains the Xml to configure NHibernate.</param>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// Calling Configure(XmlTextReader) will overwrite the values set in app.config or web.config
		/// </remarks>
		public Configuration Configure( XmlTextReader reader )
		{
			if( reader == null )
			{
				throw new HibernateException( "Could not configure NHibernate.", new ArgumentException( "A null value was passed in.", "reader" ) );
			}

			XmlDocument doc = new XmlDocument();
			XmlValidatingReader validatingReader = null;

			try
			{
				validatingReader = new XmlValidatingReader( reader );
				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.Schemas.Add( cfgSchema );

				doc.Load( validatingReader );
				CfgNamespaceMgr = new XmlNamespaceManager( doc.NameTable );
				// note that the prefix has absolutely nothing to do with what the user
				// selects as their prefix in the document.  It is the prefix we use to 
				// build the XPath and the nsmgr takes care of translating our prefix into
				// the user defined prefix...
				CfgNamespaceMgr.AddNamespace( CfgNamespacePrefix, Configuration.CfgSchemaXMLNS );
			}
			catch( Exception e )
			{
				log.Error( "Problem parsing configuration", e );
				throw new HibernateException( "problem parsing configuration : " + e );
			}

			XmlNode sfNode = doc.DocumentElement.SelectSingleNode( CfgNamespacePrefix + ":session-factory", CfgNamespaceMgr );
			XmlAttribute name = sfNode.Attributes[ "name" ];
			if( name != null )
			{
				properties[ Environment.SessionFactoryName ] = name.Value;
			}
			AddProperties( sfNode );

			foreach( XmlNode mapElement in sfNode.ChildNodes )
			{
				string elemname = mapElement.LocalName;
				if( "mapping".Equals( elemname ) )
				{
					XmlAttribute rsrc = mapElement.Attributes[ "resource" ];
					XmlAttribute file = mapElement.Attributes[ "file" ];
					XmlAttribute assembly = mapElement.Attributes[ "assembly" ];
					if( rsrc != null )
					{
						log.Debug( name.Value + "<-" + rsrc.Value + " in " + assembly.Value );
						AddResource( rsrc.Value, Assembly.Load( assembly.Value ) );
					}
					else if( assembly != null )
					{
						log.Debug( name.Value + "<-" + assembly.Value );
						AddAssembly( assembly.Value );
					}
					else
					{
						if( file == null )
						{
							throw new MappingException( "<mapping> element in configuration specifies no attributes" );
						}
						log.Debug( name.Value + "<-" + file.Value );
						AddXmlFile( file.Value );
					}
				}
			}

			log.Info( "Configured SessionFactory: " + name.Value );
			log.Debug( "properties: " + properties );

			validatingReader.Close();
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="settings"></param>
		protected void ConfigureCaches( Settings settings )
		{
			log.Info( "instantiating and configuring caches" );

			// TODO: add ability to configure cache_region_prefix

			foreach( DictionaryEntry de in caches )
			{
				string name = ( string ) de.Key;
				ICacheConcurrencyStrategy strategy = ( ICacheConcurrencyStrategy ) de.Value;

				if( log.IsDebugEnabled )
				{
					log.Debug( "instantiating cache " + name );
				}

				ICache cache;
				try
				{
					cache = settings.CacheProvider.BuildCache( name, properties );
				}
				catch( CacheException ce )
				{
					throw new HibernateException( "Could not instantiate Cache", ce );
				}

				strategy.Cache = cache;

			}
		}

		/// <summary>
		/// Get the query language imports
		/// </summary>
		/// <returns></returns>
		public IDictionary Imports
		{
			get { return imports; }
		}
	}
}