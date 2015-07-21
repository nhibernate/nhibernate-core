using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl
{
	public interface ICandidatePersistentMembersProvider
	{
		/// <summary>
		/// Get all candidate persistent properties, or fields, to be used as Persistent-Object-ID, for a given root-entity class or interface.
		/// </summary>
		/// <param name="entityClass">The root-entity class or interface.</param>
		/// <returns>All candidate properties or fields to be used as Persistent-Object-ID.</returns>
		IEnumerable<MemberInfo> GetEntityMembersForPoid(System.Type entityClass);

		/// <summary>
		/// Get all candidate persistent properties or fields for a given root-entity class or interface.
		/// </summary>
		/// <param name="entityClass">The root-entity class or interface.</param>
		/// <returns>All candidate properties or fields.</returns>
		IEnumerable<MemberInfo> GetRootEntityMembers(System.Type entityClass);

		/// <summary>
		/// Get all candidate persistent properties or fields for a given entity subclass or interface.
		/// </summary>
		/// <param name="entityClass">The entity subclass or interface.</param>
		/// <param name="entitySuperclass">The superclass (it may be different from <see cref="System.Type.BaseType"/>)</param>
		/// <returns>All candidate properties or fields.</returns>
		/// <remarks>
		/// In NHibernate, for a subclass, the method should return only those members not included in
		/// its super-classes.
		/// </remarks>
		IEnumerable<MemberInfo> GetSubEntityMembers(System.Type entityClass, System.Type entitySuperclass);

		/// <summary>
		/// Get all candidate persistent properties or fields for a given entity subclass or interface.
		/// </summary>
		/// <param name="componentClass">The class of the component or an interface.</param>
		/// <returns>All candidate properties or fields.</returns>
		IEnumerable<MemberInfo> GetComponentMembers(System.Type componentClass);
	}
}