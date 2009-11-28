using System.Xml;
using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Util;
using System.Collections.Generic;
using Iesi.Collections.Generic;

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
			private readonly string entityName;
			private readonly string extendsEntityName;
			private readonly AssemblyQualifiedTypeName fullExtends;
			private readonly AssemblyQualifiedTypeName fullClassName;
			private readonly int hashCode;

			public ClassEntry(string extends, string className, string entityName, string assembly, string @namespace)
			{
				fullExtends = string.IsNullOrEmpty(extends) ? null : TypeNameParser.Parse(extends, @namespace, assembly);
				fullClassName = string.IsNullOrEmpty(className) ? null : TypeNameParser.Parse(className, @namespace, assembly);
				this.entityName = entityName;
				extendsEntityName = string.IsNullOrEmpty(extends) ? null : extends;
				unchecked
				{
					hashCode = (entityName != null ? entityName.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (fullExtends != null ? fullExtends.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (fullClassName != null ? fullClassName.GetHashCode() : 0);
				}
			}

			public AssemblyQualifiedTypeName FullExtends
			{
				get { return fullExtends; }
			}

			public AssemblyQualifiedTypeName FullClassName
			{
				get { return fullClassName; }
			}

			public string EntityName
			{
				get { return entityName; }
			}

			public string ExtendsEntityName
			{
				get { return extendsEntityName; }
			}

			public override bool Equals(object obj)
			{
				ClassEntry that = obj as ClassEntry;
				return Equals(that);
			}

			public bool Equals(ClassEntry obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				return Equals(obj.entityName, entityName) && Equals(obj.fullExtends, fullExtends) && Equals(obj.fullClassName, fullClassName);
			}

			public override int GetHashCode()
			{
				return hashCode;
			}
		}

		/// <summary>
		/// Returns a collection of <see cref="ClassEntry" /> containing
		/// information about all classes in this stream.
		/// </summary>
		/// <param name="document">A validated <see cref="XmlDocument"/> representing
		/// a mapping file.</param>
		public static ICollection<ClassEntry> GetClassEntries(XmlDocument document)
		{
			// TODO this should be extracted into a utility method since there's similar
			// code in Configuration
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
			nsmgr.AddNamespace(HbmConstants.nsPrefix, Binder.MappingSchemaXMLNS);

			// Since the document is validated, no error checking is done in this method.
			HashedSet<ClassEntry> classEntries = new HashedSet<ClassEntry>();

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

			if (classNodes != null)
			{
				foreach (XmlNode classNode in classNodes)
				{
					string name = XmlHelper.GetAttributeValue(classNode, "name");
					string extends = XmlHelper.GetAttributeValue(classNode, "extends");
					string entityName = XmlHelper.GetAttributeValue(classNode, "entity-name");
					classEntries.Add(new ClassEntry(extends, name, entityName, assembly, @namespace));
				}
			}

			return classEntries;
		}
	}
}
