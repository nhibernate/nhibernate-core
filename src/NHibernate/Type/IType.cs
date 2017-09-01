using System.Collections;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Defines a mapping from a .NET <see cref="System.Type"/> to a SQL data-type.
	/// This interface is intended to be implemented by applications that need custom types.
	/// </summary>
	/// <remarks>
	/// Implementors should usually be immutable and MUST definitely be threadsafe.
	/// </remarks>
	public partial interface IType : ICacheAssembler
	{
		// QUESTION:
		// How do we implement Serializable interface? Standard .NET pattern or other?

		/// <summary>
		/// When implemented by a class, gets the abbreviated name of the type.
		/// </summary>
		/// <value>The NHibernate type name.</value>
		string Name { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="System.Type"/> returned
		/// by the <c>NullSafeGet()</c> methods.
		/// </summary>
		/// <value>
		/// The <see cref="System.Type"/> from the .NET framework.
		/// </value>
		/// <remarks>
		/// This is used to establish the class of an array of this <c>IType</c>.
		/// </remarks>
		System.Type ReturnedClass { get; }

		/// <summary>
		/// When implemented by a class, gets the value indicating if the objects
		/// of this IType are mutable.
		/// </summary>
		/// <value>true if the objects mapped by this IType are mutable.</value>
		/// <remarks>
		/// With respect to the referencing object...
		/// Entities and Collections are considered immutable because they manage their own internal state.
		/// </remarks>
		bool IsMutable { get; }

		/// <summary>
		/// When implemented by a class, gets a value indicating if the implementor is castable to an <see cref="IAssociationType"/>.
		/// </summary>
		/// <value><see langword="true" /> if this is an association.</value>
		/// <remarks>
		/// This does not necessarily imply that the type actually represents an association.
		/// </remarks>
		bool IsAssociationType { get; }

		/// <summary>
		/// When implemented by a class, gets a value indicating if the implementor is a collection type.
		/// </summary>
		/// <value><see langword="true" /> if this is a <see cref="CollectionType"/>.</value>
		bool IsCollectionType { get; }

		/// <summary>
		/// When implemented by a class, gets a value indicating if the implementor is an <see cref="IAbstractComponentType"/>.
		/// </summary>
		/// <value><see langword="true" /> if this is an <see cref="IAbstractComponentType"/>.</value>
		/// <remarks>
		/// If true, the implementation must be castable to <see cref="IAbstractComponentType"/>.
		/// A component type may own collections or associations and hence must provide certain extra functionality.
		/// </remarks>
		bool IsComponentType { get; }

		/// <summary>
		/// When implemented by a class, gets a value indicating if the implementor extends <see cref="EntityType"/>.
		/// </summary>
		/// <value><see langword="true" /> if this is an <see cref="EntityType"/>.</value>
		bool IsEntityType { get; }

		/// <summary>
		/// When implemented by a class, gets a value indicating if the implementation is an "any" type.
		/// </summary>
		/// <value><see langword="true" /> if this an "any" type.</value>
		/// <remarks>This is a reference to a persistent entity that is not modelled as a (foreign key) association.</remarks>
		bool IsAnyType { get; }

		/// <summary>
		/// When implemented by a class, returns the SqlTypes for the columns mapped by this IType.
		/// </summary>
		/// <param name="mapping">The <see cref="IMapping"/> that uses this IType.</param>
		/// <returns>An array of <see cref="SqlType"/>s.</returns>
		SqlType[] SqlTypes(IMapping mapping);

		/// <summary>
		/// When implemented by a class, returns how many columns are used to persist this type.
		/// </summary>
		/// <param name="mapping">The <see cref="IMapping"/> that uses this IType.</param>
		/// <returns>The number of columns this IType spans.</returns>
		/// <exception cref="MappingException">MappingException</exception>
		int GetColumnSpan(IMapping mapping);

		/// <summary>
		/// When implemented by a class, should the parent be considered dirty,
		/// given both the old and current field or element value?
		/// </summary>
		/// <param name="old">The old value</param>
		/// <param name="current">The current value</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> </param>
		/// <returns>true if the field is dirty</returns>
		bool IsDirty(object old, object current, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, should the parent be considered dirty,
		/// given both the old and current field or element value?
		/// </summary>
		/// <param name="old">The old value</param>
		/// <param name="current">The current value</param>
		/// <param name="checkable">Indicates which columns are to be checked.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> </param>
		/// <returns>true if the field is dirty</returns>
		bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session);

		bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, gets an instance of the object mapped by
		/// this IType from the <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the values</param>
		/// <param name="names">
		/// The names of the columns in the <see cref="DbDataReader"/> that contain the
		/// value to populate the IType with.
		/// </param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns>The object mapped by this IType.</returns>
		/// <remarks>
		/// Implementors should handle possibility of null values.
		/// </remarks>
		object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <summary>
		/// When implemented by a class, gets an instance of the object
		/// mapped by this IType from the <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the values</param>
		/// <param name="name">The name of the column in the <see cref="DbDataReader"/> that contains the
		/// value to populate the IType with.</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns>The object mapped by this IType.</returns>
		/// <remarks>
		/// Implementations should handle possibility of null values.
		/// This method might be called if the IType is known to be a single-column type.
		/// </remarks>
		object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner);

		/// <summary>
		/// When implemented by a class, puts the value/values from the mapped
		/// class into the <see cref="DbCommand"/>.
		/// </summary>
		/// <param name="st">The <see cref="DbCommand"/> to put the values into.</param>
		/// <param name="value">The object that contains the values.</param>
		/// <param name="index">The index of the <see cref="DbParameter"/> to start writing the values to.</param>
		/// <param name="settable">Indicates which columns are to be set.</param>
		/// <param name="session"></param>
		/// <remarks>
		/// Implementors should handle possibility of null values.
		/// A multi-column type should be written to parameters starting from <paramref name="index"/>.
		/// </remarks>
		void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session);

		/// <summary>
		/// 	When implemented by a class, puts the value/values from the mapped
		/// 	class into the <see cref="DbCommand"/>.
		/// </summary>
		/// <param name="st">
		/// 	The <see cref="DbCommand"/> to put the values into.
		/// </param>
		/// <param name="value">The object that contains the values.</param>
		/// <param name="index">
		/// 	The index of the <see cref="DbParameter"/> to start writing the values to.
		/// </param>
		/// <param name="session"></param>
		/// <remarks>
		/// 	Implementors should handle possibility of null values.
		/// 	A multi-column type should be written to parameters starting from <paramref name="index"/>.
		/// </remarks>
		void NullSafeSet(DbCommand st, object value, int index, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, a representation of the value to be
		/// embedded in an XML element
		/// </summary>
		/// <param name="value">The object that contains the values.</param>
		/// <param name="factory"></param>
		/// <returns>An Xml formatted string.</returns>
		string ToLoggableString(object value, ISessionFactoryImplementor factory);

		/// <summary>
		/// When implemented by a class, returns a deep copy of the persistent
		/// state, stopping at entities and at collections.
		/// </summary>
		/// <param name="val">A Collection element or Entity field.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>A deep copy of the object.</returns>
		object DeepCopy(object val, ISessionFactoryImplementor factory);

		/// <summary>
		/// When implemented by a class, retrieves an instance of the mapped class,
		/// or the identifier of an entity or collection from a <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the values.</param>
		/// <param name="names">
		/// The names of the columns in the <see cref="DbDataReader"/> that contain the
		/// value to populate the IType with.
		/// </param>
		/// <param name="session">The session.</param>
		/// <param name="owner">The parent Entity.</param>
		/// <returns>An identifier or actual object mapped by this IType.</returns>
		/// <remarks>
		/// <para>
		/// This is useful for 2-phase property initialization - the second phase is a call to
		/// <c>ResolveIdentifier()</c>
		/// </para>
		/// <para>
		/// Most implementors of this method will just pass the call to <c>NullSafeGet()</c>.
		/// </para>
		/// </remarks>
		object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <summary>
		/// When implemented by a class, maps identifiers to Entities or Collections.
		/// </summary>
		/// <param name="value">An identifier or value returned by <c>Hydrate()</c>.</param>
		/// <param name="session">The session.</param>
		/// <param name="owner">The parent Entity.</param>
		/// <returns>The Entity or Collection referenced by this Identifier.</returns>
		/// <remarks>
		/// This is the second phase of 2-phase property initialization.
		/// </remarks>
		object ResolveIdentifier(object value, ISessionImplementor session, object owner);

		/// <summary>
		/// Given a hydrated, but unresolved value, return a value that may be used to
		/// reconstruct property-ref associations.
		/// </summary>
		object SemiResolve(object value, ISessionImplementor session, object owner);

		/// <summary>
		/// During merge, replace the existing (target) value in the entity we are merging to
		/// with a new (original) value from the detached entity we are merging. For immutable
		/// objects, or null values, it is safe to simply return the first parameter. For
		/// mutable objects, it is safe to return a copy of the first parameter. For objects
		/// with component values, it might make sense to recursively replace component values.
		/// </summary>
		/// <param name="original">The value from the detached entity being merged.</param>
		/// <param name="target">The value in the managed entity.</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copiedAlready"></param>
		/// <returns>The value to be merged.</returns>
		object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready);

		/// <summary>
		/// During merge, replace the existing (target) value in the entity we are merging to
		/// with a new (original) value from the detached entity we are merging. For immutable
		/// objects, or null values, it is safe to simply return the first parameter. For
		/// mutable objects, it is safe to return a copy of the first parameter. For objects
		/// with component values, it might make sense to recursively replace component values.
		/// </summary>
		/// <param name="original">The value from the detached entity being merged.</param>
		/// <param name="target">The value in the managed entity.</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copyCache"></param>
		/// <param name="foreignKeyDirection"></param>
		/// <returns>The value to be merged.</returns>
		object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection);

		/// <summary> 
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality" - equality of persistent state - taking a shortcut for
		/// entity references.
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <returns> boolean </returns>
		bool IsSame(object x, object y);

		/// <summary>
		/// When implemented by a class, compare two instances of the class mapped by this
		/// <c>IType</c> for persistence "equality" - ie. Equality of persistent state.
		/// </summary>
		/// <param name="x">The left hand side object.</param>
		/// <param name="y">The right hand side object.</param>
		/// <returns>True if the two objects contain the same values.</returns>
		bool IsEqual(object x, object y);

		/// <summary>
		/// When implemented by a class, compare two instances of the class mapped by this
		/// <c>IType</c> for persistence "equality" - ie. Equality of persistent state.
		/// </summary>
		/// <param name="x">The left hand side object.</param>
		/// <param name="y">The right hand side object.</param>
		/// <param name="factory">The session factory for which the values are compared.</param>
		/// <returns>True if the two objects contain the same values.</returns>
		bool IsEqual(object x, object y, ISessionFactoryImplementor factory);

		/// <summary> Get a hashcode, consistent with persistence "equality"</summary>
		/// <param name="x"> </param>
		int GetHashCode(object x);

		/// <summary> Get a hashcode, consistent with persistence "equality"</summary>
		/// <param name="x"> </param>
		/// <param name="factory"> </param>
		int GetHashCode(object x, ISessionFactoryImplementor factory);

		/// <summary> compare two instances of the type</summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		int Compare(object x, object y);

		/// <summary> Get the type of a semi-resolved value.</summary>
		IType GetSemiResolvedType(ISessionFactoryImplementor factory);

		/// <summary>
		/// Given an instance of the type, return an array of boolean, indicating
		/// which mapped columns would be null.
		/// </summary>
		/// <param name="value">an instance of the type </param>
		/// <param name="mapping"></param>
		bool[] ToColumnNullness(object value, IMapping mapping);
	}
}