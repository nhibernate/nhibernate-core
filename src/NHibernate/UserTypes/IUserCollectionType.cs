using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.UserType
{
	public interface IUserCollectionType
	{
		/// <summary>
		/// Instantiate an uninitialized instance of the collection wrapper
		/// </summary>
		IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister );

		/// <summary>
		/// Wrap an instance of a collection
		/// </summary>
		IPersistentCollection Wrap( ISessionImplementor session, object collection );

		/// <summary>
		/// Return an <see cref="IEnumerable" /> over the elements of this collection - the passed collection
		/// instance may or may not be a wrapper
		/// </summary>
		IEnumerable GetElements( object collection );

		/// <summary>
		/// Optional operation. Does the collection contain the entity instance?
		/// </summary>
		bool Contains( object collection, object entity );

		/// <summary>
		/// Optional operation. Return the index of the entity in the collection.
		/// </summary>
		object IndexOf( object collection, object entity );

		/// <summary>
		/// Replace the elements of a collection with the elements of another collection
		/// </summary>
		object ReplaceElements(
				object original,
				object target,
				ICollectionPersister persister,
				object owner,
				IDictionary copyCache,
				ISessionImplementor session );

		/// <summary>
		/// Instantiate an empty instance of the "underlying" collection (not a wrapper)
		/// </summary>
		object Instantiate();
	}
}
