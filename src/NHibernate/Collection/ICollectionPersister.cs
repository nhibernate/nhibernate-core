using System.Data;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// <p>A strategy for persisting a collection role. Defines a contract between
	/// the persistence strategy and the actual persistent collection framework
	/// and session. Does not define operations that are required for querying
	/// collections, or loading by outer join.</p>
	/// <p>
	/// Implements persistence of a collection instance while the instance is
	/// referenced in a particular role.</p>
	/// <p>
	/// This class is highly coupled to the <see cref="PersistentCollection" />
	/// hierarchy, since double dispatch is used to load and update collection 
	/// elements.</p>
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public interface ICollectionPersister
	{
		/// <summary>
		/// Initialize the given collection with the given key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void Initialize( object key, ISessionImplementor session );

		/// <summary>
		/// Get the cache
		/// </summary>
		ICacheConcurrencyStrategy Cache { get; }

		/// <summary>
		/// Is this collection role cacheable
		/// </summary>
		bool HasCache { get; }

		/// <summary>
		/// Get the associated <c>IType</c>
		/// </summary>
		PersistentCollectionType CollectionType { get; }

		/// <summary>
		/// Get the "key" type (the type of the foreign key)
		/// </summary>
		IType KeyType { get; }

		/// <summary>
		/// Get the "index" type for a list or map (optional operation)
		/// </summary>
		IType IndexType { get; }

		/// <summary>
		/// Get the "element" type
		/// </summary>
		IType ElementType { get; }

		/// <summary>
		/// Return the element class of an array, or null otherwise
		/// </summary>
		System.Type ElementClass { get; }

		/// <summary>
		/// Read the key from a row of the <see cref="IDataReader" />
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object ReadKey( IDataReader rs, ISessionImplementor session );

		/// <summary>
		/// Read the element from a row of the <see cref="IDataReader" />
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object ReadElement( IDataReader rs, object owner, ISessionImplementor session );

		/// <summary>
		/// Read the index from a row of the <see cref="IDataReader" />
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object ReadIndex( IDataReader rs, ISessionImplementor session );

		/// <summary>
		/// Read the identifier from a row of the <see cref="IDataReader" />
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object ReadIdentifier( IDataReader rs, ISessionImplementor session );

		/// <summary>
		/// Write the key to an <see cref="IDbCommand" />
		/// </summary>
		/// <param name="st"></param>
		/// <param name="key"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		void WriteKey( IDbCommand st, object key, bool writeOrder, ISessionImplementor session );

		/// <summary>
		/// Write the element to an <see cref="IDbCommand" />
		/// </summary>
		/// <param name="st"></param>
		/// <param name="elt"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		void WriteElement( IDbCommand st, object elt, bool writeOrder, ISessionImplementor session );

		/// <summary>
		/// Write the index to an <see cref="IDbCommand" />
		/// </summary>
		/// <param name="st"></param>
		/// <param name="idx"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		void WriteIndex( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session );

		/// <summary>
		/// Write the identifier to an <see cref="IDbCommand" />
		/// </summary>
		/// <param name="st"></param>
		/// <param name="idx"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		void WriteIdentifier( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session );

		/// <summary>
		/// Is this an array or primitive values?
		/// </summary>
		bool IsPrimitiveArray { get; }

		/// <summary>
		/// Is this an array?
		/// </summary>
		bool IsArray { get; }

		/// <summary>
		/// Is this a one-to-many association?
		/// </summary>
		bool IsOneToMany { get; }

		/// <summary>
		/// Is this an "indexed" collection? (list or map)
		/// </summary>
		bool HasIndex { get; }

		/// <summary>
		/// Is this collection lazyily initialized?
		/// </summary>
		bool IsLazy { get; }

		/// <summary>
		/// Is this collection "inverse", so state changes are not propogated to the database.
		/// </summary>
		bool IsInverse { get; }

		/// <summary>
		/// Completely remove the persistent state of the collection
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		void Remove( object id, ISessionImplementor session );

		/// <summary>
		/// (Re)create the collection's persistent state
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void Recreate( PersistentCollection collection, object key, ISessionImplementor session );

		/// <summary>
		/// Delete the persistent state of any elements that were removed from the collection
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void DeleteRows( PersistentCollection collection, object key, ISessionImplementor session );

		/// <summary>
		/// Update the persistent state of any elements that were modified
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void UpdateRows( PersistentCollection collection, object key, ISessionImplementor session );

		/// <summary>
		/// Insert the persistent state of any new collection elements
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void InsertRows( PersistentCollection collection, object key, ISessionImplementor session );

		/// <summary>
		/// Get the name of this collection role (the fully qualified class name, extended by a "property path")
		/// </summary>
		string Role { get; }

		/// <summary>
		/// Get the entity class that "owns" this collection
		/// </summary>
		System.Type OwnerClass { get; }

		/// <summary>
		/// Get the surrogate key generation strategy (optional operation)
		/// </summary>
		IIdentifierGenerator IdentifierGenerator { get; }

		/// <summary>
		/// Get the type of the surrogate key
		/// </summary>
		IType IdentifierType { get; }

		/// <summary>
		/// Does this collection implement "orphan delete"?
		/// </summary>
		bool HasOrphanDelete { get; }

		/// <summary>
		/// Is this an ordered collection? (An ordered collection is
		/// ordered by the initialization operation, not by sorting
		/// that happens in memory, as in the case of a sorted collection.)
		/// </summary>
		bool HasOrdering { get; }

		/// <summary>
		/// Get the "space" that holds the persistent state
		/// </summary>
		object CollectionSpace { get; }

		/// <summary>
		/// 
		/// </summary>
		ICollectionMetadata CollectionMetadata { get; }
	}
}
