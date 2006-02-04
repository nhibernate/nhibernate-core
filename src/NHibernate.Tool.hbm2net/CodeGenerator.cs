using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using MultiHashMap = System.Collections.Hashtable;
using MultiMap = System.Collections.Hashtable;
using Document = System.Xml.XmlDocument;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
{
	
	
	/// <summary> </summary>
	public class CodeGenerator
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		internal static XmlNamespaceManager nsmgr;
	
		private static ArrayList children;
		private static MultiMap allMaps;

		[STAThread]
		public static void  Main(String[] args)
		{
			nsmgr = new XmlNamespaceManager(new NameTable());
			nsmgr.AddNamespace("urn", "urn:nhibernate-mapping-2.0");
			
			children = new ArrayList();
			allMaps = new MultiMap();

			File.Delete("error-log.txt");
            
            // DOMConfigurator is deprecated in the latest log4net, but we are using an earlier
            // version that comes with NVelocity.
			XmlConfigurator.Configure(new FileInfo("NHibernate.Tool.hbm2net.exe.config"));
			
			if (args.Length == 0)
			{
				Console.Error.WriteLine("No arguments provided. Nothing to do. Exit.");
				Environment.Exit(- 1);
			}
//			try
//			{
				ArrayList mappingFiles = new ArrayList();
				
				string outputDir = null;
				
				SupportClass.ListCollectionSupport generators = new SupportClass.ListCollectionSupport();
				
				MultiMap globalMetas = new MultiHashMap();
				// parse command line parameters
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("--"))
					{
						
						if (args[i].StartsWith("--config="))
						{
							// parse config xml file
							Document document = new XmlDocument();
							document.Load(args[i].Substring(9));
							globalMetas = MetaAttributeHelper.loadAndMergeMetaMap((document["codegen"]), null);
							IEnumerator generateElements = document["codegen"].SelectNodes("generate").GetEnumerator();
							
							while (generateElements.MoveNext())
							{
								generators.Add(new Generator((Element) generateElements.Current));
							}
						}
						else if (args[i].StartsWith("--output="))
						{
							outputDir = args[i].Substring(9);
						}
					}
					else if ( args[i].IndexOf("*") > -1 )
					{
						// Handle wildcards
						mappingFiles.AddRange( GetFiles( args[ i ] ) );
					}
					else
					{
						mappingFiles.Add(args[i]);
					}
				}
				
				// if no config xml file, add a default generator
				if (generators.Count == 0)
				{
					generators.Add(new Generator());
				}
				
				Hashtable classMappings = new Hashtable();
				for (IEnumerator iter = mappingFiles.GetEnumerator(); iter.MoveNext(); )
				{
					try
					{
						log.Info(iter.Current.ToString());
						// parse the mapping file
						NameTable nt = new NameTable();
						nt.Add("urn:nhibernate-mapping-2.0");
						Document document = new XmlDocument(nt);
						document.Load((String) iter.Current);
					
						Element rootElement = document["hibernate-mapping"];

						if (rootElement == null)
							continue;
					
						XmlAttribute a = rootElement.Attributes["package"];
						string pkg = null;
						if (a != null)
						{
							pkg = a.Value;
						}
						MappingElement me = new MappingElement(rootElement, null);
						IEnumerator classElements = rootElement.SelectNodes("urn:class", nsmgr).GetEnumerator();
						MultiMap mm = MetaAttributeHelper.loadAndMergeMetaMap(rootElement, globalMetas);
						handleClass(pkg, me, classMappings, classElements, mm, false);
					
						classElements = rootElement.SelectNodes("urn:subclass", nsmgr).GetEnumerator();
						handleClass(pkg, me, classMappings, classElements, mm, true);
					
						classElements = rootElement.SelectNodes("urn:joined-subclass", nsmgr).GetEnumerator();
						handleClass(pkg, me, classMappings, classElements, mm, true);
					}
					catch(Exception exc)
					{
						log.Error("Error in map",exc);
					}
				}

				// Ok, pickup subclasses that we found before their superclasses
				ProcessChildren( classMappings );

				// generate source files
				for (IEnumerator iterator = generators.GetEnumerator(); iterator.MoveNext(); )
				{
					Generator g = (Generator) iterator.Current;
					g.BaseDirName = outputDir;
					g.generate(classMappings);
				}
//			}
//			catch (Exception e)
//			{
//				SupportClass.WriteStackTrace(e, Console.Error);
//			}
		}
		
		private static ICollection GetFiles( string fileSpec )
		{
			int posn = fileSpec.LastIndexOf( '\\' );
			string path = fileSpec.Substring( 0, posn );
			string names = fileSpec.Substring( posn + 1 );
			DirectoryInfo di = new DirectoryInfo( path );

			FileInfo[] files = di.GetFiles( names );
			
			ArrayList fileNames = new ArrayList( files.Length );

			foreach ( FileInfo file in files )
			{
				fileNames.Add( file.FullName );
			}

			return fileNames;
		}

		private static void  handleClass(string classPackage, MappingElement me, Hashtable classMappings, IEnumerator classElements, MultiMap mm, bool extendz)
		{
			while (classElements.MoveNext())
			{
				Element clazz = (Element) classElements.Current;
				
				if (!extendz)
				{
					ClassMapping cmap = new ClassMapping(classPackage, clazz, me, mm);
					SupportClass.PutElement(classMappings, cmap.FullyQualifiedName, cmap);
					SupportClass.PutElement(allMaps, cmap.FullyQualifiedName, cmap);
				}
				else
				{
					string ex = clazz.Attributes["extends"] == null ? null : clazz.Attributes["extends"].Value;
					if ((object) ex == null)
					{
						throw new MappingException("Missing extends attribute on <" + clazz.LocalName + " name=" + clazz.Attributes["name"].Value + ">");
					}
					ClassMapping superclass = (ClassMapping) allMaps[ex];
					if (superclass == null)
					{
						// Haven't seen the superclass yet, so record this and process at the end
						SubclassMapping orphan = new SubclassMapping(classPackage, me, ex, clazz, mm);
						children.Add( orphan );
					}
					else
					{
						ClassMapping subclassMapping = new ClassMapping(classPackage, me, superclass.ClassName, superclass, clazz, mm);
						superclass.addSubClass(subclassMapping);
						SupportClass.PutElement(allMaps, subclassMapping.FullyQualifiedName, subclassMapping);
					}
				}
			}
		}

		/// <summary>
		/// Try to locate superclasses for any orphans we have
		/// </summary>
		private static void ProcessChildren( Hashtable classMappings )
		{
			while ( FindParents( classMappings ) ) { }

			foreach( SubclassMapping child in children )
			{
				if ( child.Orphaned )
				{
					// Log that we had an orphan
					log.Warn( string.Format( "Cannot extend {0} child of unmapped class {1} ", child.Name, child.SuperClass ) );
				}
			}
		}

		/// <summary>
		/// Find parents for any orphans
		/// </summary>
		/// <returns></returns>
		private static bool FindParents( Hashtable classMappings )
		{
			if ( children.Count == 0 )
			{
				// No parents to find
				return false;
			}
			else
			{
				bool found = false;

				foreach( SubclassMapping child in children )
				{
					if ( child.Orphaned )
					{
						ClassMapping superclass = (ClassMapping) allMaps[ child.SuperClass ];
						if (superclass != null)
						{
							ClassMapping subclassMapping = new ClassMapping(child.ClassPackage, child.MappingElement, superclass.ClassName, superclass, child.Clazz, child.MultiMap);
							superclass.addSubClass(subclassMapping);
							// NB Can't remove it from the iterator, so record that we've found the parent.
							child.Orphaned = false;
							found = true;
						}
					}
				}

				// Tell them if we found any
				return found;
			}
		}
	}
}