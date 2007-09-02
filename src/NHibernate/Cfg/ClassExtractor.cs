using System;
using System.Collections;
using System.IO;
using System.Xml;
using Iesi.Collections;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Extracts the names of classes mapped in a given file,
	/// and the names of the classes they extend.
	/// </summary>
	public class ClassExtractor
	{
		/// <summary>
		/// Holds information about mapped classes found in the <c>hbm.xml</c> files.
		/// </summary>
		public class ClassEntry
		{
			private readonly AssemblyQualifiedTypeName _fullExtends;
			private readonly AssemblyQualifiedTypeName _fullClassName;

			public ClassEntry(string extends, string className, string assembly, string @namespace)
			{
				_fullExtends = extends == null ? null : TypeNameParser.Parse(extends, @namespace, assembly);
				_fullClassName = TypeNameParser.Parse(className, @namespace, assembly);
			}

			public AssemblyQualifiedTypeName FullExtends
			{
				get { return _fullExtends; }
			}

			public AssemblyQualifiedTypeName FullClassName
			{
				get { return _fullClassName; }
			}
		}

		/// <summary>
		/// Returns a collection of <see cref="ClassEntry" /> containing
		/// information about all classes in this stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static ICollection GetClassEntries(Stream stream)
		{
			HashedSet classes = new HashedSet();

			// XmlReader does not implement IDisposable on .NET 1.1 so have to use
			// try/finally instead of using here.
			XmlTextReader xmlReader = new XmlTextReader(stream);

			string assembly = null;
			string @namespace = null;

			try
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType != XmlNodeType.Element)
					{
						continue;
					}

					switch (xmlReader.Name)
					{
						case "hibernate-mapping":
							assembly = xmlReader.MoveToAttribute("assembly") ? xmlReader.Value : null;
							@namespace = xmlReader.MoveToAttribute("namespace") ? xmlReader.Value : null;
							break;
						case "class":
						case "joined-subclass":
						case "subclass":
						case "union-subclass":
							xmlReader.MoveToAttribute("name");
							string className = xmlReader.Value;

							string extends = null;
							if (xmlReader.MoveToAttribute("extends"))
							{
								extends = xmlReader.Value;
							}

							ClassEntry ce = new ClassEntry(extends, className, assembly, @namespace);
							classes.Add(ce);
							break;
					}
				}
			}
			finally
			{
				xmlReader.Close();
			}

			return classes;
		}
	}
}
