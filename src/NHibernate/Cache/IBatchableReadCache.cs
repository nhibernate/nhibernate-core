﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines a method for retrieving multiple keys from the cache at once. The implementor
	/// should use this interface along with <see cref="ICache"/> when the cache supports
	/// a multiple get operation.
	/// </summary>
	/// <remarks>
	/// <threadsafety instance="true" />
	/// <para>
	/// All implementations <em>must</em> be threadsafe.
	/// </para>
	/// </remarks>
	public partial interface IBatchableReadCache
	{
		/// <summary>
		/// Get multiple objects from the cache.
		/// </summary>
		/// <param name="keys">The keys to be retrieved from the cache.</param>
		/// <returns></returns>
		object[] GetMultiple(object[] keys);
	}
}
