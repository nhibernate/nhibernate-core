using System.Collections;
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
				_fullClassName = className == null ? null : TypeNameParser.Parse(className, @namespace, assembly);
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
		/// <param name="document">A validated <see cref="XmlDocument"/> representing
		/// a mapping file.</param>
		public static ICollection GetClassEntries(XmlDocument document)
		{
			// TODO this should be extracted into a utility method since there's similar
			// code in Configuration
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
			nsmgr.AddNamespace(HbmConstants.nsPrefix, Configuration.MappingSchemaXMLNS);

			// Since the document is validated, no error checking is done in this method.
			HashedSet classEntries = new HashedSet();

			XmlNode root = document.DocumentElement;

			string assembly = XmlHelper.GetAttributeValue(root, "assembly");
			string @namespace = XmlHelper.GetAttributeValue(root, "namespace");

			XmlNodeList classNodes = document.SelectNodes(
				"//" + HbmConstants.nsClass +
				"|//" + HbmConstants.nsSubclass +
				"|//" + HbmConstants.nsJoinedSubclass +
				"|//" + HbmConstants.nsUnionSubclass,
				nsmgr
				);

			foreach (XmlNode classNode in classNodes)
			{
				string name = XmlHelper.GetAttributeValue(classNode, "name");
				string extends = XmlHelper.GetAttributeValue(classNode, "extends");
				ClassEntry ce = new ClassEntry(extends, name, assembly, @namespace);
				classEntries.Add(ce);
			}

			return classEntries;
		}
	}
}
