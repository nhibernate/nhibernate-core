using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.UserTypes
{
	public interface IUserCollectionType
	{
		/// <summary>
		/// Instantiate an uninitialized instance of the collection wrapper
		/// </summary>
		IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister);

		/// <summary>
		/// Wrap an instance of a collection
		/// </summary>
		IPersistentCollection Wrap(ISessionImplementor session, object collection);

		/// <summary>
		/// Return an <see cref="IEnumerable" /> over the elements of this collection - the passed collection
		/// instance may or may not be a wrapper
		/// </summary>
		IEnumerable GetElements(object collection);

		/// <summary>
		/// Optional operation. Does the collection contain the entity instance?
		/// </summary>
		bool Contains(object collection, object entity);

		/// <summary>
		/// Optional operation. Return the index of the entity in the collection.
		/// </summary>
		object IndexOf(object collection, object entity);

		/// <summary>
		/// Replace the elements of a collection with the elements of another collection
		/// </summary>
		object ReplaceElements(object original, object target, ICollectionPersister persister, object owner,
		                       IDictionary copyCache, ISessionImplementor session);

		/// <summary> 
		/// Instantiate an empty instance of the "underlying" collection (not a wrapper),
		/// but with the given anticipated size (i.e. accounting for initial size
		/// and perhaps load factor).
		///  </summary>
		/// <param name="anticipatedSize">
		/// The anticipated size of the instantiated collection
		/// after we are done populating it.  Note, may be negative to indicate that
		/// we not yet know anything about the anticipated size (i.e., when initializing
		/// from a result set row by row).
		/// </param>		
		object Instantiate(int anticipatedSize);
	}
}
