using System;
using MappingException = NHibernate.MappingException;

using MultiHashMap = System.Collections.Hashtable;
using MultiMap = System.Collections.Hashtable;
using Document = System.Xml.XmlDocument;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
{
	
	
	/// <summary> </summary>
	public class CodeGenerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		[STAThread]
		public static void  Main(System.String[] args)
		{
			System.IO.File.Delete("error-log.txt");
			log4net.Config.DOMConfigurator.Configure(new System.IO.FileInfo("hbm2net.exe.config"));
			
			if (args.Length == 0)
			{
				System.Console.Error.WriteLine("No arguments provided. Nothing to do. Exit.");
				System.Environment.Exit(- 1);
			}
			try
			{
				System.Collections.ArrayList mappingFiles = new System.Collections.ArrayList();
				
				System.String outputDir = null;
				
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
							Document document = new System.Xml.XmlDocument();
							document.Load(args[i].Substring(9));
							globalMetas = MetaAttributeHelper.loadAndMergeMetaMap((System.Xml.XmlElement)(document["codegen"]), null);
							System.Collections.IEnumerator generateElements = document.GetElementsByTagName("generate").GetEnumerator();
							
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
				
				System.Collections.Hashtable classMappings = new System.Collections.Hashtable();
				for (System.Collections.IEnumerator iter = mappingFiles.GetEnumerator(); iter.MoveNext(); )
				{
					try
					{
						log.Info(iter.Current.ToString());
						// parse the mapping file
						System.Xml.NameTable nt = new System.Xml.NameTable();
						nt.Add("urn:nhibernate-mapping-2.0");
						Document document = new System.Xml.XmlDocument(nt);
						document.Load((System.String) iter.Current);
					
						Element rootElement = document["hibernate-mapping"];

						if (rootElement == null)
							continue;
					
						System.Xml.XmlAttribute a = rootElement.Attributes["package"];
						System.String pkg = null;
						if (a != null)
						{
							pkg = a.Value;
						}
						MappingElement me = new MappingElement(rootElement, null);
						System.Collections.IEnumerator classElements = rootElement.GetElementsByTagName("class").GetEnumerator();
						MultiMap mm = MetaAttributeHelper.loadAndMergeMetaMap(rootElement, globalMetas);
						handleClass(pkg, me, classMappings, classElements, mm, false);
					
						classElements = rootElement.GetElementsByTagName("subclass").GetEnumerator();
						handleClass(pkg, me, classMappings, classElements, mm, true);
					
						classElements = rootElement.GetElementsByTagName("joined-subclass").GetEnumerator();
						handleClass(pkg, me, classMappings, classElements, mm, true);
					}
					catch(Exception exc)
					{
						log.Error("Error in map",exc);
					}
				}
				// generate source files
				for (System.Collections.IEnumerator iterator = generators.GetEnumerator(); iterator.MoveNext(); )
				{
					Generator g = (Generator) iterator.Current;
					g.BaseDirName = outputDir;
					g.generate(classMappings);
				}
			}
			catch (System.Exception e)
			{
				SupportClass.WriteStackTrace(e, Console.Error);
			}
		}
		
		private static void  handleClass(System.String classPackage, MappingElement me, System.Collections.Hashtable classMappings, System.Collections.IEnumerator classElements, MultiMap mm, bool extendz)
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
					System.String ex = (clazz.Attributes["extends"] == null?null:clazz.Attributes["extends"].Value);
					if ((System.Object) ex == null)
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