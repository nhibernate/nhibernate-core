﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IBatchableReadWriteCache : IBatchableReadCache
	{
		/// <summary>
		/// Add multiple objects to the cache.
		/// </summary>
		/// <param name="keys">The keys to cache.</param>
		/// <param name="values">The objects to cache.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task PutManyAsync(object[] keys, object[] values, CancellationToken cancellationToken);

		/// <summary>
		/// Lock the objects from being changed by another thread.
		/// </summary>
		/// <param name="keys">The keys to lock.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task LockManyAsync(object[] keys, CancellationToken cancellationToken);

		/// <summary>
		/// Unlock the objects that were previously locked.
		/// </summary>
		/// <param name="keys">The keys to unlock.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task UnlockManyAsync(object[] keys, CancellationToken cancellationToken);
	}
}
