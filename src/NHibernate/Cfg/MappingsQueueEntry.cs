using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Holds information about mapped classes found in an embedded resource
	/// </summary>
	public class MappingsQueueEntry
	{
		private readonly HashedSet<string> containedClassNames;
		private readonly NamedXmlDocument document;
		private readonly HashedSet<string> requiredClassNames;

		public MappingsQueueEntry(NamedXmlDocument document, IEnumerable<ClassExtractor.ClassEntry> classEntries)
		{
			this.document = document;

			containedClassNames = GetClassNames(classEntries);
			requiredClassNames = GetRequiredClassNames(classEntries, containedClassNames);
		}

		public NamedXmlDocument Document
		{
			get { return document; }
		}

		/// <summary>
		/// Gets the names of all entities outside this resource
		/// needed by the classes in this resource.
		/// </summary>
		public ICollection<string> RequiredClassNames
		{
			get { return requiredClassNames; }
		}

		/// <summary>
		/// Gets the names of all entities in this resource
		/// </summary>
		public ICollection<string> ContainedClassNames
		{
			get { return containedClassNames; }
		}

		private static HashedSet<string> GetClassNames(IEnumerable<ClassExtractor.ClassEntry> classEntries)
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

		private static HashedSet<string> GetRequiredClassNames(IEnumerable<ClassExtractor.ClassEntry> classEntries,
		                                                       ICollection<string> containedNames)
		{
			HashedSet<string> result = new HashedSet<string>();

			foreach (ClassExtractor.ClassEntry ce in classEntries)
			{
				if (ce.ExtendsEntityName != null && !containedNames.Contains(ce.FullExtends.Type) && !containedNames.Contains(ce.ExtendsEntityName))
				{
					result.Add(ce.FullExtends.Type);
				}
			}

			return result;
		}
	}
}