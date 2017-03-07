using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public class SplitDefinition
	{
		public SplitDefinition(System.Type @on, string groupId, MemberInfo member)
		{
			On = on;
			GroupId = groupId;
			Member = member;
		}

		public System.Type On { get; private set; }
		public string GroupId { get; private set; }
		public MemberInfo Member { get; private set; }
	}

	public interface IModelExplicitDeclarationsHolder
	{
		IEnumerable<System.Type> RootEntities { get; }
		IEnumerable<System.Type> Components { get; }
		IEnumerable<System.Type> TablePerClassEntities { get; }
		IEnumerable<System.Type> TablePerClassHierarchyEntities { get; }
		IEnumerable<System.Type> TablePerConcreteClassEntities { get; }

		IEnumerable<MemberInfo> OneToOneRelations { get; }
		IEnumerable<MemberInfo> ManyToOneRelations { get; }
		IEnumerable<MemberInfo> KeyManyToManyRelations { get; }
		IEnumerable<MemberInfo> ItemManyToManyRelations { get; }
		IEnumerable<MemberInfo> OneToManyRelations { get; }
		IEnumerable<MemberInfo> ManyToAnyRelations { get; }

		IEnumerable<MemberInfo> Any { get; }

		IEnumerable<MemberInfo> Poids { get; }
		IEnumerable<MemberInfo> ComposedIds { get; }
		IEnumerable<MemberInfo> VersionProperties { get; }
		IEnumerable<MemberInfo> NaturalIds { get; }

		IEnumerable<MemberInfo> Sets { get; }
		IEnumerable<MemberInfo> Bags { get; }
		IEnumerable<MemberInfo> IdBags { get; }
		IEnumerable<MemberInfo> Lists { get; }
		IEnumerable<MemberInfo> Arrays { get; }
		IEnumerable<MemberInfo> Dictionaries { get; }
		IEnumerable<MemberInfo> Properties { get; }
		IEnumerable<MemberInfo> DynamicComponents { get; }
		IEnumerable<MemberInfo> PersistentMembers { get; }
		IEnumerable<SplitDefinition> SplitDefinitions { get; }

		IEnumerable<string> GetSplitGroupsFor(System.Type type);
		string GetSplitGroupFor(MemberInfo member);
		System.Type GetDynamicComponentTemplate(MemberInfo member);

		void AddAsRootEntity(System.Type type);
		void AddAsComponent(System.Type type);
		void AddAsTablePerClassEntity(System.Type type);
		void AddAsTablePerClassHierarchyEntity(System.Type type);
		void AddAsTablePerConcreteClassEntity(System.Type type);

		void AddAsOneToOneRelation(MemberInfo member);
		void AddAsManyToOneRelation(MemberInfo member);
		void AddAsManyToManyKeyRelation(MemberInfo member);
		void AddAsManyToManyItemRelation(MemberInfo member);
		void AddAsOneToManyRelation(MemberInfo member);
		void AddAsManyToAnyRelation(MemberInfo member);

		void AddAsAny(MemberInfo member);

		void AddAsPoid(MemberInfo member);
		void AddAsPartOfComposedId(MemberInfo member);
		void AddAsVersionProperty(MemberInfo member);
		void AddAsNaturalId(MemberInfo member);

		void AddAsSet(MemberInfo member);
		void AddAsBag(MemberInfo member);
		void AddAsIdBag(MemberInfo member);
		void AddAsList(MemberInfo member);
		void AddAsArray(MemberInfo member);
		void AddAsMap(MemberInfo member);
		void AddAsProperty(MemberInfo member);
		void AddAsPersistentMember(MemberInfo member);
		void AddAsPropertySplit(SplitDefinition definition);
		void AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate);
	}
}