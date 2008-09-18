using Iesi.Collections;
using NHibernate.Util;
using Iesi.Collections.Generic;
using System.Collections.Generic;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Holds information about mapped classes found in an embedded resource
	/// </summary>
	public class MappingsQueueEntry
	{
		private readonly NamedXmlDocument document;
		private readonly ISet<string> requiredClassNames;
		private readonly ISet<string> containedClassNames;

		public MappingsQueueEntry(NamedXmlDocument document, IEnumerable<ClassExtractor.ClassEntry> classEntries)
		{
			this.document = document;

			containedClassNames = GetClassNames(classEntries);
			requiredClassNames = GetExtendsNames(classEntries);
			requiredClassNames.RemoveAll(containedClassNames);
		}

		public NamedXmlDocument Document
		{
			get { return document; }
		}

		private static ISet<string> GetClassNames(IEnumerable<ClassExtractor.ClassEntry> classEntries)
		{
			HashedSet<string> result = new HashedSet<string>();

			foreach (ClassExtractor.ClassEntry ce in classEntries)
			{
				if (ce.EntityName != null)
				{
					result.Add(ce.EntityName);
				}
				else if (ce.FullClassName != null)
				{
					result.Add(ce.FullClassName.Type);
				}
			}

			return result;
		}

		private static ISet<string> GetExtendsNames(IEnumerable<ClassExtractor.ClassEntry> classEntries)
		{
			HashedSet<string> result = new HashedSet<string>();

			foreach (ClassExtractor.ClassEntry ce in classEntries)
			{
				if (ce.FullExtends != null)
				{
					result.Add(ce.FullExtends.Type);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the names of all classes outside this resource
		/// needed by the classes in this resource.
		/// </summary>
		/// <returns>An <see cref="ISet"/> of <see cref="AssemblyQualifiedTypeName" /></returns>
		public ICollection<string> RequiredClassNames
		{
			get { return requiredClassNames; }
		}

		public ICollection<string> ContainedClassNames
		{
			get { return containedClassNames; }
		}
	}
}
