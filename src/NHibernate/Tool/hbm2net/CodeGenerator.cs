/*
* $Id$
*/
using System;
using MappingException = NHibernate.MappingException;

using MultiHashMap = System.Collections.Hashtable;
using MultiMap = System.Collections.Hashtable;
using Document = System.Xml.XmlDocument;
using Element = System.Xml.XmlElement;

namespace NHibernate.tool.hbm2net
{
	
	
	/// <summary> </summary>
	public class CodeGenerator
	{
	
		[STAThread]
		public static void  Main(System.String[] args)
		{
			log4net.Config.DOMConfigurator.Configure();
			
			if (args.Length == 0)
			{
				System.Console.Error.WriteLine("No arguments provided. Nothing to do. Exit.");
				System.Environment.Exit(- 1);
			}
			try
			{
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				System.Collections.ArrayList mappingFiles = new System.Collections.ArrayList();
				
				//SAXBuilder builder = new SAXBuilder(true);
				//builder.setEntityResolver(new DTDEntityResolver());
				
				//UPGRADE_TODO: Interface 'org.xml.sax.ErrorHandler' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1095"'
				//builder.setErrorHandler(new AnonymousClassErrorHandler());
				
				System.String outputDir = null;
				
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
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
							//builder.setValidation(false);
							Document document = new System.Xml.XmlDocument();
							document.Load(args[i].Substring(9));
							globalMetas = MetaAttributeHelper.loadAndMergeMetaMap((System.Xml.XmlElement)(document["codegen"]), null);
							System.Collections.IEnumerator generateElements = document.GetElementsByTagName("generate").GetEnumerator();
							
							//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
							while (generateElements.MoveNext())
							{
								//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
								//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
								generators.Add(new Generator((Element) generateElements.Current));
							}
							//builder.setValidation(true);
						}
						else if (args[i].StartsWith("--output="))
						{
							outputDir = args[i].Substring(9);
						}
					}
					else
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						mappingFiles.Add(args[i]);
					}
				}
				
				// if no config xml file, add a default generator
				if (generators.Count == 0)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					generators.Add(new Generator());
				}
				
				//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap"'
				System.Collections.Hashtable classMappings = new System.Collections.Hashtable();
				//builder.setValidation(true);
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator iter = mappingFiles.GetEnumerator(); iter.MoveNext(); )
				{
					// parse the mapping file
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					System.Xml.NameTable nt = new System.Xml.NameTable();
					nt.Add("urn:nhibernate-mapping-2.0");
					Document document = new System.Xml.XmlDocument(nt);
					document.Load((System.String) iter.Current);
					
					Element rootElement = document["hibernate-mapping"];
					
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
				
				// generate source files
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator iterator = generators.GetEnumerator(); iterator.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
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
		
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap"'
		private static void  handleClass(System.String classPackage, MappingElement me, System.Collections.Hashtable classMappings, System.Collections.IEnumerator classElements, MultiMap mm, bool extendz)
		{
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			while (classElements.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				Element clazz = (Element) classElements.Current;
				
				if (!extendz)
				{
					ClassMapping cmap = new ClassMapping(classPackage, clazz, me, mm);
					SupportClass.PutElement(classMappings, cmap.FullyQualifiedName, cmap);
				}
				else
				{
					System.String ex = clazz["extends"].Value;
					if ((System.Object) ex == null)
					{
						throw new MappingException("Missing extends attribute on <" + clazz.LocalName + " name=" + clazz["name"].Value + ">");
					}
					//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.this' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMapget_javalangObject"'
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
		static CodeGenerator()
		{
			//log = LogFactory.getLog(typeof(CodeGenerator));
		}
	}
}