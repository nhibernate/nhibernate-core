using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IModelInspector
	{
		bool IsRootEntity(System.Type type);
		bool IsComponent(System.Type type);
		bool IsEntity(System.Type type);

		bool IsTablePerClass(System.Type type);
		bool IsTablePerClassHierarchy(System.Type type);
		bool IsTablePerClassHierarchyJoin(System.Type type);
		bool IsTablePerConcreteClass(System.Type type);

		bool IsOneToOne(MemberInfo member);
		bool IsManyToOne(MemberInfo member);
		bool IsManyToMany(MemberInfo member);
		bool IsOneToMany(MemberInfo member);
		bool IsHeterogeneousAssociation(MemberInfo member);

		bool IsPersistentId(MemberInfo member);
		bool IsVersion(MemberInfo member);
		bool IsMemberOfNaturalId(MemberInfo member);

		bool IsPersistentProperty(MemberInfo member);

		bool IsSet(MemberInfo role);
		bool IsBag(MemberInfo role);
		bool IsIdBag(MemberInfo role);
		bool IsList(MemberInfo role);
		bool IsArray(MemberInfo role);
		bool IsDictionary(MemberInfo role);
		bool IsProperty(MemberInfo member);
	}
}