using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using log4net;
using Iesi.Collections;
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
	public sealed class Configuration : IMapping
	{
		private Hashtable classes;
		private Hashtable imports; 
		private Hashtable collections;
		private Hashtable tables;
		private Hashtable namedQueries;
		private Hashtable namedSqlQueries;
		private ArrayList secondPasses;
		private ArrayList propertyReferences;
		private IInterceptor interceptor;
		private IDictionary properties;
		private IDictionary caches;

		private INamingStrategy namingStrategy = DefaultNamingStrategy.Instance;

		private XmlSchemaCollection mappingSchemaCollection;
		private XmlSchemaCollection cfgSchemaCollection;

		private static readonly ILog log = LogManager.GetLogger( typeof( Configuration ) );
		private static readonly IInterceptor emptyInterceptor = new EmptyInterceptor();

		private class Mapping : IMapping
		{
			private Hashtable classes;

			public Mapping( Hashtable classes )
			{
				this.classes = classes;
			}

			public Hashtable Classes
			{
				set { classes = value; }
			}

			#region IMapping Members

			public IType GetIdentifierType( System.Type persistentClass )
			{
				return ( (PersistentClass) classes[ persistentClass ] ).Identifier.Type;
			}

			public string GetIdentifierPropertyName( System.Type persistentClass )
			{
				return ( (PersistentClass) classes[ persistentClass ] ).IdentifierProperty.Name;
			}

			public IType GetPropertyType( System.Type persistentClass, string propertyName )
			{
				return ( (PersistentClass) classes[ persistentClass ] ).GetProperty( propertyName ).Type;
			}

			#endregion
		}

		private Mapping mapping;

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

		/// <summary>
		/// Clear the internal state of the <see cref="Configuration"/> object.
		/// </summary>
		//protected void Reset()
		private void Reset()
		{
			classes = new Hashtable();
			imports = new Hashtable();
			collections = new Hashtable();
			tables = new Hashtable();
			namedQueries = new Hashtable();
			namedSqlQueries = new Hashtable();
			secondPasses = new ArrayList();
			propertyReferences = new ArrayList();
			interceptor = emptyInterceptor;
			caches = new Hashtable();
			mapping = new Mapping( classes );
		}

		/// <summary>
		/// Create a new Configuration object.
		/// </summary>
		public Configuration()
		{
			Environment.Configure();
			Reset();
			XmlNode confNode = Environment.ConfigurationNode;
			if ( confNode != null ) 
			{
				properties = new Hashtable();
				Environment.SetProperties(properties);
				XmlTextReader reader = new XmlTextReader(new StringReader(confNode.OuterXml));
				Configure(reader);
				reader.Close();
			} 
			else 
			{
				properties = Environment.Properties;
			}

		}

		/// <summary></summary>
		/// <remarks>Allocate on first use as we are expensive in time/space</remarks>
		private XmlSchemaCollection MappingSchemaCollection
		{
			get 
			{
				if ( mappingSchemaCollection == null )
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
				if ( cfgSchemaCollection == null )
				{
					cfgSchemaCollection = new XmlSchemaCollection();
					cfgSchemaCollection.Add( XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream( CfgSchemaResource ), null ) );
				}
				return cfgSchemaCollection;
			}
			set { cfgSchemaCollection = value; }
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
		public NHibernate.Mapping.Collection GetCollectionMapping( string role )
		{
			return ( NHibernate.Mapping.Collection ) collections[ role ];
		}

		/// <summary>
		/// Create a new <c>Mappings</c> to add classes and collection mappings to
		/// </summary>
		/// <returns></returns>
		public Mappings CreateMappings()
		{
			return new Mappings( classes, collections, tables, namedQueries, namedSqlQueries, imports, caches, secondPasses, propertyReferences, namingStrategy );
		}

		/// <summary>
		/// Takes the validated XmlDocument and has the Binder do its work of
		/// creating Mapping objects from the Mapping Xml.
		/// </summary>
		/// <param name="doc">The <b>validated</b> XmlDocument that contains the Mappings.</param>
		private void AddValidatedDocument( XmlDocument doc )
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
					log.Info( "Found mapping documents in assembly: " + fileName );
					Stream resourceStream = null;
					try
					{
						resourceStream = assembly.GetManifestResourceStream( fileName );
						AddInputStream( resourceStream );
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
					finally
					{
						if( resourceStream!=null ) 
						{
							resourceStream.Close();
						}

					}
				}
			}

			return this;
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
			if( rsrc==null )
			{
				throw new MappingException( "Resource: " + fileName + " not found" );
			}

			try 
			{
				return AddInputStream( rsrc );
			}
			finally
			{
				rsrc.Close();	
			}
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
				using( MemoryStream ms = new MemoryStream() )
				{
					doc.Save( ms );
					ms.Position = 0;
					AddInputStream( ms );
				}
				return this;
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from XML document", e );
				throw new MappingException( e );
			}
		}

		/// <summary>
		/// Adds the Xml Mappings from the Stream.
		/// </summary>
		/// <param name="xmlInputStream">The Stream to read Xml from.</param>
		/// <returns>This Configuration object.</returns>
		/// <remarks>
		/// The <see cref="Stream"/> passed in through the parameter <c>xmlInputStream</c>
		/// is not <b>guaranteed</b> to be cleaned up by this method.  It is the callers responsiblity to
		/// ensure that the <c>xmlInputStream</c> is properly handled when this method
		/// completes.
		/// </remarks>
		public Configuration AddInputStream( Stream xmlInputStream )
		{
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader( xmlInputStream );
				AddXmlReader( textReader );
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
			finally
			{
				if( textReader!=null ) 
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
			log.Info( "mapping resource: " + path );
			Stream rsrc = assembly.GetManifestResourceStream( path );
			if( rsrc==null )
			{
				throw new MappingException( "Resource: " + path + " not found" );
			}

			try 
			{
				return AddInputStream( rsrc );
			}
			finally
			{
				if( rsrc!=null ) 
				{
					rsrc.Close();
				}
			}
		}

		/// <summary>
		/// Adds the Mappings contained in the file specified.
		/// </summary>
		/// <param name="xmlFile">The name of the file (url or file system) that contains the Xml.</param>
		/// <returns>This Configuration object.</returns>
		public Configuration AddXmlFile( string xmlFile )
		{
			log.Debug( "Mapping file: " + xmlFile );
			XmlTextReader textReader = null;
			try
			{
				textReader = new XmlTextReader( xmlFile );
				AddXmlReader( textReader );
			}
			catch( Exception e )
			{
				log.Error( "Could not configure datastore from file: " + xmlFile, e );
				throw new MappingException( e );
			}
			finally
			{
				if( textReader!=null )
				{
					textReader.Close();
				}
			}
			return this;
		}

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
		public XmlDocument LoadMappingDocument( XmlTextReader hbmReader )
		{
			XmlValidatingReader validatingReader = new XmlValidatingReader( hbmReader );

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
			AddValidatedDocument( LoadMappingDocument( hbmReader ) );
			return this;
		}

		private static void ValidationHandler( object o, ValidationEventArgs args )
		{
			throw args.Exception;
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
						script.Add( fk.SqlDropString( dialect, (string) properties[ Environment.DefaultSchema ] ) );
					}
				}
			}

			foreach( Table table in TableMappings )
			{
				script.Add( table.SqlDropString( dialect, (string) properties[ Environment.DefaultSchema ] ) );
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
				script.Add( table.SqlCreateString( dialect, this, (string) properties[ Environment.DefaultSchema ] ) );
			}

			foreach( Table table in TableMappings )
			{
				foreach( Index index in table.IndexCollection )
				{
					script.Add( index.SqlCreateString( dialect, this, (string) properties[ Environment.DefaultSchema ] ) );
				}
				if( dialect.HasAlterTable )
				{
					foreach( ForeignKey fk in table.ForeignKeyCollection )
					{
						script.Add( fk.SqlCreateString( dialect, this, (string) properties[ Environment.DefaultSchema ] ) );
					}
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

			log.Info("processing one-to-one association property references");

			foreach( Mappings.UniquePropertyReference upr in propertyReferences )
			{
				PersistentClass clazz = GetClassMapping( upr.ReferencedClass );
				if ( clazz == null ) throw new MappingException( "property-ref to unmapped class: " + upr.ReferencedClass.Name );
				bool found = false;

				foreach( NHibernate.Mapping.Property prop in clazz.PropertyCollection )
				{
					if ( upr.PropertyName.Equals( prop.Name )  )
					{
						( (SimpleValue) prop.Value ).IsUnique = true;
						found = true;
						break;
					}
				}
				if (!found) 
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
				if ( !done.Contains( fk ) )
				{
					done.Add( fk );
					if( log.IsDebugEnabled )
					{
						log.Debug( "resolving reference to class: " + fk.ReferencedClass.Name );
					}
					PersistentClass referencedClass = ( PersistentClass ) classes[ fk.ReferencedClass ];
					if( referencedClass == null )
					{
						string message = "An association from the table {0} refers to an unmapped class: {1}";

						if( log.IsErrorEnabled )
						{
							log.Error( message );
						}

						throw new MappingException( string.Format( message, fk.Table.Name, fk.ReferencedClass.Name ) );
					}
					if ( referencedClass.IsJoinedSubclass )
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
			Hashtable copy = new Hashtable( properties );
			Settings settings = BuildSettings();
			ConfigureCaches( settings );

			// Ok, don't need schemas anymore, so free them
			MappingSchemaCollection = null;
			CfgSchemaCollection = null;

			return new SessionFactoryImpl( this, settings );
		}

		private void Validate()
		{
			foreach( PersistentClass clazz in classes.Values )
			{
				clazz.Validate( mapping );
			}

			foreach( NHibernate.Mapping.Collection col in collections.Values )
			{
				col.Validate( mapping );
			}
		}

		/// <summary>
		/// Builds an object-oriented view of the settings.
		/// </summary>
		/// <returns>A <see cref="Settings"/> object initialized from the settings properties.</returns>
		//protected Settings BuildSettings()
		private Settings BuildSettings()
		{
			return SettingsFactory.BuildSettings( properties );
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
		/// Set a custom naming strategy
		/// </summary>
		/// <param name="namingStrategy">the NamingStrategy to set</param>
		/// <returns></returns>
		public Configuration SetNamingStrategy( INamingStrategy namingStrategy )
		{
			this.namingStrategy = namingStrategy;
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

		private void AddProperties( XmlNode parent, XmlNamespaceManager CfgNamespaceMgr )
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
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			string binPath = relativeSearchPath == null ? baseDir : Path.Combine( baseDir, relativeSearchPath );
			string filePath = Path.Combine( binPath, "hibernate.cfg.xml" );
			return Configure( filePath );
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
				if( reader!=null )
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
				if( stream==null )
				{
					// resource does not exist - throw appropriate exception 
					throw new HibernateException( "A ManifestResourceStream could not be created for the resource " + resourceName + " in Assembly " + assembly.FullName );
				}

				return Configure( new XmlTextReader( stream ) );
			}
			finally
			{
				if( stream!=null )
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
			XmlNamespaceManager cfgNamespaceMgr;

			try
			{
				validatingReader = new XmlValidatingReader( reader );
				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.Schemas.Add( CfgSchemaCollection );

				doc.Load( validatingReader );
				cfgNamespaceMgr = new XmlNamespaceManager( doc.NameTable );
				// note that the prefix has absolutely nothing to do with what the user
				// selects as their prefix in the document.  It is the prefix we use to 
				// build the XPath and the nsmgr takes care of translating our prefix into
				// the user defined prefix...
				cfgNamespaceMgr.AddNamespace( CfgNamespacePrefix, Configuration.CfgSchemaXMLNS );
			}
			catch( Exception e )
			{
				log.Error( "Problem parsing configuration", e );
				throw new HibernateException( "problem parsing configuration : " + e );
			}
			finally
			{
				if( validatingReader!=null ) 
				{
					validatingReader.Close();
				}
			}

			XmlNode sfNode = doc.DocumentElement.SelectSingleNode( "//" + CfgNamespacePrefix + ":session-factory", cfgNamespaceMgr );
			XmlAttribute name = sfNode.Attributes[ "name" ];
			if( name != null )
			{
				properties[ Environment.SessionFactoryName ] = name.Value;
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

			if( name!=null ) 
			{
				log.Info( "Configured SessionFactory: " + name.Value );
			}
			log.Debug( "properties: " + properties );

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="settings"></param>
		//protected void ConfigureCaches( Settings settings )
		private void ConfigureCaches( Settings settings )
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

		// HACK: SHould really implement IMapping, but simpler than dealing with the cascades

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectClass"></param>
		/// <returns></returns>
		public string GetIdentifierPropertyName( System.Type objectClass )
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType GetPropertyType( System.Type persistentClass, string propertyName )
		{
			return null;
		}

	}
}