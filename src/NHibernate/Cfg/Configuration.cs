using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Iesi.Collections;
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
	/// are immutable and do not retain any association back to the <c>Configuration</c>
	/// </para>
	/// </remarks>
	public class Configuration
	{
		#region NHibernate-Specific Members

		private XmlSchemaCollection mappingSchemaCollection;
		private XmlSchemaCollection cfgSchemaCollection;

		/// <summary>
		/// The XML Namespace for the nhibernate-mapping
		/// </summary>
		public static readonly string MappingSchemaXMLNS = "urn:nhibernate-mapping-2.0";

		private const string MappingSchemaResource = "NHibernate.nhibernate-mapping-2.0.xsd";

		/// <summary>
		/// The XML Namespace for the nhibernate-configuration
		/// </summary>
		public static readonly string CfgSchemaXMLNS = "urn:nhibernate-configuration-2.0";

		private const string CfgSchemaResource = "NHibernate.nhibernate-configuration-2.0.xsd";
		private const string CfgNamespacePrefix = "cfg";

		private string currentDocumentName;

		/// <summary></summary>
		/// <remarks>Allocate on first use as we are expensive in time/space</remarks>
		private XmlSchemaCollection MappingSchemaCollection
		{
			get
			{
				if( mappingSchemaCollection == null )
				{
					mappingSchemaCollection = new XmlSchemaCollection();
					mappingSchemaCollection.Add( XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream( MappingSchemaResource ), null ) );
				}
				return mappingSchemaCollection;
			}
			set { mappingSchemaCollection = value; }
		}

		/// <summary></summary>
		/// <remarks>Allocate on first use as we are expensive in time/space</remarks>
		private XmlSchemaCollection CfgSchemaCollection
		{
			get
			{
				if( cfgSchemaCollection == null )
				{
					cfgSchemaCollection = new XmlSchemaCollection();
					cfgSchemaCollection.Add( XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream( CfgSchemaResource ), null ) );
				}
				return cfgSchemaCollection;
			}
			set { cfgSchemaCollection = value; }
		}

		#endregion

		private IDictionary classes;
		private Hashtable imports;
		private Hashtable collections;
		private Hashtable tables;
		private Hashtable namedQueries;
		private Hashtable namedSqlQueries;
		private Hashtable sqlResultSetMappings;
		private ArrayList secondPasses;
		private ArrayList propertyReferences;
		private IInterceptor interceptor;
		private IDictionary properties;
		private IDictionary caches;

		private INamingStrategy namingStrategy = DefaultNamingStrategy.Instance;

		private static readonly ILog log = LogManager.GetLogger( typeof( Configuration ) );

		/// <summary>
		/// Clear the internal state of the <see cref="Configuration"/> object.
		/// </summary>
		private void Reset()
		{
			classes = new Hashtable(); //new SequencedHashMap(); - to make NH-369 bug deterministic
			imports = new Hashtable();
			collections = new Hashtable();
			tables = new Hashtable();
			namedQueries = new Hashtable();
			namedSqlQueries = new Hashtable();
			sqlResultSetMappings = new Hashtable();
			secondPasses = new ArrayList();
			propertyReferences = new ArrayList();
			interceptor = emptyInterceptor;
			caches = new Hashtable();
			mapping = new Mapping( this );
			properties = Environment.Properties;
		}

		private class Mapping : IMapping
		{
			private Configuration configuration;

			public Mapping( Configuration configuration )
			{
				this.configuration = configuration;
			}

			private PersistentClass GetPersistentClass( System.Type type )
			{
				PersistentClass pc = ( PersistentClass ) configuration.classes[ type ];
				if( pc == null )
				{
					throw new MappingException( "persistent class not known: " + type.FullName );
				}
				return pc;
			}

			public IType GetIdentifierType( System.Type persistentClass )
			{
				return GetPersistentClass( persistentClass ).Identifier.Type;
			}

			public string GetIdentifierPropertyName( System.Type persistentClass )
			{
				PersistentClass pc = GetPersistentClass( persistentClass );
				if( !pc.HasIdentifierProperty )
				{
					return null;
				}
				return pc.IdentifierProperty.Name;
			}

			public IType GetPropertyType( System.Type persistentClass, string propertyName )
			{
				PersistentClass pc = GetPersistentClass( persistentClass );
				NHibernate.Mapping.Property prop = pc.GetProperty( propertyName );

				if( prop == null )
				{
					throw new MappingException( "property not known: " + persistentClass.FullName + '.' + propertyName );
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
		/// <param name="role">a collection role</param>
		/// <returns><see cref="NHibernate.Mapping.Collection" /></returns>
		public NHibernate.Mapping.Collection GetCollectionMapping( string role )
		{
			return ( NHibernate.Mapping.Collection ) collections[ role ];
		}

		/// <summary>
		/// Read mappings from a particular XML file. This method is equivalent
		/// to <see cref="AddXmlFile(string)" />.
		/// </summary>
		/// <param name="xmlFile"></param>
		/// <returns></returns>
		public Configuration AddFile( string xmlFile )
		{
			return AddXmlFile( xmlFile );
		}

		public Configuration AddFile( FileInfo xmlFile )
		{
			return AddFile( xmlFile.FullName );
		}
		
		private static void LogAndThrow( MappingException me )
		{
			log.Error( me.Message, me );
			throw me;
		}

		/// <summary>
		/// Read mappings from a particular XML file.
		/// </summary>
		/// <param name="xmlFile">a path to a file</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXmlFile( string xmlFile )
		{
			log.Info( "Mapping file: " + xmlFile );
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader( xmlFile );
				AddXmlReader( textReader, xmlFile );
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException( "Could not configure datastore from file " + xmlFile, e ) );
			}
			finally
			{
				if( textReader != null )
				{
					textReader.Close();
				}
			}
			return this;
		}
		
		public Configuration AddXml( string xml )
		{
			return AddXml( xml, "(string)" );
		}

		/// <summary>
		/// Read mappings from a <c>string</c>. This method is equivalent to
		/// <see cref="AddXmlString(string)" />.
		/// </summary>
		/// <param name="xml">an XML string</param>
		/// <param name="name">The name to use in error reporting. May be <c>null</c>.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXml( string xml, string name )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Mapping XML:\n" + xml );
			}
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader( xml, XmlNodeType.Document, null );
				// make a StringReader for the string passed in - the StringReader
				// inherits from TextReader.  We can use the XmlTextReader.ctor that
				// takes the TextReader to build from a string...
				AddXmlReader( reader, name );
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException("Could not configure datastore from XML string " + name, e ) );
			}
			finally
			{
				if( reader != null )
				{
					reader.Close();
				}
			}
			return this;
		}

		/// <summary>
		/// Read mappings from a <c>string</c>
		/// </summary>
		/// <param name="xml">an XML string</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddXmlString( string xml )
		{
			return AddXml( xml );
		}

		/// <summary>
		/// Read mappings from a URL.
		/// </summary>
		/// <param name="url">a URL</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddUrl( string url )
		{
			// AddFile works for URLs currently
			return AddFile( url );
		}

		/// <summary>
		/// Read mappings from a URL.
		/// </summary>
		/// <param name="url">a <see cref="Uri" /> to read the mappings from.</param>
		/// <returns>This configuration object.</returns>
		public Configuration AddUrl( Uri url )
		{
			return AddUrl( url.AbsolutePath );
		}

		public Configuration AddDocument( XmlDocument doc )
		{
			return AddDocument( doc, "(XmlDocument)" );
		}

		/// <summary>
		/// Read mappings from an <c>XmlDocument</c>.
		/// </summary>
		/// <param name="doc">A loaded XmlDocument that contains the Mappings.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddDocument( XmlDocument doc, string name )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Mapping XML:\n" + doc.OuterXml );
			}

			try
			{
				using( MemoryStream ms = new MemoryStream() )
				{
					doc.Save( ms );
					ms.Position = 0;
					AddInputStream( ms, name );
				}
				return this;
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException( "Could not configure datastore from XML document " + name, e ) );
				return this; // To please the compiler
			}
		}

		/// <summary>
		/// Takes the validated XmlDocument and has the Binder do its work of
		/// creating Mapping objects from the Mapping Xml.
		/// </summary>
		/// <param name="doc">The <b>validated</b> XmlDocument that contains the Mappings.</param>
		private void AddValidatedDocument( XmlDocument doc, string name )
		{
			try
			{
				HbmBinder.dialect = Dialect.Dialect.GetDialect( properties );
				HbmBinder.BindRoot( doc, CreateMappings() );
			}
			catch( Exception e )
			{
				string nameFormatted = name == null ? "(unknown)" : name;
				LogAndThrow( new MappingException( "Could not compile the mapping document: " + nameFormatted, e ) );
			}
		}

		/// <summary>
		/// Create a new <c>Mappings</c> to add classes and collection
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
				caches,
				secondPasses,
				propertyReferences,
				namingStrategy
				);
		}

		/// <summary>
		/// Read mappings from a <c>Stream</c>.
		/// </summary>
		/// <param name="xmlInputStream">The stream containing XML</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The <see cref="Stream"/> passed in through the parameter <c>xmlInputStream</c>
		/// is not <b>guaranteed</b> to be cleaned up by this method.  It is the callers responsiblity to
		/// ensure that the <c>xmlInputStream</c> is properly handled when this method
		/// completes.
		/// </remarks>
		public Configuration AddInputStream( Stream xmlInputStream )
		{
			return AddInputStream( xmlInputStream, null );
		}
		
		/// <summary>
		/// Read mappings from a <c>Stream</c>.
		/// </summary>
		/// <param name="xmlInputStream">The stream containing XML</param>
		/// <param name="name">The name of the stream to use in error reporting. May be <c>null</c>.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The <see cref="Stream"/> passed in through the parameter <c>xmlInputStream</c>
		/// is not <b>guaranteed</b> to be cleaned up by this method.  It is the callers responsiblity to
		/// ensure that the <c>xmlInputStream</c> is properly handled when this method
		/// completes.
		/// </remarks>
		public Configuration AddInputStream( Stream xmlInputStream, string name )
		{
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader( xmlInputStream );
				AddXmlReader( textReader, name );
				return this;
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException( "Could not configure datastore from input stream " + name, e ) );
				return this; // To please the compiler
			}
			finally
			{
				if( textReader != null )
				{
					textReader.Close();
				}
			}
		}

		/// <summary>
		/// Adds the Mappings in the Resource of the Assembly.
		/// </summary>
		/// <param name="path">The path to the Resource file in the Assembly</param>
		/// <param name="assembly">The Assembly that contains the Resource file.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddResource( string path, Assembly assembly )
		{
			log.Info( "Mapping resource: " + path );
			Stream rsrc = assembly.GetManifestResourceStream( path );
			if( rsrc == null )
			{
				LogAndThrow( new MappingException( "Resource not found: " + path ) );
			}

			try
			{
				return AddInputStream( rsrc, path );
			}
			catch( MappingException )
			{
				throw;
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException( "Could not configure datastore from resource " + path, e ) );
				return this; // To please the compiler
			}
			finally
			{
				if( rsrc != null )
				{
					rsrc.Close();
				}
			}
		}

		// Not ported - addResource(String path) - not applicable

		/// <summary>
		/// Read a mapping from an application resource, using a convention.
		/// The class <c>Foo.Bar.Foo</c> is mapped by the resource named
		/// <c>Foo.Bar.Foo.hbm.xml</c>, embedded in the class' assembly.
		/// </summary>
		/// <param name="persistentClass">The type to map.</param>
		/// <returns>This configuration object.</returns>
		/// <remarks>
		/// If the mappings and classes are defined in different assemblies
		/// or don't follow the naming convention, then this method cannot
		/// be used.
		/// </remarks>
		public Configuration AddClass( System.Type persistentClass )
		{
			return AddResource( persistentClass.FullName + ".hbm.xml", persistentClass.Assembly );
		}

		/// <summary>
		/// Adds all of the Assembly's Resource files that end with "<c>hbm.xml</c>"
		/// </summary>
		/// <param name="assemblyName">The name of the Assembly to load.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The Assembly must be in the local bin, probing path, or GAC so that the
		/// Assembly can be loaded by name.  If these conditions are not satisfied
		/// then your code should load the Assembly and call the override 
		/// <see cref="AddAssembly(Assembly)"/> instead.
		/// </remarks>
		public Configuration AddAssembly( string assemblyName )
		{
			log.Info( "Searching for mapped documents in assembly: " + assemblyName );

			Assembly assembly = null;
			try
			{
				assembly = Assembly.Load( assemblyName );
			}
			catch( Exception e )
			{
				LogAndThrow( new MappingException( "Could not add assembly " + assemblyName, e ) );
			}

			return AddAssembly( assembly );
		}

		/// <summary>
		/// Adds all of the Assembly's Resource files that end with "hbm.xml" 
		/// </summary>
		/// <param name="assembly">The loaded Assembly.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// This assumes that the <c>hbm.xml</c> files in the Assembly need to be put
		/// in the correct order by NHibernate.  See <see cref="AddAssembly(Assembly, bool)">
		/// AddAssembly(Assembly assembly, bool skipOrdering)</see>
		/// for the impacts and reasons for letting NHibernate order the 
		/// <c>hbm.xml</c> files.
		/// </remarks>
		public Configuration AddAssembly( Assembly assembly )
		{
			// assume ordering is needed because that is the
			// safest way to handle it.
			return AddAssembly( assembly, false );
		}

		/// <summary>
		/// Adds all of the Assembly's Resource files that end with "hbm.xml" 
		/// </summary>
		/// <param name="assembly">The loaded Assembly.</param>
		/// <param name="skipOrdering">
		/// A <see cref="Boolean"/> indicating if the ordering of hbm.xml files can be skipped.
		/// </param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// <p>
		/// The order of <c>hbm.xml</c> files only matters if the attribute "extends" is used.
		/// The ordering should only be done when needed because it takes extra time 
		/// to read the Xml files to find out the order the files should be passed to the Binder.  
		/// If you don't use the "extends" attribute then it is reccommended to call this 
		/// with <c>skipOrdering=true</c>.
		/// </p>
		/// </remarks>
		public Configuration AddAssembly( Assembly assembly, bool skipOrdering )
		{
			IList resources = null;
			if( skipOrdering )
			{
				resources = assembly.GetManifestResourceNames();
			}
			else
			{
				AssemblyHbmOrderer orderer = new AssemblyHbmOrderer( assembly );
				resources = orderer.GetHbmFiles();
			}

			foreach( string fileName in resources )
			{
				if( fileName.EndsWith( ".hbm.xml" ) )
				{
					log.Info( "Found mapping document in assembly: " + fileName );
					AddResource( fileName, assembly );
				}
			}

			return this;
		}

		/// <summary>
		/// Read all mapping documents from a directory tree. Assume that any
		/// file named <c>*.hbm.xml</c> is a mapping document.
		/// </summary>
		/// <param name="dir">a directory</param>
		public Configuration AddDirectory( DirectoryInfo dir )
		{
			foreach( DirectoryInfo subDirectory in dir.GetDirectories() )
			{
				AddDirectory( subDirectory );
			}

			foreach( FileInfo hbmXml in dir.GetFiles( "*.hbm.xml" ) )
			{
				AddFile( hbmXml );
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

			foreach( NHibernate.Mapping.Collection collection in collections.Values )
			{
				if( collection is IdentifierCollection )
				{
					IIdentifierGenerator ig = ( ( IdentifierCollection ) collection )
						.Identifier
						.CreateIdentifierGenerator( dialect );

					if( ig is IPersistentIdentifierGenerator )
					{
						generators[ ( ( IPersistentIdentifierGenerator ) ig ).GeneratorKey() ] = ig;
					}
				}
			}
			return generators.Values;
		}

		/// <summary>
		/// Generate DDL for droping tables
		/// </summary>
		/// <remarks>
		/// <seealso cref="NHibernate.Tool.hbm2ddl.SchemaExport" />
		/// </remarks>
		public string[] GenerateDropSchemaScript( Dialect.Dialect dialect )
		{
			SecondPassCompile();

			ArrayList script = new ArrayList( 50 );

			if( dialect.DropConstraints )
			{
				foreach( Table table in TableMappings )
				{
					foreach( ForeignKey fk in table.ForeignKeyCollection )
					{
						script.Add( fk.SqlDropString( dialect, ( string ) properties[ Environment.DefaultSchema ] ) );
					}
				}
			}

			foreach( Table table in TableMappings )
			{
				script.Add( table.SqlDropString( dialect, ( string ) properties[ Environment.DefaultSchema ] ) );
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
		public string[] GenerateSchemaCreationScript( Dialect.Dialect dialect )
		{
			SecondPassCompile();

			ArrayList script = new ArrayList( 50 );

			foreach( Table table in TableMappings )
			{
				script.Add( table.SqlCreateString( dialect, mapping, ( string ) properties[ Environment.DefaultSchema ] ) );
			}

			foreach( Table table in TableMappings )
			{
				/*
				TODO: H2.1.8 has the code below, but only TimesTen dialect ever
				enters the if, so I don't want to add this to Dialect right now

				if( !dialect.SupportsUniqueConstraintInCreateAlterTable )
				{
					foreach( UniqueKey uk in table.UniqueKeyCollection )
					{
						script.Add( uk.SqlCreateString( dialect, mapping, ( string ) properties[ Environment.DefaultSchema ] ) );
					}
				}
				*/

				foreach( Index index in table.IndexCollection )
				{
					script.Add( index.SqlCreateString( dialect, mapping, ( string ) properties[ Environment.DefaultSchema ] ) );
				}

				if( dialect.HasAlterTable )
				{
					foreach( ForeignKey fk in table.ForeignKeyCollection )
					{
						script.Add( fk.SqlCreateString( dialect, mapping, ( string ) properties[ Environment.DefaultSchema ] ) );
					}
				}
			}

			foreach( IPersistentIdentifierGenerator idGen in CollectionGenerators( dialect ) )
			{
				string[] lines = idGen.SqlCreateStrings( dialect );
				script.AddRange( lines );
			}

			return ArrayHelper.ToStringArray( script );
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
			bool validateProxy = PropertiesHelper.GetBoolean( Environment.UseProxyValidator, properties, true );

			foreach( PersistentClass clazz in classes.Values )
			{
				clazz.Validate( mapping );
				if( validateProxy )
				{
					ValidateProxyInterface( clazz );
				}
			}

			foreach( NHibernate.Mapping.Collection col in collections.Values )
			{
				col.Validate( mapping );
			}
		}

		private static void ValidateProxyInterface( PersistentClass persistentClass )
		{
			if( !persistentClass.IsLazy )
			{
				// Nothing to validate
				return;
			}

			if( persistentClass.ProxyInterface == null )
			{
				// Nothing to validate
				return;
			}

			Proxy.ProxyTypeValidator.ValidateType( persistentClass.ProxyInterface );
		}

		/// <remarks>
		/// This method may be called many times!!
		/// </remarks>
		private void SecondPassCompile()
		{
			log.Info( "processing one-to-many association mappings" );

			foreach( ISecondPass sp in secondPasses )
			{
				sp.DoSecondPass( classes );
			}

			secondPasses.Clear();

			log.Info( "processing one-to-one association property references" );

			foreach( Mappings.UniquePropertyReference upr in propertyReferences )
			{
				PersistentClass clazz = GetClassMapping( upr.ReferencedClass );
				if( clazz == null )
				{
					throw new MappingException( "property-ref to unmapped class: " + upr.ReferencedClass.Name );
				}
				bool found = false;

				foreach( NHibernate.Mapping.Property prop in clazz.PropertyCollection )
				{
					if( upr.PropertyName.Equals( prop.Name ) )
					{
						( ( SimpleValue ) prop.Value ).IsUnique = true;
						found = true;
						break;
					}
				}

				if( !found )
				{
					throw new MappingException(
						"property-ref not found: " + upr.PropertyName +
							" in class: " + upr.ReferencedClass.Name
						);
				}
			}

			//TODO: Somehow add the newly created foreign keys to the internal collection

			log.Info( "processing foreign key constraints" );

			ISet done = new HashedSet();
			foreach( Table table in TableMappings )
			{
				SecondPassCompileForeignKeys( table, done );
			}
		}

		private void SecondPassCompileForeignKeys( Table table, ISet done )
		{
			foreach( ForeignKey fk in table.ForeignKeyCollection )
			{
				if( !done.Contains( fk ) )
				{
					done.Add( fk );
					if( log.IsDebugEnabled )
					{
						log.Debug( "resolving reference to class: " + fk.ReferencedClass.Name );
					}
					PersistentClass referencedClass = ( PersistentClass ) classes[ fk.ReferencedClass ];
					if( referencedClass == null )
					{
						string messageTemplate = "An association from the table {0} refers to an unmapped class: {1}";
						string message = string.Format( messageTemplate, fk.Table.Name, fk.ReferencedClass.Name );

						LogAndThrow( new MappingException( message ) );
					}

					if( referencedClass.IsJoinedSubclass )
					{
						SecondPassCompileForeignKeys( referencedClass.Superclass.Table, done );
					}

					try
					{
						fk.ReferencedTable = referencedClass.Table;
					}
					catch( MappingException me )
					{
						if( log.IsErrorEnabled )
						{
							log.Error( me );
						}

						// rethrow the error - only caught it for logging purposes
						throw;
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

		private static readonly IInterceptor emptyInterceptor = new EmptyInterceptor();

		/// <summary>
		/// Instantitate a new <c>ISessionFactory</c>, using the properties and mappings in this
		/// configuration. The <c>ISessionFactory</c> will be immutable, so changes made to the
		/// <c>Configuration</c> after building the <c>ISessionFactory</c> will not affect it.
		/// </summary>
		/// <returns></returns>
		public ISessionFactory BuildSessionFactory()
		{
			SecondPassCompile();
			Validate();
			Environment.VerifyProperties( properties );
			Settings settings = BuildSettings();
			ConfigureCaches( settings );

			// Ok, don't need schemas anymore, so free them
			MappingSchemaCollection = null;
			CfgSchemaCollection = null;

			return new SessionFactoryImpl( this, mapping, settings );
		}

		/// <summary>
		/// Gets or sets the <see cref="IInterceptor"/> to use.
		/// </summary>
		/// <value>The <see cref="IInterceptor"/> to use.</value>
		public IInterceptor Interceptor
		{
			get { return interceptor; }
			set { this.interceptor = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IDictionary"/> that contains the configuration
		/// Properties and their values.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> that contains the configuration
		/// Properties and their values.
		/// </value>
		public IDictionary Properties
		{
			get { return properties; }
			set { this.properties = value; }
		}

		/// <summary>
		/// Configure an <see cref="IInterceptor" />
		/// </summary>
		public Configuration SetInterceptor( IInterceptor interceptor )
		{
			this.interceptor = interceptor;
			return this;
		}

		/// <summary>
		/// Specify a completely new set of properties
		/// </summary>
		public Configuration SetProperties( IDictionary properties )
		{
			this.properties = properties;
			return this;
		}

		/// <summary>
		/// Adds an <see cref="IDictionary"/> of configuration properties.  The 
		/// Key is the name of the Property and the Value is the <see cref="String"/>
		/// value of the Property.
		/// </summary>
		/// <param name="properties">An <see cref="IDictionary"/> of configuration properties.</param>
		/// <returns>
		/// This <see cref="Configuration"/> object.
		/// </returns>
		public Configuration AddProperties( IDictionary properties )
		{
			foreach( DictionaryEntry de in properties )
			{
				this.properties.Add( de.Key, de.Value );
			}
			return this;
		}

		/// <summary>
		/// Sets the value of the configuration property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		/// <returns>
		/// This <see cref="Configuration"/> object.
		/// </returns>
		public Configuration SetProperty( string name, string value )
		{
			properties[ name ] = value;
			return this;
		}

		/// <summary>
		/// Gets the value of the configuration property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The configured value of the property, or <c>null</c> if the property was not specified.</returns>
		public string GetProperty( string name )
		{
			return properties[ name ] as string;
		}

		private void AddProperties( XmlNode parent, XmlNamespaceManager cfgNamespaceMgr )
		{
			foreach( XmlNode node in parent.SelectNodes( CfgNamespacePrefix + ":property", cfgNamespaceMgr ) )
			{
				string name = node.Attributes[ "name" ].Value;
				string value = node.InnerText;
				if( log.IsDebugEnabled )
				{
					log.Debug( name + "=" + value );
				}
				properties[ name ] = value;
				if( !name.StartsWith( "hibernate" ) )
				{
					properties[ "hibernate." + name ] = value;
				}
			}
			Environment.VerifyProperties( properties );
		}

		// TODO - getConfigurationInputStream(String resource)

		/// <summary>
		/// Configure NHibernate using the <c>&lt;hibernate-configuration&gt;</c> section
		/// from the application config file, if found, or the file <c>hibernate.cfg.xml</c>
		/// otherwise.
		/// </summary>
		/// <returns>A Configuration object initialized with the file.</returns>
		/// <remarks>
		/// To configure NHibernate explicitly using <c>hibernate.cfg.xml</c>, ignoring
		/// the application configuration file, use this code:
		/// <code>
		///		configuration.Configure( "path/to/hibernate.cfg.xml" );
		/// </code>
		/// </remarks>
		public Configuration Configure()
		{
			XmlNode configNode = GetAppConfigConfigurationNode();

			if( configNode != null )
			{
				return Configure( configNode );
			}
			else
			{
				return Configure( GetDefaultConfigurationFilePath() );
			}
		}

		/// <summary>
		/// Configure NHibernate from an <see cref="XmlNode" /> representing the root
		/// <c>&lt;hibernate-configuration&gt;</c> element.
		/// </summary>
		/// <param name="node">Configuration node</param>
		/// <returns>This Configuration object</returns>
		private Configuration Configure( XmlNode node )
		{
			XmlTextReader reader = new XmlTextReader( node.OuterXml, XmlNodeType.Document, null );
			try
			{
				Configure( reader );
				return this;
			}
			finally
			{
				reader.Close();
			}
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
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader( resource );
				return Configure( reader );
			}
			finally
			{
				if( reader != null )
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
		public Configuration Configure( Assembly assembly, string resourceName )
		{
			if( assembly == null || resourceName == null )
			{
				throw new HibernateException( "Could not configure NHibernate.", new ArgumentException( "A null value was passed in.", "assembly or resourceName" ) );
			}

			Stream stream = null;
			try
			{
				stream = assembly.GetManifestResourceStream( resourceName );
				if( stream == null )
				{
					// resource does not exist - throw appropriate exception 
					throw new HibernateException( "A ManifestResourceStream could not be created for the resource " + resourceName + " in Assembly " + assembly.FullName );
				}

				return Configure( new XmlTextReader( stream ) );
			}
			finally
			{
				if( stream != null )
				{
					stream.Close();
				}
			}
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
				validatingReader.Schemas.Add( CfgSchemaCollection );

				doc.Load( validatingReader );
			}
			catch( Exception e )
			{
				log.Error( "Problem parsing configuration", e );
				throw new HibernateException( "problem parsing configuration : " + e );
			}
			finally
			{
				if( validatingReader != null )
				{
					validatingReader.Close();
				}
			}

			return DoConfigure( doc );
		}

		// Not ported - configure(org.w3c.dom.Document)

		protected Configuration DoConfigure( XmlDocument doc )
		{
			XmlNamespaceManager cfgNamespaceMgr = CreateXmlNamespaceManager( doc );

			XmlNode sfNode = doc.DocumentElement.SelectSingleNode( "//" + CfgNamespacePrefix + ":session-factory", cfgNamespaceMgr );

			if( sfNode == null )
			{
				throw new MappingException( "<session-factory> element was not found in the configuration file." );
			}

			XmlAttribute nameNode = sfNode.Attributes[ "name" ];
			string name = nameNode == null ? null : nameNode.Value;

			if( name != null )
			{
				properties[ Environment.SessionFactoryName ] = name;
			}
			AddProperties( sfNode, cfgNamespaceMgr );

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
						log.Debug( name + "<-" + rsrc.Value + " in " + assembly.Value );
						AddResource( rsrc.Value, Assembly.Load( assembly.Value ) );
					}
					else if( assembly != null )
					{
						log.Debug( name + "<-" + assembly.Value );
						AddAssembly( assembly.Value );
					}
					else if( file != null )
					{
						log.Debug( name + "<-" + file.Value );
						AddFile( file.Value );
					}
					else
					{
						throw new MappingException( "<mapping> element in configuration specifies no attributes" );
					}
				}
				else if( "jcs-class-cache".Equals( elemname ) || "class-cache".Equals( elemname ) )
				{
					string className = mapElement.Attributes[ "class" ].Value;
					System.Type clazz;
					try
					{
						clazz = ReflectHelper.ClassForName( className );
					}
					catch( TypeLoadException tle )
					{
						throw new MappingException( "Could not find class: " + className, tle );
					}

					XmlAttribute regionNode = mapElement.Attributes[ "region" ];
					string region = ( regionNode == null ) ? className : regionNode.Value;
					ICacheConcurrencyStrategy cache = CacheFactory.CreateCache( mapElement, region, GetRootClassMapping( clazz ).IsMutable );
					SetCacheConcurrencyStrategy( clazz, cache, region );
				}
				else if( "jcs-collection-cache".Equals( elemname ) || "collection-cache".Equals( elemname ) )
				{
					String role = mapElement.Attributes[ "collection" ].Value;
					NHibernate.Mapping.Collection collection = GetCollectionMapping( role );

					if( collection == null )
					{
						throw new MappingException( "Cannot configure cache for unknown collection role " + role );
					}

					XmlAttribute regionNode = mapElement.Attributes[ "region" ];
					string region = ( regionNode == null ) ? role : regionNode.Value;
					ICacheConcurrencyStrategy cache = CacheFactory.CreateCache( mapElement, region, collection.Owner.IsMutable );
					SetCacheConcurrencyStrategy( role, cache, region );
				}
			}

			if( name != null )
			{
				log.Info( "Configured SessionFactory: " + name );
			}
			log.Debug( "properties: " + properties );

			return this;
		}

		internal RootClass GetRootClassMapping( System.Type clazz )
		{
			PersistentClass persistentClass = GetClassMapping( clazz );
			
			if( persistentClass == null )
			{
				throw new MappingException( "Cache specified for unmapped class " + clazz );
			}
			
			RootClass rootClass = persistentClass as RootClass;
			
			if( rootClass == null )
			{
				throw new MappingException(
					"You may only specify a cache for root <class> mappings "
						+ "(cache was specified for " + clazz + ")" );
			}

			return rootClass;
		}

		/// <summary>
		/// Set up a cache for an entity class
		/// </summary>
		public Configuration SetCacheConcurrencyStrategy( System.Type clazz, ICacheConcurrencyStrategy concurrencyStrategy )
		{
			SetCacheConcurrencyStrategy( clazz, concurrencyStrategy, clazz.FullName );
			return this;
		}

		internal void SetCacheConcurrencyStrategy( System.Type clazz, ICacheConcurrencyStrategy concurrencyStrategy, string region )
		{
			RootClass rootClass = GetRootClassMapping( clazz );
			rootClass.Cache = concurrencyStrategy;
			caches[ rootClass.MappedClass.FullName ] = concurrencyStrategy;
		}

		/// <summary>
		/// Set up a cache for a collection role
		/// </summary>
		public Configuration SetCacheConcurrencyStrategy( string collectionRole, ICacheConcurrencyStrategy concurrencyStrategy )
		{
			SetCacheConcurrencyStrategy( collectionRole, concurrencyStrategy, collectionRole );
			return this;
		}

		internal void SetCacheConcurrencyStrategy( string collectionRole, ICacheConcurrencyStrategy concurrencyStrategy, string region )
		{
			// TODO: this is ported from H2.1.8, but the implementation looks
			// very strange to me - region parameter is ignored,
			// collection.Role is used instead of collectionRole, etc.
			NHibernate.Mapping.Collection collection = GetCollectionMapping( collectionRole );
			collection.Cache = concurrencyStrategy;
			if( caches.Contains( collection.Role ) )
			{
				throw new MappingException( "duplicate cache region " + collection.Role );
			}
			caches.Add( collection.Role, concurrencyStrategy );
		}

		protected void ConfigureCaches( Settings settings )
		{
			//TODO: this is actually broken, I guess, since changing the
			//      cache provider property and rebuilding the SessionFactory
			//      will affect existing SessionFactory!

			log.Info( "instantiating and configuring caches" );

			// needed here because caches are built directly below.  This is fixed in H3.
			settings.CacheProvider.Start( properties );

			string prefix = properties[ Environment.CacheRegionPrefix ] as string;

			foreach( DictionaryEntry de in caches )
			{
				string name = ( string ) de.Key;

				if( prefix != null )
				{
					name = prefix + "." + name;
				}

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
				strategy.MinimalPuts = settings.IsMinimalPutsEnabled;
			}

			caches.Clear();
		}

		/// <summary>
		/// Get the query language imports
		/// </summary>
		/// <returns></returns>
		public IDictionary Imports
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
			return SettingsFactory.BuildSettings( properties );
		}

		/// <summary>
		/// The named SQL queries
		/// </summary>
		public IDictionary NamedSQLQueries
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
		/// <param name="namingStrategy">the NamingStrategy to set</param>
		/// <returns></returns>
		public Configuration SetNamingStrategy( INamingStrategy namingStrategy )
		{
			this.namingStrategy = namingStrategy;
			return this;
		}

		public IDictionary SqlResultSetMappings
		{
			get { return sqlResultSetMappings; }
		}

		#region NHibernate-Specific Members

		/// <summary>
		/// Load and validate the mappings in the <see cref="XmlTextReader" /> against
		/// the nhibernate-mapping-2.0 schema, without adding them to the configuration.
		/// </summary>
		/// <remarks>
		/// This method is made public to be usable from the unit tests. It is not intended
		/// to be called by end users.
		/// </remarks>
		/// <param name="hbmReader">The XmlReader that contains the mapping.</param>
		/// <returns>Validated XmlDocument built from the XmlReader.</returns>
		public XmlDocument LoadMappingDocument( XmlTextReader hbmReader, string name )
		{
			XmlValidatingReader validatingReader = new XmlValidatingReader( hbmReader );
			
			Debug.Assert( currentDocumentName == null );
			currentDocumentName = name;

			try
			{
				XmlDocument hbmDocument = new XmlDocument();

				validatingReader.ValidationEventHandler += new ValidationEventHandler( ValidationHandler );
				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.Schemas.Add( MappingSchemaCollection );

				hbmDocument.Load( validatingReader );
				return hbmDocument;
			}
			finally
			{
				currentDocumentName = null;
				validatingReader.Close();
			}
		}

		/// <summary>
		/// Adds the Mappings in the <see cref="XmlTextReader"/> after validating it
		/// against the nhibernate-mapping-2.0 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlTextReader that contains the mapping.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader( XmlTextReader hbmReader )
		{
			return AddXmlReader( hbmReader, null );
		}
		
		/// <summary>
		/// Adds the Mappings in the <see cref="XmlTextReader"/> after validating it
		/// against the nhibernate-mapping-2.0 schema.
		/// </summary>
		/// <param name="hbmReader">The XmlTextReader that contains the mapping.</param>
		/// <param name="name">The name of the document to use for error reporting. May be <c>null</c>.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlReader( XmlTextReader hbmReader, string name )
		{
			XmlDocument document = LoadMappingDocument( hbmReader, name );
			AddValidatedDocument( document, name );
			return this;
		}

		private void ValidationHandler( object o, ValidationEventArgs args )
		{
			string message =
				string.Format(
					"{0}({1},{2}): XML validation error: {3}",
					currentDocumentName,
					args.Exception.LineNumber,
					args.Exception.LinePosition,
					args.Exception.Message );
			LogAndThrow( new MappingException( message, args.Exception ) );
		}

		protected XmlNamespaceManager CreateXmlNamespaceManager( XmlDocument doc )
		{
			XmlNamespaceManager cfgNamespaceMgr = new XmlNamespaceManager( doc.NameTable );
			// note that the prefix has absolutely nothing to do with what the user
			// selects as their prefix in the document.  It is the prefix we use to 
			// build the XPath and the nsmgr takes care of translating our prefix into
			// the user defined prefix...
			cfgNamespaceMgr.AddNamespace( CfgNamespacePrefix, Configuration.CfgSchemaXMLNS );
			return cfgNamespaceMgr;
		}

		private XmlNode GetAppConfigConfigurationNode()
		{
			XmlNode node = ConfigurationSettings.GetConfig( "hibernate-configuration" ) as XmlNode;
			return node;
		}

		private string GetDefaultConfigurationFilePath()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			string binPath = relativeSearchPath == null ? baseDir : Path.Combine( baseDir, relativeSearchPath );
			return Path.Combine( binPath, "hibernate.cfg.xml" );
		}

		#endregion
	}
}