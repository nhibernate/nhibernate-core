using System.Xml;
using System.Linq;
using NHibernate.Util;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;

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
				return Equals(obj as ClassEntry);
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
		public static ICollection<ClassEntry> GetClassEntries(HbmMapping document)
		{
			// Since the document is validated, no error checking is done in this method.
			var classEntries = new HashSet<ClassEntry>();

			string assembly = document.assembly;
			string @namespace = document.@namespace;

			classEntries.UnionWith(document.RootClasses.Select(c=> new ClassEntry(null, c.Name, c.EntityName, assembly, @namespace)));
			classEntries.UnionWith(document.SubClasses.Select(c => new ClassEntry(c.extends, c.Name, c.EntityName, assembly, @namespace)));
			classEntries.UnionWith(document.JoinedSubclasses.Select(c => new ClassEntry(c.extends, c.Name, c.EntityName, assembly, @namespace)));
			classEntries.UnionWith(document.UnionSubclasses.Select(c => new ClassEntry(c.extends, c.Name, c.EntityName, assembly, @namespace)));

			return classEntries;
		}
	}
}
