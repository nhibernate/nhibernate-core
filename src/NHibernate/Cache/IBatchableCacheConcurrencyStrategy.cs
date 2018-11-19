﻿using System;
using System.Collections;
using System.Text;
using NHibernate.Cache.Entry;

namespace NHibernate.Cache
{
	/// <summary>
	/// Implementors manage transactional access to cached data.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Transactions pass in a timestamp indicating transaction start time.
	/// </para>
	/// <para>
	/// When used to cache entities and collections the key is the identifier of the
	/// entity/collection and the value should be set to the <see cref="CacheEntry"/> 
	/// for an entity and the results of <see cref="Collection.AbstractPersistentCollection.Disassemble"/>
	/// for a collection.
	/// </para>
	/// </remarks>
	// 6.0 TODO: merge into ICacheConcurrencyStrategy
	public partial interface IBatchableCacheConcurrencyStrategy : ICacheConcurrencyStrategy
	{
		/// <summary>
		/// Attempt to retrieve multiple items from the cache.
		/// </summary>
		/// <param name="keys">The keys of the items.</param>
		/// <param name="timestamp">A timestamp prior to the transaction start time.</param>
		/// <returns>The cached items, matching each key of <paramref name="keys"/> respectively. For each missed key,
		/// it will contain a <see langword="null" />.</returns>
		/// <exception cref="CacheException"></exception>
		object[] GetMany(CacheKey[] keys, long timestamp);

		/// <summary>
		/// Attempt to cache items, after loading them from the database.
		/// </summary>
		/// <param name="keys">The keys of the items.</param>
		/// <param name="values">The items.</param>
		/// <param name="timestamp">A timestamp prior to the transaction start time.</param>
		/// <param name="versions">The version numbers of the items.</param>
		/// <param name="versionComparers">The comparers to be used to compare version numbers.</param>
		/// <param name="minimalPuts">Indicates that the cache should avoid a put if the item is already cached.</param>
		/// <returns>An array of boolean indicating if each item was successfully cached.</returns>
		/// <exception cref="CacheException"></exception>
		bool[] PutMany(CacheKey[] keys, object[] values, long timestamp, object[] versions, IComparer[] versionComparers,
								  bool[] minimalPuts);

		// 6.0 TODO: remove for using ICacheConcurrencyStrategy.Cache re-typed CacheBase instead
		new CacheBase Cache { get; set; }
	}
}
