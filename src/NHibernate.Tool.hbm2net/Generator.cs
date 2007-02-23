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

		public virtual string BaseDirName
		{
			get { return baseDirName; }

			set
			{
				if ((Object) value != null)
				{
					this.baseDirName = value;
				}
			}
		}

		private string rendererClass = "NHibernate.Tool.hbm2net.VelocityRenderer";
		private string baseDirName = "generated";
		private string packageName = null;
		private string suffix;
		private string prefix;
		private string extension = "cs";
		private bool lowerFirstLetter = false;
		private DirectoryInfo workingDirectory;
		public NameValueCollection params_Renamed = new NameValueCollection();

		/// <summary> Constructs a new Generator using the defaults.</summary>
		public Generator(DirectoryInfo workingDirectory)
		{
			this.workingDirectory = workingDirectory;
			this.suffix = string.Empty;
			this.prefix = string.Empty;
		}

		/// <summary> Constructs a new Generator, configured from XML.</summary>
		public Generator(DirectoryInfo workingDirectory, Element generateElement) : this(workingDirectory)
		{
//			InitBlock();			
			string value_Renamed = null;

			// set rendererClass field
			if (
				(Object)
				(this.rendererClass =
				 (generateElement.Attributes["renderer"] == null ? null : generateElement.Attributes["renderer"].Value)) == null)
			{
				throw new Exception("attribute renderer is required.");
			}

			// set dirName field
			if (
				(Object)
				(value_Renamed = (generateElement.Attributes["dir"] == null ? null : generateElement.Attributes["dir"].Value)) !=
				null)
			{
				this.baseDirName = value_Renamed;
			}

			// set packageName field
			this.packageName = (generateElement.Attributes["package"] == null
			                    	? null : generateElement.Attributes["package"].Value);

			// set prefix
			if (
				(Object)
				(value_Renamed = (generateElement.Attributes["prefix"] == null ? null : generateElement.Attributes["prefix"].Value)) !=
				null)
			{
				this.prefix = value_Renamed;
			}

			// set suffix
			if (
				(Object)
				(value_Renamed = (generateElement.Attributes["suffix"] == null ? null : generateElement.Attributes["suffix"].Value)) !=
				null)
			{
				this.suffix = value_Renamed;
			}

			// set extension
			if (
				(Object)
				(value_Renamed =
				 (generateElement.Attributes["extension"] == null ? null : generateElement.Attributes["extension"].Value)) != null)
			{
				this.extension = value_Renamed;
			}

			// set lowerFirstLetter
			value_Renamed = (generateElement.Attributes["lowerFirstLetter"] == null
			                 	? null : generateElement.Attributes["lowerFirstLetter"].Value);
			try
			{
				this.lowerFirstLetter = Boolean.Parse(value_Renamed);
			}
			catch
			{
			}

			IEnumerator iter = generateElement.SelectNodes("param").GetEnumerator();
			while (iter.MoveNext())
			{
				Element childNode = (Element) iter.Current;
				params_Renamed[childNode.Attributes["name"].Value] = childNode.InnerText;
			}
		}

		/// <summary> </summary>
		public virtual void generate(IDictionary classMappingsCol)
		{
			log.Info("Generating " + classMappingsCol.Count + " in " + BaseDirName);
			Renderer renderer = (Renderer) SupportClass.CreateNewInstance(System.Type.GetType(this.rendererClass));

			//Configure renderer
			renderer.configure(workingDirectory, params_Renamed);

			//Running through actual classes
			for (IEnumerator classMappings = classMappingsCol.Values.GetEnumerator(); classMappings.MoveNext();)
			{
				ClassMapping classMapping = (ClassMapping) classMappings.Current;
				writeRecur(classMapping, classMappingsCol, renderer);
			}
			//Running through components
			for (IEnumerator cmpMappings = ClassMapping.Components; cmpMappings.MoveNext();)
			{
				ClassMapping mapping = (ClassMapping) cmpMappings.Current;
				write(mapping, classMappingsCol, renderer);
			}
		}

		private void writeRecur(ClassMapping classMapping, IDictionary class2classmap, Renderer renderer)
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
		private void write(ClassMapping classMapping, IDictionary class2classmap, Renderer renderer)
		{
			string saveToPackage = renderer.getSaveToPackage(classMapping);
			string saveToClassName = renderer.getSaveToClassName(classMapping);
			FileInfo dir = this.getDir(saveToPackage);
			FileInfo file = new FileInfo(Path.Combine(dir.FullName, this.getFileName(saveToClassName)));
			log.Debug("Writing " + file);

			StreamWriter writer = new StreamWriter(new FileStream(file.FullName, FileMode.Create));

			renderer.render(getPackageName(saveToPackage), getName(saveToClassName), classMapping, class2classmap, writer);
			writer.Close();
		}

		/// <summary> </summary>
		private string getFileName(string className)
		{
			return this.getName(className) + "." + this.extension;
		}

		/// <summary> </summary>
		private string getName(string className)
		{
			string name = null;

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

		private string getPackageName(string packageName)
		{
			if ((Object) this.packageName == null)
			{
				return (Object) packageName == null ? string.Empty : packageName;
			}
			else
			{
				return this.packageName;
			}
		}

		/// <summary> </summary>
		private FileInfo getDir(string packageName)
		{
			FileInfo baseDir = new FileInfo(this.baseDirName);
			FileInfo dir = null;

			string p = getPackageName(packageName);

			dir = new FileInfo(Path.Combine(baseDir.FullName, p.Replace(StringHelper.Dot, Path.DirectorySeparatorChar)));

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