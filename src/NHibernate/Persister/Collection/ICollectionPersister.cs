using System.Collections.Generic;
using System.Data;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// A strategy for persisting a collection role.
	/// </summary>
	/// <remarks>
	/// Defines a contract between the persistence strategy and the actual persistent collection framework
	/// and session. Does not define operations that are required for querying collections, or loading by outer join.
	/// <para/>
	/// Implements persistence of a collection instance while the instance is
	/// referenced in a particular role.
	/// <para/>
	/// This class is highly coupled to the <see cref="IPersistentCollection" />
	/// hierarchy, since double dispatch is used to load and update collection 
	/// elements.
	/// <para/>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public interface ICollectionPersister
	{
		/// <summary>
		/// Get the cache
		/// </summary>
		ICacheConcurrencyStrategy Cache { get; }

		/// <summary> Get the cache structure</summary>
		ICacheEntryStructure CacheEntryStructure { get;}

		/// <summary>
		/// Get the associated <c>IType</c>
		/// </summary>
		CollectionType CollectionType { get; }

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
		/// Is this an array or primitive values?
		/// </summary>
		bool IsPrimitiveArray { get; }

		/// <summary>
		/// Is this an array?
		/// </summary>
		bool IsArray { get; }

		/// <summary> Is this a one-to-many association?</summary>
		bool IsOneToMany { get;}

		/// <summary> 
		/// Is this a many-to-many association?  Note that this is mainly
		/// a convenience feature as the single persister does not
		/// contain all the information needed to handle a many-to-many
		/// itself, as internally it is looked at as two many-to-ones.
		/// </summary>
		bool IsManyToMany { get;}

		/// <summary>
		/// Is this collection lazily initialized?
		/// </summary>
		bool IsLazy { get; }

		/// <summary>
		/// Is this collection "inverse", so state changes are not propogated to the database.
		/// </summary>
		bool IsInverse { get; }

		/// <summary>
		/// Get the name of this collection role (the fully qualified class name, extended by a "property path")
		/// </summary>
		string Role { get; }

		/// <summary> Get the persister of the entity that "owns" this collection</summary>
		IEntityPersister OwnerEntityPersister { get;}

		/// <summary>
		/// Get the surrogate key generation strategy (optional operation)
		/// </summary>
		IIdentifierGenerator IdentifierGenerator { get; }

		/// <summary>
		/// Get the type of the surrogate key
		/// </summary>
		IType IdentifierType { get; }

		/// <summary> Get the "space" that holds the persistent state</summary>
		string[] CollectionSpaces { get; }

		ICollectionMetadata CollectionMetadata { get; }

		/// <summary> 
		/// Is cascade delete handled by the database-level
		/// foreign key constraint definition?
		/// </summary>
		bool CascadeDeleteEnabled { get;}

		/// <summary> 
		/// Does this collection cause version increment of the owning entity?
		/// </summary>
		bool IsVersioned { get; }

		/// <summary> Can the elements of this collection change?</summary>
		bool IsMutable { get;}

		string NodeName { get;}

		string ElementNodeName { get;}

		string IndexNodeName { get;}

		ISessionFactoryImplementor Factory { get; }
		bool IsExtraLazy { get;}

		/// <summary>
		/// Initialize the given collection with the given key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void Initialize(object key, ISessionImplementor session);

		/// <summary>
		/// Is this collection role cacheable
		/// </summary>
		bool HasCache { get; }

		/// <summary>
		/// Read the key from a row of the <see cref="IDataReader" />
		/// </summary>
		object ReadKey(IDataReader rs, string[] keyAliases, ISessionImplementor session);

		/// <summary>
		/// Read the element from a row of the <see cref="IDataReader" />
		/// </summary>
		//TODO: the ReadElement should really be a parameterized TElement
		object ReadElement(IDataReader rs, object owner, string[] columnAliases, ISessionImplementor session);

		/// <summary>
		/// Read the index from a row of the <see cref="IDataReader" />
		/// </summary>
		//TODO: the ReadIndex should really be a parameterized TIndex
		object ReadIndex(IDataReader rs, string[] columnAliases, ISessionImplementor session);

		/// <summary>
		/// Read the identifier from a row of the <see cref="IDataReader" />
		/// </summary>
		//TODO: the ReadIdentifier should really be a parameterized TIdentifier
		object ReadIdentifier(IDataReader rs, string columnAlias, ISessionImplementor session);

		string GetManyToManyFilterFragment(string alias, IDictionary<string, IFilter> enabledFilters);

		/// <summary>
		/// Is this an "indexed" collection? (list or map)
		/// </summary>
		bool HasIndex { get; }

		/// <summary>
		/// Completely remove the persistent state of the collection
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		void Remove(object id, ISessionImplementor session);

		/// <summary>
		/// (Re)create the collection's persistent state
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void Recreate(IPersistentCollection collection, object key, ISessionImplementor session);

		/// <summary>
		/// Delete the persistent state of any elements that were removed from the collection
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void DeleteRows(IPersistentCollection collection, object key, ISessionImplementor session);

		/// <summary>
		/// Update the persistent state of any elements that were modified
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void UpdateRows(IPersistentCollection collection, object key, ISessionImplementor session);

		/// <summary>
		/// Insert the persistent state of any new collection elements
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="key"></param>
		/// <param name="session"></param>
		void InsertRows(IPersistentCollection collection, object key, ISessionImplementor session);

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

		bool HasManyToManyOrdering { get; }

		void PostInstantiate();

		bool IsAffectedByEnabledFilters(ISessionImplementor session);

		/// <summary>
		/// Generates the collection's key column aliases, based on the given
		/// suffix.
		/// </summary>
		/// <param name="suffix">The suffix to use in the key column alias generation.</param>
		/// <returns>The key column aliases.</returns>
		string[] GetKeyColumnAliases(string suffix);

		/// <summary>
		/// Generates the collection's index column aliases, based on the given
		/// suffix.
		/// </summary>
		/// <param name="suffix">The suffix to use in the index column alias generation.</param>
		/// <returns>The index column aliases, or null if not indexed.</returns>
		string[] GetIndexColumnAliases(string suffix);

		/// <summary>
		/// Generates the collection's element column aliases, based on the given
		/// suffix.
		/// </summary>
		/// <param name="suffix">The suffix to use in the element column alias generation.</param>
		/// <returns>The element column aliases.</returns>
		string[] GetElementColumnAliases(string suffix);

		/// <summary>
		/// Generates the collection's identifier column aliases, based on the given
		/// suffix.
		/// </summary>
		/// <param name="suffix">The suffix to use in the identifier column alias generation.</param>
		/// <returns>The identifier column aliases.</returns>
		string GetIdentifierColumnAlias(string suffix);

		int GetSize(object key, ISessionImplementor session);
		bool IndexExists(object key, object index, ISessionImplementor session);
		bool ElementExists(object key, object element, ISessionImplementor session);
		object GetElementByIndex(object key, object index, ISessionImplementor session, object owner);
	}
}
