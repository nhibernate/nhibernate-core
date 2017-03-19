using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IModelInspector
	{
		bool IsRootEntity(System.Type type);
		bool IsComponent(System.Type type);
		bool IsEntity(System.Type type);

		bool IsTablePerClass(System.Type type);
		bool IsTablePerClassSplit(System.Type type, object splitGroupId, MemberInfo member);
		bool IsTablePerClassHierarchy(System.Type type);
		bool IsTablePerConcreteClass(System.Type type);

		bool IsOneToOne(MemberInfo member, System.Type componentType);
		bool IsManyToOne(MemberInfo member);
		bool IsManyToOne(MemberInfo member, System.Type componentType);
		bool IsManyToManyItem(MemberInfo member, System.Type componentType);
		bool IsManyToManyKey(MemberInfo member, System.Type componentType);
		bool IsOneToMany(MemberInfo member);
		bool IsOneToMany(MemberInfo member, System.Type componentType);
		bool IsManyToAny(MemberInfo member, System.Type componentType);

		bool IsAny(MemberInfo member, System.Type componentType);

		bool IsPersistentId(MemberInfo member);
		bool IsPersistentId(MemberInfo member, System.Type componentType);
		bool IsMemberOfComposedId(MemberInfo member);
		bool IsVersion(MemberInfo member);
		bool IsVersion(MemberInfo member, System.Type componentType);
		bool IsMemberOfNaturalId(MemberInfo member);
		bool IsMemberOfNaturalId(MemberInfo member, System.Type componentType);

		bool IsPersistentProperty(MemberInfo member);
		bool IsPersistentProperty(MemberInfo member, System.Type componentType);

		bool IsSet(MemberInfo role);
		bool IsSet(MemberInfo role, System.Type componentType);
		bool IsBag(MemberInfo role);
		bool IsBag(MemberInfo role, System.Type componentType);
		bool IsIdBag(MemberInfo role, System.Type componentType);
		bool IsList(MemberInfo role, System.Type componentType);
		bool IsArray(MemberInfo role);
		bool IsArray(MemberInfo role, System.Type componentType);
		bool IsDictionary(MemberInfo role);
		bool IsDictionary(MemberInfo role, System.Type componentType);
		bool IsProperty(MemberInfo member, System.Type componentType);
		bool IsDynamicComponent(MemberInfo member, System.Type componentType);
		System.Type GetDynamicComponentTemplate(MemberInfo member);
		IEnumerable<string> GetPropertiesSplits(System.Type type);
	}
}
