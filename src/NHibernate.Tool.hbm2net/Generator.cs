using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using log4net;
using NHibernate.Util;
using Element = System.Xml.XmlElement;

namespace NHibernate.Tool.hbm2net
{
	/// <summary> </summary>
	public class Generator
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private void  InitBlock()
		{
			suffix = string.Empty;
			prefix = string.Empty;
		}
		virtual public String BaseDirName
		{
			get
			{
				return baseDirName;
			}
			
			set
			{
				if ((Object) value != null)
				{
					this.baseDirName = value;
				}
			}
			
		}
		
		private String rendererClass = "NHibernate.Tool.hbm2net.VelocityRenderer";
		private String baseDirName = "generated";
		private String packageName = null;
		private String suffix;
		private String prefix;
		private String extension = "cs";
		private bool lowerFirstLetter = false;
		
		public static NameValueCollection params_Renamed = new NameValueCollection();
		
		/// <summary> Constructs a new Generator using the defaults.</summary>
		public Generator()
		{
			InitBlock();
		}
		
		/// <summary> Constructs a new Generator, configured from XML.</summary>
		public Generator(Element generateElement)
		{
			InitBlock();
			String value_Renamed = null;
			
			// set rendererClass field
			if ((Object) (this.rendererClass = (generateElement.Attributes["renderer"] == null?null:generateElement.Attributes["renderer"].Value)) == null)
			{
				throw new Exception("attribute renderer is required.");
			}
			
			// set dirName field
			if ((Object) (value_Renamed = (generateElement.Attributes["dir"] == null?null:generateElement.Attributes["dir"].Value)) != null)
			{
				this.baseDirName = value_Renamed;
			}
			
			// set packageName field
			this.packageName = (generateElement.Attributes["package"] == null?null:generateElement.Attributes["package"].Value);
			
			// set prefix
			if ((Object) (value_Renamed = (generateElement.Attributes["prefix"] == null?null:generateElement.Attributes["prefix"].Value)) != null)
			{
				this.prefix = value_Renamed;
			}
			
			// set suffix
			if ((Object) (value_Renamed = (generateElement.Attributes["suffix"] == null?null:generateElement.Attributes["suffix"].Value)) != null)
			{
				this.suffix = value_Renamed;
			}
			
			// set extension
			if ((Object) (value_Renamed = (generateElement.Attributes["extension"] == null?null:generateElement.Attributes["extension"].Value)) != null)
			{
				this.extension = value_Renamed;
			}
			
			// set lowerFirstLetter
			value_Renamed = (generateElement.Attributes["lowerFirstLetter"] == null?null:generateElement.Attributes["lowerFirstLetter"].Value);
			try
			{
				this.lowerFirstLetter = Boolean.Parse(value_Renamed);
			}
			catch{}
			
			IEnumerator iter = generateElement.SelectNodes("param").GetEnumerator();
			while (iter.MoveNext())
			{
				Element childNode = (Element) iter.Current;
				params_Renamed[childNode.Attributes["name"].Value] = childNode.InnerText;
			}
		}
		
		/// <summary> </summary>
		public virtual void  generate(IDictionary classMappingsCol)
		{
			log.Info("Generating " + classMappingsCol.Count + " in " + BaseDirName);
			Renderer renderer = (Renderer) SupportClass.CreateNewInstance(System.Type.GetType(this.rendererClass));
			
			/// <summary>Configure renderer </summary>
			renderer.configure(params_Renamed);
			
			/// <summary>Running through actual classes </summary>
			for (IEnumerator classMappings = classMappingsCol.Values.GetEnumerator(); classMappings.MoveNext(); )
			{
				ClassMapping classMapping = (ClassMapping) classMappings.Current;
				writeRecur(classMapping, classMappingsCol, renderer);
			}
			/// <summary>Running through components </summary>
			for (IEnumerator cmpMappings = ClassMapping.Components; cmpMappings.MoveNext(); )
			{
				ClassMapping mapping = (ClassMapping) cmpMappings.Current;
				write(mapping, classMappingsCol, renderer);
			}
		}
		
		private void  writeRecur(ClassMapping classMapping, IDictionary class2classmap, Renderer renderer)
		{
			
			write(classMapping, class2classmap, renderer);
			
			if (!(classMapping.Subclasses.Count == 0))
			{
				IEnumerator it = classMapping.Subclasses.GetEnumerator();
				while (it.MoveNext())
				{
					writeRecur((ClassMapping) it.Current, class2classmap, renderer);
				}
			}
		}
		
		
		/// <summary> </summary>
		private void  write(ClassMapping classMapping, IDictionary class2classmap, Renderer renderer)
		{
			String saveToPackage = renderer.getSaveToPackage(classMapping);
			String saveToClassName = renderer.getSaveToClassName(classMapping);
			FileInfo dir = this.getDir(saveToPackage);
			FileInfo file = new FileInfo(dir.FullName + "\\" + this.getFileName(saveToClassName));
			log.Debug("Writing " + file);
			
			StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.Create));
			
			renderer.render(getPackageName(saveToPackage), getName(saveToClassName), classMapping, class2classmap, writer);
			writer.Close();
		}
		
		/// <summary> </summary>
		private String getFileName(String className)
		{
			return this.getName(className) + "." + this.extension;
		}
		
		/// <summary> </summary>
		private String getName(String className)
		{
			String name = null;
			
			if (this.lowerFirstLetter)
			{
				name = className.Substring(0, (1) - (0)).ToLower() + className.Substring(1, (className.Length) - (1));
			}
			else
			{
				name = className;
			}
			
			return this.prefix + name + this.suffix;
		}
		
		private String getPackageName(String packageName)
		{
			if ((Object) this.packageName == null)
			{
				return (Object) packageName == null?string.Empty:packageName;
			}
			else
			{
				return this.packageName;
			}
		}
		/// <summary> </summary>
		private FileInfo getDir(String packageName)
		{
			FileInfo baseDir = new FileInfo(this.baseDirName);
			FileInfo dir = null;
			
			String p = getPackageName(packageName);
			
			dir = new FileInfo(baseDir.FullName + "\\" + p.Replace(StringHelper.Dot, Path.DirectorySeparatorChar));
			
			// if the directory exists, make sure it is a directory
			bool tmpBool;
			if (File.Exists(dir.FullName))
				tmpBool = true;
			else
				tmpBool = Directory.Exists(dir.FullName);
			if (tmpBool)
			{
				if (!Directory.Exists(dir.FullName))
				{
					throw new Exception("The path: " + dir.FullName + " exists, but is not a directory");
				}
			}
				// else make the directory and any non-existent parent directories
			else
			{
				if (!Directory.CreateDirectory(dir.FullName).Exists)
				{
					throw new Exception("unable to create directory: " + dir.FullName);
				}
			}
			
			return dir;
		}
	}
}