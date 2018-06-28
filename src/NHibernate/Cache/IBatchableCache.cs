﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines methods for retrieving and adding multiple objects from/to the cache at once.
	/// The implementor should use this interface along with <see cref="ICache"/> when the
	/// cache supports a multiple get and put operation.
	/// </summary>
	/// <remarks>
	/// <threadsafety instance="true" />
	/// <para>
	/// All implementations <em>must</em> be threadsafe.
	/// </para>
	/// </remarks>
	public partial interface IBatchableCache : IBatchableReadOnlyCache
	{
		/// <summary>
		/// Add multiple objects to the cache.
		/// </summary>
		/// <param name="keys">The keys to cache.</param>
		/// <param name="values">The objects to cache.</param>
		void PutMany(object[] keys, object[] values);

		/// <summary>
		/// Lock the objects from being changed by another thread.
		/// </summary>
		/// <param name="keys">The keys to lock.</param>
		/// <returns>The value that was used to lock the keys.</returns>
		object LockMany(object[] keys);

		/// <summary>
		/// Unlock the objects that were previously locked.
		/// </summary>
		/// <param name="keys">The keys to unlock.</param>
		/// <param name="lockValue">The value that was used to lock the keys.</param>
		void UnlockMany(object[] keys, object lockValue);
	}
}
