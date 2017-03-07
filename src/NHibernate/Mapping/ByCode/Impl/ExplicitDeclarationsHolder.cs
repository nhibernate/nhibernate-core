using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ExplicitDeclarationsHolder : IModelExplicitDeclarationsHolder
	{
		private readonly HashSet<MemberInfo> any = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> components = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> idBags = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> keyManyToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> itemManyToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> manyToOneRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> manyToAnyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> naturalIds = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToOneRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> composedIds = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> properties = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> dynamicComponents = new HashSet<MemberInfo>();
		private readonly Dictionary<MemberInfo, System.Type> dynamicComponentTemplates = new Dictionary<MemberInfo, System.Type>();
		private readonly HashSet<MemberInfo> persistentMembers = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> rootEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly HashSet<SplitDefinition> splitDefinitions = new HashSet<SplitDefinition>();
		private readonly HashSet<System.Type> tablePerClassEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerClassHierarchyEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerConcreteClassEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> versionProperties = new HashSet<MemberInfo>();

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

		public IEnumerable<MemberInfo> KeyManyToManyRelations
		{
			get { return keyManyToManyRelations; }
		}

		public IEnumerable<MemberInfo> ItemManyToManyRelations
		{
			get { return itemManyToManyRelations; }
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
			System.Type template;
			dynamicComponentTemplates.TryGetValue(member, out template);
			return template ?? typeof(object);
		}

		public void AddAsRootEntity(System.Type type)
		{
			rootEntities.Add(type);
		}

		public void AddAsComponent(System.Type type)
		{
			components.Add(type);
		}

		public void AddAsTablePerClassEntity(System.Type type)
		{
			tablePerClassEntities.Add(type);
		}

		public void AddAsTablePerClassHierarchyEntity(System.Type type)
		{
			tablePerClassHierarchyEntities.Add(type);
		}

		public void AddAsTablePerConcreteClassEntity(System.Type type)
		{
			tablePerConcreteClassEntities.Add(type);
		}

		public void AddAsOneToOneRelation(MemberInfo member)
		{
			oneToOneRelations.Add(member);
		}

		public void AddAsManyToOneRelation(MemberInfo member)
		{
			manyToOneRelations.Add(member);
		}

		public void AddAsManyToManyKeyRelation(MemberInfo member)
		{
			keyManyToManyRelations.Add(member);
		}

		public void AddAsManyToManyItemRelation(MemberInfo member)
		{
			itemManyToManyRelations.Add(member);
		}

		public void AddAsOneToManyRelation(MemberInfo member)
		{
			oneToManyRelations.Add(member);
		}

		public void AddAsManyToAnyRelation(MemberInfo member)
		{
			manyToAnyRelations.Add(member);
		}

		public void AddAsAny(MemberInfo member)
		{
			any.Add(member);
		}

		public void AddAsPoid(MemberInfo member)
		{
			poids.Add(member);
		}

		public void AddAsPartOfComposedId(MemberInfo member)
		{
			composedIds.Add(member);
		}

		public void AddAsVersionProperty(MemberInfo member)
		{
			versionProperties.Add(member);
		}

		public void AddAsNaturalId(MemberInfo member)
		{
			naturalIds.Add(member);
		}

		public void AddAsSet(MemberInfo member)
		{
			sets.Add(member);
		}

		public void AddAsBag(MemberInfo member)
		{
			bags.Add(member);
		}

		public void AddAsIdBag(MemberInfo member)
		{
			idBags.Add(member);
		}

		public void AddAsList(MemberInfo member)
		{
			lists.Add(member);
		}

		public void AddAsArray(MemberInfo member)
		{
			arrays.Add(member);
		}

		public void AddAsMap(MemberInfo member)
		{
			dictionaries.Add(member);
		}

		public void AddAsProperty(MemberInfo member)
		{
			properties.Add(member);
		}

		public void AddAsPersistentMember(MemberInfo member)
		{
			persistentMembers.Add(member);
		}

		public void AddAsPropertySplit(SplitDefinition definition)
		{
			splitDefinitions.Add(definition);
		}

		public void AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate)
		{
			dynamicComponents.Add(member);
			dynamicComponentTemplates[member] = componentTemplate;
		}

		#endregion
	}
}