using System.Collections.Generic;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Holds information about mapped classes found in an embedded resource
	/// </summary>
	public class MappingsQueueEntry
	{
		private readonly HashSet<string> containedClassNames;
		private readonly NamedXmlDocument document;
		private readonly HashSet<RequiredEntityName> requiredClassNames;

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
		public ICollection<RequiredEntityName> RequiredClassNames
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

		private static HashSet<string> GetClassNames(IEnumerable<ClassExtractor.ClassEntry> classEntries)
		{
			var result = new HashSet<string>();

			foreach (var ce in classEntries)
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

		private static HashSet<RequiredEntityName> GetRequiredClassNames(IEnumerable<ClassExtractor.ClassEntry> classEntries,
		                                                       ICollection<string> containedNames)
		{
			var result = new HashSet<RequiredEntityName>();

			foreach (var ce in classEntries)
			{
				if (ce.ExtendsEntityName != null && !containedNames.Contains(ce.FullExtends.Type)
				    && !containedNames.Contains(ce.ExtendsEntityName))
				{
					result.Add(new RequiredEntityName(ce.ExtendsEntityName, ce.FullExtends.Type));
				}
			}

			return result;
		}

		public class RequiredEntityName
		{
			public RequiredEntityName(string entityName, string fullClassName)
			{
				EntityName = entityName;
				FullClassName = fullClassName;
			}

			public string EntityName { get; private set; }

			public string FullClassName { get; private set; }

			public bool Equals(RequiredEntityName obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				return Equals(obj.EntityName, EntityName) && Equals(obj.FullClassName, FullClassName);
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				var thatSimple = obj as string;
				if (thatSimple != null && (thatSimple.Equals(EntityName) || thatSimple.Equals(FullClassName)))
				{
					return true;
				}
				var that = obj as RequiredEntityName;
				if (that != null)
				{
					return false;
				}
				return Equals(that);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((EntityName != null ? EntityName.GetHashCode() : 0) * 397)
					       ^ (FullClassName != null ? FullClassName.GetHashCode() : 0);
				}
			}

			public override string ToString()
			{
				return string.Format("FullName:{0} - Name:{1}", FullClassName ?? "<null>", EntityName ?? "<null>");
			}
		}
	}
}