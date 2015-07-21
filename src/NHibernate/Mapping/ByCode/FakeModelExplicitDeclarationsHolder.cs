using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public class FakeModelExplicitDeclarationsHolder : IModelExplicitDeclarationsHolder
	{
		private readonly IEnumerable<MemberInfo> any = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> arrays = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> bags = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<System.Type> components = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<MemberInfo> dictionaries = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> idBags = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> lists = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> manyToManyRelations = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> manyToOneRelations = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> manyToAnyRelations = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> naturalIds = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> oneToManyRelations = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> oneToOneRelations = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> poids = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> composedIds = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> properties = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> dynamicComponents = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<MemberInfo> persistentMembers = new HashSet<MemberInfo>();
		private readonly IEnumerable<System.Type> rootEntities = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<MemberInfo> sets = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<System.Type> tablePerClassEntities = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<System.Type> tablePerClassHierarchyEntities = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<System.Type> tablePerClassHierarchyJoinEntities = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<System.Type> tablePerConcreteClassEntities = Enumerable.Empty<System.Type>();
		private readonly IEnumerable<MemberInfo> versionProperties = Enumerable.Empty<MemberInfo>();
		private readonly IEnumerable<SplitDefinition> splitDefinitions = Enumerable.Empty<SplitDefinition>();

		#region IModelExplicitDeclarationsHolder Members

		public IEnumerable<System.Type> RootEntities
		{
			get { return rootEntities; }
		}

		public IEnumerable<System.Type> Components
		{
			get { return components; }
		}

		public IEnumerable<System.Type> TablePerClassEntities
		{
			get { return tablePerClassEntities; }
		}

		public IEnumerable<System.Type> TablePerClassHierarchyEntities
		{
			get { return tablePerClassHierarchyEntities; }
		}

		public IEnumerable<System.Type> TablePerClassHierarchyJoinEntities
		{
			get { return tablePerClassHierarchyJoinEntities; }
		}

		public IEnumerable<System.Type> TablePerConcreteClassEntities
		{
			get { return tablePerConcreteClassEntities; }
		}

		public IEnumerable<MemberInfo> OneToOneRelations
		{
			get { return oneToOneRelations; }
		}

		public IEnumerable<MemberInfo> ManyToOneRelations
		{
			get { return manyToOneRelations; }
		}

		public IEnumerable<MemberInfo> ManyToManyRelations
		{
			get { return manyToManyRelations; }
		}

		public IEnumerable<MemberInfo> OneToManyRelations
		{
			get { return oneToManyRelations; }
		}

		public IEnumerable<MemberInfo> ManyToAnyRelations
		{
			get { return manyToAnyRelations; }
		}

		public IEnumerable<MemberInfo> Any
		{
			get { return any; }
		}

		public IEnumerable<MemberInfo> Poids
		{
			get { return poids; }
		}

		public IEnumerable<MemberInfo> ComposedIds
		{
			get { return composedIds; }
		}

		public IEnumerable<MemberInfo> VersionProperties
		{
			get { return versionProperties; }
		}

		public IEnumerable<MemberInfo> NaturalIds
		{
			get { return naturalIds; }
		}

		public IEnumerable<MemberInfo> Sets
		{
			get { return sets; }
		}

		public IEnumerable<MemberInfo> Bags
		{
			get { return bags; }
		}

		public IEnumerable<MemberInfo> IdBags
		{
			get { return idBags; }
		}

		public IEnumerable<MemberInfo> Lists
		{
			get { return lists; }
		}

		public IEnumerable<MemberInfo> Arrays
		{
			get { return arrays; }
		}

		public IEnumerable<MemberInfo> Dictionaries
		{
			get { return dictionaries; }
		}

		public IEnumerable<MemberInfo> Properties
		{
			get { return properties; }
		}

		public IEnumerable<MemberInfo> DynamicComponents
		{
			get { return dynamicComponents; }
		}

		public IEnumerable<MemberInfo> PersistentMembers
		{
			get { return persistentMembers; }
		}

		public IEnumerable<SplitDefinition> SplitDefinitions
		{
			get { return splitDefinitions; }
		}

		public IEnumerable<string> GetSplitGroupsFor(System.Type type)
		{
			return Enumerable.Empty<string>();
		}

		public string GetSplitGroupFor(MemberInfo member)
		{
			return null;
		}

		public System.Type GetDynamicComponentTemplate(MemberInfo member)
		{
			return typeof(object);
		}

		public void AddAsRootEntity(System.Type type) {}

		public void AddAsComponent(System.Type type) {}

		public void AddAsTablePerClassEntity(System.Type type) {}

		public void AddAsTablePerClassHierarchyEntity(System.Type type) {}

		public void AddAsTablePerConcreteClassEntity(System.Type type) {}

		public void AddAsOneToOneRelation(MemberInfo member) {}

		public void AddAsManyToOneRelation(MemberInfo member) {}

		public void AddAsManyToManyRelation(MemberInfo member) {}

		public void AddAsOneToManyRelation(MemberInfo member) {}

		public void AddAsManyToAnyRelation(MemberInfo member) {}

		public void AddAsAny(MemberInfo member) {}

		public void AddAsPoid(MemberInfo member) {}

		public void AddAsPartOfComposedId(MemberInfo member) {}

		public void AddAsVersionProperty(MemberInfo member) {}

		public void AddAsNaturalId(MemberInfo member) {}

		public void AddAsSet(MemberInfo member) {}

		public void AddAsBag(MemberInfo member) {}

		public void AddAsIdBag(MemberInfo member) {}

		public void AddAsList(MemberInfo member) {}

		public void AddAsArray(MemberInfo member) {}

		public void AddAsMap(MemberInfo member) {}

		public void AddAsProperty(MemberInfo member) {}

		public void AddAsPersistentMember(MemberInfo member) {}

		public void AddAsPropertySplit(SplitDefinition definition) {}

		public void AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate) {}

		#endregion
	}
}