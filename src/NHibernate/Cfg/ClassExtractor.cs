using System.Xml;
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

			classEntries.UnionWith(GetRootClassesEntries(assembly, @namespace, document.RootClasses));
			classEntries.UnionWith(GetSubclassesEntries(assembly, @namespace, null, document.SubClasses));
			classEntries.UnionWith(GetJoinedSubclassesEntries(assembly, @namespace, null, document.JoinedSubclasses));
			classEntries.UnionWith(GetUnionSubclassesEntries(assembly, @namespace, null, document.UnionSubclasses));

			return classEntries;
		}

		private static IEnumerable<ClassEntry> GetRootClassesEntries(string assembly, string @namespace,IEnumerable<HbmClass> rootClasses)
		{
			foreach (var rootClass in rootClasses)
			{
				string entityName = rootClass.EntityName;
				yield return new ClassEntry(null, rootClass.Name, entityName, assembly, @namespace);
				foreach (var classEntry in GetSubclassesEntries(assembly, @namespace, entityName, rootClass.Subclasses))
				{
					yield return classEntry;
				}
				foreach (var classEntry in GetJoinedSubclassesEntries(assembly, @namespace, entityName, rootClass.JoinedSubclasses))
				{
					yield return classEntry;
				}
				foreach (var classEntry in GetUnionSubclassesEntries(assembly, @namespace, entityName, rootClass.UnionSubclasses))
				{
					yield return classEntry;
				}
			}
		}

		private static IEnumerable<ClassEntry> GetSubclassesEntries(string assembly, string @namespace, string defaultExtends,
		                                                            IEnumerable<HbmSubclass> hbmSubclasses)
		{
			foreach (HbmSubclass subclass in hbmSubclasses)
			{
				string extends = subclass.extends ?? defaultExtends;
				yield return new ClassEntry(extends, subclass.Name, subclass.EntityName, assembly, @namespace);
				foreach (ClassEntry classEntry in GetSubclassesEntries(assembly, @namespace, subclass.EntityName,subclass.Subclasses))
				{
					yield return classEntry;
				}
			}
		}

		private static IEnumerable<ClassEntry> GetJoinedSubclassesEntries(string assembly, string @namespace,
		                                                                  string defaultExtends,
		                                                                  IEnumerable<HbmJoinedSubclass> hbmJoinedSubclasses)
		{
			foreach (HbmJoinedSubclass subclass in hbmJoinedSubclasses)
			{
				string extends = subclass.extends ?? defaultExtends;
				yield return new ClassEntry(extends, subclass.Name, subclass.EntityName, assembly, @namespace);
				foreach (ClassEntry classEntry in GetJoinedSubclassesEntries(assembly, @namespace, subclass.EntityName, subclass.JoinedSubclasses))
				{
					yield return classEntry;
				}
			}
		}

		private static IEnumerable<ClassEntry> GetUnionSubclassesEntries(string assembly, string @namespace,
		                                                                 string defaultExtends,
		                                                                 IEnumerable<HbmUnionSubclass> hbmUnionSubclasses)
		{
			foreach (HbmUnionSubclass subclass in hbmUnionSubclasses)
			{
				string extends = subclass.extends ?? defaultExtends;
				yield return new ClassEntry(extends, subclass.Name, subclass.EntityName, assembly, @namespace);
				foreach (ClassEntry classEntry in GetUnionSubclassesEntries(assembly, @namespace, subclass.EntityName,subclass.UnionSubclasses))
				{
					yield return classEntry;
				}
			}
		}
	}
}
