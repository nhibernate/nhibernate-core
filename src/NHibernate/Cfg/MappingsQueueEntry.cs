using System.Collections;
using Iesi.Collections;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Holds information about mapped classes found in an embedded resource
	/// </summary>
	public class MappingsQueueEntry
	{
		private readonly NamedXmlDocument document;
		private readonly ISet requiredClassNames;
		private readonly ISet containedClassNames;

		public MappingsQueueEntry(NamedXmlDocument document, ICollection classEntries)
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

		private static ISet GetClassNames(ICollection classEntries)
		{
			HashedSet result = new HashedSet();

			foreach (ClassExtractor.ClassEntry ce in classEntries)
			{
				result.Add(ce.FullClassName);
			}

			return result;
		}

		private static ISet GetExtendsNames(ICollection classEntries)
		{
			HashedSet result = new HashedSet();

			foreach (ClassExtractor.ClassEntry ce in classEntries)
			{
				if (ce.FullExtends != null)
				{
					result.Add(ce.FullExtends);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the names of all classes outside this resource
		/// needed by the classes in this resource.
		/// </summary>
		/// <returns>An <see cref="ISet"/> of <see cref="AssemblyQualifiedTypeName" /></returns>
		public ICollection RequiredClassNames
		{
			get { return requiredClassNames; }
		}

		public ICollection ContainedClassNames
		{
			get { return containedClassNames; }
		}
	}
}