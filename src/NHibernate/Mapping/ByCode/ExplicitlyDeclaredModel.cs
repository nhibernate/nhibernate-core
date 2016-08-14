using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public class ExplicitlyDeclaredModel : AbstractExplicitlyDeclaredModel, IModelInspector
	{
		#region Implementation of IModelInspector

		public override bool IsRootEntity(System.Type type)
		{
			return RootEntities.Contains(type);
		}

		public override bool IsComponent(System.Type type)
		{
			return Components.Contains(type);
		}

		public virtual bool IsEntity(System.Type type)
		{
			return RootEntities.Contains(type) || type.GetBaseTypes().Any(t => RootEntities.Contains(t)) || HasDelayedEntityRegistration(type);
		}

		public virtual bool IsTablePerClass(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedForTablePerClassEntities(type);
		}

		public virtual bool IsTablePerClassSplit(System.Type type, object splitGroupId, MemberInfo member)
		{
			return Equals(splitGroupId, GetSplitGroupFor(member));
		}

		public virtual bool IsTablePerClassHierarchy(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedForTablePerClassHierarchyEntities(type);
		}

		public virtual bool IsTablePerConcreteClass(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedForTablePerConcreteClassEntities(type);
		}

		public virtual bool IsOneToOne(MemberInfo member, System.Type _)
		{
			return OneToOneRelations.Contains(member);
		}

		public virtual bool IsManyToOne(MemberInfo member)
		{
			return ManyToOneRelations.Contains(member);
		}

		public virtual bool IsManyToOne(MemberInfo member, System.Type _)
		{
			return ManyToOneRelations.Contains(member);
		}

		public virtual bool IsManyToManyItem(MemberInfo member, System.Type _)
		{
			return ItemManyToManyRelations.Contains(member);
		}

		public virtual bool IsManyToManyKey(MemberInfo member, System.Type _)
		{
			return KeyManyToManyRelations.Contains(member);
		}

		public virtual bool IsOneToMany(MemberInfo member)
		{
			return OneToManyRelations.Contains(member);
		}

		public virtual bool IsOneToMany(MemberInfo member, System.Type _)
		{
			return OneToManyRelations.Contains(member);
		}

		public bool IsManyToAny(MemberInfo member, System.Type _)
		{
			return ManyToAnyRelations.Contains(member);
		}

		public virtual bool IsAny(MemberInfo member, System.Type _)
		{
			return Any.Contains(member);
		}

		public virtual bool IsPersistentId(MemberInfo member)
		{
			return Poids.Contains(member);
		}

		public virtual bool IsPersistentId(MemberInfo member, System.Type _)
		{
			return Poids.Contains(member);
		}

		public bool IsMemberOfComposedId(MemberInfo member)
		{
			return ComposedIds.Contains(member);
		}

		public virtual bool IsVersion(MemberInfo member)
		{
			return VersionProperties.Contains(member);
		}

		public virtual bool IsVersion(MemberInfo member, System.Type _)
		{
			return VersionProperties.Contains(member);
		}

		public virtual bool IsMemberOfNaturalId(MemberInfo member)
		{
			return NaturalIds.Contains(member);
		}

		public virtual bool IsMemberOfNaturalId(MemberInfo member, System.Type _)
		{
			return NaturalIds.Contains(member);
		}

		public virtual bool IsPersistentProperty(MemberInfo member)
		{
			return PersistentMembers.Contains(member);
		}

		public virtual bool IsPersistentProperty(MemberInfo member, System.Type _)
		{
			return PersistentMembers.Contains(member);
		}

		public virtual bool IsSet(MemberInfo role)
		{
			return Sets.Contains(role);
		}

		public virtual bool IsSet(MemberInfo role, System.Type _)
		{
			return Sets.Contains(role);
		}

		public virtual bool IsBag(MemberInfo role)
		{
			return Bags.Contains(role);
		}

		public virtual bool IsBag(MemberInfo role, System.Type _)
		{
			return Bags.Contains(role);
		}

		public virtual bool IsIdBag(MemberInfo role, System.Type _)
		{
			return IdBags.Contains(role);
		}

		public virtual bool IsList(MemberInfo role, System.Type _)
		{
			return Lists.Contains(role);
		}

		public virtual bool IsArray(MemberInfo role)
		{
			return Arrays.Contains(role);
		}

		public virtual bool IsArray(MemberInfo role, System.Type _)
		{
			return Arrays.Contains(role);
		}

		public virtual bool IsDictionary(MemberInfo role)
		{
			return Dictionaries.Contains(role);
		}

		public virtual bool IsDictionary(MemberInfo role, System.Type _)
		{
			return Dictionaries.Contains(role);
		}

		public virtual bool IsProperty(MemberInfo member, System.Type _)
		{
			return Properties.Contains(member);
		}

		public virtual bool IsDynamicComponent(MemberInfo member, System.Type _)
		{
			return DynamicComponents.Contains(member);
		}

		public virtual IEnumerable<string> GetPropertiesSplits(System.Type type)
		{
			return GetSplitGroupsFor(type);
		}

		#endregion
	}
}
