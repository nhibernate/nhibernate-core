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
	
		[STAThread]
		public static void  Main(String[] args)
		{
			nsmgr = new XmlNamespaceManager(new NameTable());
			nsmgr.AddNamespace("urn", "urn:nhibernate-mapping-2.0");
			
			File.Delete("error-log.txt");
			DOMConfigurator.Configure(new FileInfo("NHibernate.Tool.hbm2net.exe.config"));
			
			if (args.Length == 0)
			{
				Console.Error.WriteLine("No arguments provided. Nothing to do. Exit.");
				Environment.Exit(- 1);
			}
			try
			{
				ArrayList mappingFiles = new ArrayList();
				
				String outputDir = null;
				
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
						String pkg = null;
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
				// generate source files
				for (IEnumerator iterator = generators.GetEnumerator(); iterator.MoveNext(); )
				{
					Generator g = (Generator) iterator.Current;
					g.BaseDirName = outputDir;
					g.generate(classMappings);
				}
			}
			catch (Exception e)
			{
				SupportClass.WriteStackTrace(e, Console.Error);
			}
		}
		
		private static void  handleClass(String classPackage, MappingElement me, Hashtable classMappings, IEnumerator classElements, MultiMap mm, bool extendz)
		{
			while (classElements.MoveNext())
			{
				Element clazz = (Element) classElements.Current;
				
				if (!extendz)
				{
					ClassMapping cmap = new ClassMapping(classPackage, clazz, me, mm);
					SupportClass.PutElement(classMappings, cmap.FullyQualifiedName, cmap);
				}
				else
				{
					String ex = (clazz.Attributes["extends"] == null?null:clazz.Attributes["extends"].Value);
					if ((Object) ex == null)
					{
						throw new MappingException("Missing extends attribute on <" + clazz.LocalName + " name=" + clazz.Attributes["name"].Value + ">");
					}
					ClassMapping superclass = (ClassMapping) classMappings[ex];
					if (superclass == null)
					{
						throw new MappingException("Cannot extend unmapped class " + ex);
					}
					ClassMapping subclassMapping = new ClassMapping(classPackage, me, superclass.ClassName, superclass, clazz, mm);
					superclass.addSubClass(subclassMapping);
				}
			}
		}
	}
}